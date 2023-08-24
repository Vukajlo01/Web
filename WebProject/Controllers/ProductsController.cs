using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebProject.Models;

namespace WebProject.Controllers
{
    public class ProductsController : ApiController
    {
        public List<Product> Get()
        {
            return Products.listOfProducts;
        }

        [HttpPost]
        [Route("api/registerProduct")]
        public IHttpActionResult Register(Product product)
        {           

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/products.txt");

            // Generate a new GUID as the ID
            product.ProductId = Guid.NewGuid();
            product.PublishingDate = DateTime.Now;
            product.Available = true;

            // Serialize the user object to a string
            var productData = $"{product.ProductId};{product.Name};{product.Price};{product.Amount};" +
                $"{product.SellerUsername};{product.Description};{product.PublishingDate};{product.City};" +
                $"{product.Available}";

            // Append the user data to the text file
            using (var writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine(productData);
            }

            return Ok("Registration successful!");
        }
    }
}
