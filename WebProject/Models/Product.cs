using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Web;

namespace WebProject.Models
{
    public class Product
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int Amount { get; set; }
        public string SellerUsername { get; set; }      
        public string Description { get; set; }
        public DateTime PublishingDate { get; set; }
        public string City { get; set; }
        public bool Available { get; set; }

        public Product()
        {
        }

        public Product(string[] tokens)
        {
            Id = Guid.Parse(tokens[0]);
            Name = tokens[1];
            Price = double.Parse(tokens[2]);
            Amount = int.Parse(tokens[3]);
            SellerUsername = tokens[4];
            Description = tokens[5];
            PublishingDate = DateTime.ParseExact(tokens[6], "yyyy-MM-dd", CultureInfo.InvariantCulture);
            City = tokens[7];
            Available = bool.Parse(tokens[8]);
        }
    }
}