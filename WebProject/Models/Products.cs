using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace WebProject.Models
{
    public class Products
    {
        public static List<Product> listOfProducts { get; set; }

        public Products(string path)
        {
            path = HostingEnvironment.MapPath(path);
            listOfProducts = new List<Product>();
            FileStream stream = new FileStream(path, FileMode.Open);
            StreamReader sr = new StreamReader(stream);
            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                string[] tokens = line.Split(';');
                Product p = new Product(tokens[0], double.Parse(tokens[1]), int.Parse(tokens[2]));
                listOfProducts.Add(p);
            }
            sr.Close();
            stream.Close();
        }
    }
}