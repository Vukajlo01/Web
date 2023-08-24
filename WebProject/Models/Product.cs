using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Web;

namespace WebProject.Models
{
    public class Product
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int Amount { get; set; }
        public string SellerUsername { get; set; }      
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public Byte[] ProductImage { get; set; }
        public DateTime PublishingDate { get; set; } //dd/MM/yyyy
        public string City { get; set; }
        public List<Review> reviews { get; set; }
        public bool Available { get; set; }

        public Product(Guid productId, string name, double price, int amount, string sellerUsername, string description, DateTime publishingDate, string city, bool available)
        {
            ProductId = productId;
            Name = name;
            Price = price;
            Amount = amount;
            SellerUsername = sellerUsername;
            Description = description;
            PublishingDate = publishingDate;
            City = city;
            Available = available;
        }
    }
}