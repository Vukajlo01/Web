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
    public class ProductsController : ApiController
    {
        [HttpGet]
        [Route("api/products")]
        public List<Product> ListProducts()
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/products.txt");

            List<Product> products = new List<Product>();
            using (var reader = new StreamReader(filePath, true))
            {
                String line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] tokens = line.Split(';');
                    Product p = new Product(tokens);
                    if (p.Amount <= 0) p.Available = false;
                    products.Add(p);
                }
            }

            return products;
        }

        [HttpPost]
        [Route("api/products")]
        public IHttpActionResult CreateProducts(Product product)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = Authentication.GetUserFromSession();
            if (user == null) return Unauthorized();

            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/products.txt");

            // Generate a new GUID as the ID
            product.Id = Guid.NewGuid();
            product.PublishingDate = DateTime.Now;
            product.Available = true;
            product.SellerUsername = user.Username;

            // Serialize the user object to a string
            CRUD.AddInstance(product, filePath);

            return Ok(product.Id.ToString());
        }

        [HttpDelete]
        [Route("api/product/{id}")]
        public IHttpActionResult DeleteProduct(Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = Authentication.GetUserFromSession();
            if (user == null) return Unauthorized();

            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/products.txt");

            var instance = new Product(CRUD.RetrieveInstance(id.ToString(), 0, filePath).ToArray());

            instance.Available = false;
            if (!Authentication.ValidateUser(instance.SellerUsername)) return Unauthorized();

            CRUD.UpdateInstance(id.ToString(), 0, instance, filePath);

            return Ok("Deletion successful");
        }

        [HttpPut]
        [Route("api/product/{id}")]
        public IHttpActionResult UpdateProduct(Guid id, Product product)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = Authentication.GetUserFromSession();
            if (user == null) return Unauthorized();

            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/products.txt");

            var oldProduct = new Product(CRUD.RetrieveInstance(id.ToString(), 0, filePath).ToArray());

            product.Available = oldProduct.Available;
            product.SellerUsername = oldProduct.SellerUsername;
            if (!oldProduct.Available) return NotFound();
            if (!Authentication.ValidateUser(oldProduct.SellerUsername)) return Unauthorized();

            // Serialize the user object to a string
            CRUD.UpdateInstance(id.ToString(), 0, product, filePath);

            return Ok("Registration successful!");
        }

        [HttpGet]
        [Route("api/product/{id}")]
        public IHttpActionResult RetrieveProduct(Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/products.txt");

            var product = new Product(CRUD.RetrieveInstance(id.ToString(), 0, filePath).ToArray());
            if (product.Amount <= 0) product.Available = true;

            return Ok(product);
        }

        [HttpPost]
        [Route("api/product/{id}/image")]
        public IHttpActionResult UpdateProductImage(Guid id) {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = Authentication.GetUserFromSession();
            if (user == null) return Unauthorized();

            if (HttpContext.Current.Request.Files.Count < 1 || HttpContext.Current.Request.Files[0] == null)
            {
                return BadRequest();
            }

            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/products.txt");
            var product = new Product(CRUD.RetrieveInstance(id.ToString(), 0, filePath).ToArray());

            if (!product.Available) return NotFound();
            if (!Authentication.ValidateUser(product.SellerUsername)) return Unauthorized();

            var extension = HttpContext.Current.Request.Files[0].FileName.Split('.').Last();
            filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/uploads/products/" + product.Id.ToString() + "." + extension);
            HttpContext.Current.Request.Files[0].SaveAs(filePath);

            return Ok();
        }

        [HttpGet]
        [Route("api/product/{id}/image")]
        public IHttpActionResult RetrieveProductImage(Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/products.txt");
            var product = new Product(CRUD.RetrieveInstance(id.ToString(), 0, filePath).ToArray());
            if (!product.Available) return NotFound();

            filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/uploads/products/");

            var imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "noImage.jpg");
            var files = Directory.GetFiles(filePath);
            foreach(var i in files)
            {
                if (Path.GetFileName(i).StartsWith(product.Id.ToString()))
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
