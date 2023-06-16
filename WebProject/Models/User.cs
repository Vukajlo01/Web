using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebProject.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        public string Username { get; set; } //unique
        public string Password { get; set; }
        //public string FirstName { get; set; }
        //public string LastName { get; set; }
        //public string Gender { get; set; }
        //public string Email { get; set; }
        //public DateTime Birthdate { get; set; } //dd/MM/yyyy
        public Role Role { get; set; }
        //public List<Order> Orders { get; set; } //only if role = Buyer
        //public List<Product> FavProducts { get; set; } //only if role = Buyer
        //public List<Product> PublishedProducts { get; set; } //only if role = Prodavac

    }
}