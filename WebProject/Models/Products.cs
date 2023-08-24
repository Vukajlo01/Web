using System;
using System.Collections.Generic;
using System.Drawing;
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

                //Byte[] productImage = File.ReadAllBytes(tokens[9]);
                Product p = new Product(Guid.Parse(tokens[0]),tokens[1], double.Parse(tokens[2]), int.Parse(tokens[3]), tokens[4], tokens[5], DateTime.Parse(tokens[6]), tokens[7], bool.Parse(tokens[8]));
                listOfProducts.Add(p);
            }
            sr.Close();
            stream.Close();
        }
    }
}