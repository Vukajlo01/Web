using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;

namespace WebProject.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public Role Role { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public DateTime Birthdate { get; set; }
        public HashSet<Guid> FavouriteProductIds { get; set; }

        public User ()
        {

        }

        public User (string[] tokens)
        {
            Id = Guid.Parse(tokens[0]);
            Username = tokens[1];
            Password = tokens[2];
            Role = (Role)Enum.Parse(typeof(Role), tokens[3]);
            FirstName = tokens[4];
            LastName = tokens[5];
            Gender = tokens[6];
            Email = tokens[7];
            Birthdate = DateTime.ParseExact(tokens[8], "yyyy-MM-dd", CultureInfo.InvariantCulture);
            HashSet<Guid> favs = new HashSet<Guid>();
            foreach(var i in tokens[9].Split(','))
            {
                var guid = Guid.Empty;
                if (Guid.TryParse(i, out guid))
                {
                    favs.Add(guid);
                }
            }
            FavouriteProductIds = favs;
        }
    }
}