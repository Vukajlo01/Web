using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebProject.Models
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public int Amount { get; set; }
        public string BuyerUsername { get; set; }
        public DateTime OrderingDate { get; set; }
        public Status Status { get; set; }

        public Order()
        {

        }

        public Order(string[] tokens)
        {
            Id = Guid.Parse(tokens[0]);
            ProductId = Guid.Parse(tokens[1]);
            Amount = int.Parse(tokens[2]);
            BuyerUsername = tokens[3];
            OrderingDate = DateTime.Parse(tokens[4]);
            Status = (Status)Enum.Parse(typeof(Status), tokens[5]);
        }
    }
}