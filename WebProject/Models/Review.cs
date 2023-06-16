using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebProject.Models
{
    public class Review
    {
        public Product Product { get; set; }
        //public User ReviewAuthor { get; set; }
        public string Title { get; set; }
        //public string ReviewContent { get; set; }
        ////public ??? Image { get; set; }
    }
}