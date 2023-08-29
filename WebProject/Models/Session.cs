using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebProject.Models
{
    public class Session
    {
        [Key]
        public Guid Id { get; set; }
        public string UserUsername { get; set; }

        public Session()
        {

        }

        public Session(string[] tokens)
        {
            Id = Guid.Parse(tokens[0]);
            UserUsername = tokens[1];
        }

        public Session(string userId)
        {
            Id = Guid.NewGuid();
            UserUsername = userId;
        }
    }
}