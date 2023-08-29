using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Http;
using WebProject.Models;
using WebProject.Utils;

namespace WebProject.Controllers
{
    public class OrderController : ApiController
    {
        [HttpPost]
        [Route("api/orders")]
        public IHttpActionResult CreateOrders(Order order)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = Authentication.GetUserFromSession();
            if (user == null) return Unauthorized();

            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/orders.txt");

            order.Id = Guid.NewGuid();
            order.OrderingDate = DateTime.Now;
            order.Status = Status.ACTIVE;
            order.BuyerUsername = user.Username;

            CRUD.AddInstance(order, filePath);

            filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/products.txt");

            var instance = CRUD.RetrieveInstance(order.ProductId.ToString(), 0, filePath);

            System.Diagnostics.Debug.WriteLine(instance);
            var product = new Product(instance.ToArray());
            product.Amount -= order.Amount;

            CRUD.UpdateInstance(order.ProductId.ToString(), 0, product, filePath);

            return Ok("Ordering successful");
        }

        [HttpPost]
        [Route("api/order/{id}/cancel")]
        public IHttpActionResult CancelOrder([FromUri] Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = Authentication.GetUserFromSession();
            if (user == null) return Unauthorized();

            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/orders.txt");

            var instance = new Order(CRUD.RetrieveInstance(id.ToString(), 0, filePath).ToArray());
            //TODO: Check user

            instance.Status = Status.CANCELLED;

            if (!Authentication.ValidateUser(instance.BuyerUsername)) return Unauthorized();

            CRUD.UpdateInstance(id.ToString(), 0, instance, filePath);

            return Ok("Order canceled successfully");
        }

        [HttpPost]
        [Route("api/order/{id}/confirm")]
        public IHttpActionResult ConfirmOrder([FromUri] Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = Authentication.GetUserFromSession();
            if (user == null) return Unauthorized();

            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/orders.txt");

            var instance = new Order(CRUD.RetrieveInstance(id.ToString(), 0, filePath).ToArray());

            instance.Status = Status.DELIVERED;

            if (!Authentication.ValidateUser(instance.BuyerUsername)) return Unauthorized();

            CRUD.UpdateInstance(id.ToString(), 0, instance, filePath);

            return Ok("Order confirmed successfully");
        }

        [HttpGet]
        [Route("api/orders")]
        public IHttpActionResult ListOrders()
        {
            var user = Authentication.GetUserFromSession();
            if (user == null) return Unauthorized();

            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/orders.txt");

            List<(Order, Product)> orders = new List<(Order, Product)>();
            using (var reader = new StreamReader(filePath, true))
            {

                filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/products.txt");

                String line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] tokens = line.Split(';');
                    var order = new Order(tokens);
                    
                    var product = new Product(CRUD.RetrieveInstance(order.ProductId.ToString(), 0, filePath).ToArray());

                    if (order.BuyerUsername == user.Username) { 
                        orders.Add((order, product));
                    }
                }
            }

            return Ok(orders.ToArray());
        }
    }
}