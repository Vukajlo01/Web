using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using WebProject.Models;
using WebProject.Utils;

namespace WebProject.Controllers
{
    public class ReviewsController : ApiController
    {
        [HttpGet]
        [Route("api/product/{id}/reviews")]
        public IHttpActionResult ListReviews([FromUri] Guid id)
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/products.txt");

            var product = new Product(CRUD.RetrieveInstance(id.ToString(), 0, filePath).ToArray());

            filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/reviews.txt");

            List<Review> reviews = new List<Review>();
            using (var reader = new StreamReader(filePath, true))
            {
                String line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] tokens = line.Split(';');
                    Review r = new Review(tokens);
                    if (r.ProductId == product.Id && !r.isDeleted)
                    {
                        reviews.Add(r);
                    }
                }
            }

            return Ok(reviews);
        }

        [HttpPost]
        [Route("api/product/{id}/reviews")]
        public IHttpActionResult CreateReviews([FromUri] Guid id, [FromBody] Review review)
        {
            var user = Authentication.GetUserFromSession();
            if (user == null) return Unauthorized();

            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/products.txt");

            var product = new Product(CRUD.RetrieveInstance(id.ToString(), 0, filePath).ToArray());

            filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/reviews.txt");

            review.Id = Guid.NewGuid();
            review.ProductId = id;
            review.isDeleted = false;
            review.AuthorUsername = user.Username;

            CRUD.AddInstance(review, filePath);

            return Ok(review.Id);
        }

        [HttpPut]
        [Route("api/product/{id}/reviews/{rid}")]
        public IHttpActionResult UpdateReview([FromUri] Guid id, [FromUri] Guid rid, [FromBody] Review review)
        {
            var user = Authentication.GetUserFromSession();
            if (user == null) return Unauthorized();

            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/products.txt");

            var product = new Product(CRUD.RetrieveInstance(id.ToString(), 0, filePath).ToArray());

            filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/reviews.txt");

            var oldReview = new Review(CRUD.RetrieveInstance(rid.ToString(), 0, filePath).ToArray());

            review.Id = rid;
            review.ProductId = id;
            review.isDeleted = oldReview.isDeleted;
            review.AuthorUsername = user.Username;

            if (!Authentication.ValidateUser(oldReview.AuthorUsername)) return Unauthorized();

            CRUD.UpdateInstance(rid.ToString(), 0, review, filePath);

            return Ok("Review updated successfully");
        }

        [HttpDelete]
        [Route("api/product/{id}/reviews/{rid}")]
        public IHttpActionResult DeleteReview([FromUri] Guid id, [FromUri] Guid rid)
        {
            var user = Authentication.GetUserFromSession();
            if (user == null) return Unauthorized();

            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/products.txt");

            var product = new Product(CRUD.RetrieveInstance(id.ToString(), 0, filePath).ToArray());

            filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/reviews.txt");

            var review = new Review(CRUD.RetrieveInstance(rid.ToString(), 0, filePath).ToArray());

            review.Id = rid;
            review.ProductId = id;
            review.isDeleted = true;

            if (!Authentication.ValidateUser(review.AuthorUsername)) return Unauthorized();

            CRUD.UpdateInstance(rid.ToString(), 0, review, filePath);

            return Ok("Review deleted successfully");
        }

        [HttpPost]
        [Route("api/review/{id}/image")]
        public IHttpActionResult UpdateProductImage(Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = Authentication.GetUserFromSession();
            if (user == null) return Unauthorized();

            if (HttpContext.Current.Request.Files.Count < 1 || HttpContext.Current.Request.Files[0] == null)
            {
                return BadRequest();
            }

            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/reviews.txt");
            var review = new Review(CRUD.RetrieveInstance(id.ToString(), 0, filePath).ToArray());

            if (!Authentication.ValidateUser(review.AuthorUsername)) return Unauthorized();

            var extension = HttpContext.Current.Request.Files[0].FileName.Split('.').Last();
            filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/uploads/reviews/" + review.Id.ToString() + "." + extension);
            HttpContext.Current.Request.Files[0].SaveAs(filePath);

            return Ok();
        }

        [HttpGet]
        [Route("api/review/{id}/image")]
        public IHttpActionResult RetrieveProductImage(Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/reviews.txt");
            var review = new Review(CRUD.RetrieveInstance(id.ToString(), 0, filePath).ToArray());

            filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/uploads/reviews/");

            var imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "noImage.jpg");
            var files = Directory.GetFiles(filePath);
            foreach (var i in files)
            {
                if (Path.GetFileName(i).StartsWith(review.Id.ToString()))
                {
                    imagePath = i;
                    break;
                }
            }

            HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK);
            resp.Content = new ByteArrayContent(System.IO.File.ReadAllBytes(imagePath));
            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            return ResponseMessage(resp);
        }
    }
}