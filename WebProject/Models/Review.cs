using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebProject.Models
{
    public class Review
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string AuthorUsername { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool isDeleted { get; set; }
        
        public Review()
        {

        }

        public Review(string[] tokens)
        {
            Id = Guid.Parse(tokens[0]);
            ProductId = Guid.Parse(tokens[1]);
            AuthorUsername = tokens[2];
            Title = tokens[3];
            Content = tokens[4];
            isDeleted = bool.Parse(tokens[5]);
        }
    }
}