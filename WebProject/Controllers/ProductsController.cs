using System;
using System.Collections.Generic;
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
    }
}
