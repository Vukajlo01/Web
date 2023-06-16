using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebProject.Models
{
    public class Order
    {
        public Product product { get; set; }
        public int Amount { get; set; }
        //public User Buyer { get; set; }
        //public DateTime OrderingDate { get; set; }
        //public Status Status { get; set; }
    }
}