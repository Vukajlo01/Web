using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebProject.Models
{
    public class Product
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public int Amount { get; set; }

        public Product(string name, double price, int amount)
        {
            Name = name;
            Price = price;
            Amount = amount;
        }
        //public string Description { get; set; }
        ////public ??? Image { get; set; }
        ////public DateTime PublishingDate { get; set; } //dd/MM/yyyy
        //public string City { get; set; }
        //public List<Review> reviews { get; set; }
        //public bool Available { get; set; }


    }
}