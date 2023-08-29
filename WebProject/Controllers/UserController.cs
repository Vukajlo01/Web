using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebProject.Models;
using WebProject.Utils;

namespace WebProject.Controllers
{
    public class UserController : ApiController
    {
        [HttpGet]
        [Route("api/users")]
        public IHttpActionResult ListUsers()
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/users.txt");

            var user = Authentication.GetUserFromSession();
            if (user == null)
            {
                return Unauthorized();
            }

            if (user.Role != Role.ADMIN) return Unauthorized();

            List<User> users = new List<User>();
            using (var reader = new StreamReader(filePath, true))
            {
                String line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] tokens = line.Split(';');
                    User u = new User(tokens);
                    users.Add(u);
                }
            }

            return Ok(users);
        }

        [HttpGet]
        [Route("api/user/{username}")]
        public IHttpActionResult RetriveUser(string username)
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/users.txt");

            var user = new User(CRUD.RetrieveInstance(username, 1, filePath).ToArray());

            return Ok(JToken.Parse(JsonConvert.SerializeObject(user, new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd" })));
        }

        [HttpPost]
        [Route("api/user/{username}/favourites")]
        public IHttpActionResult CreateFavourites(string username, Product prod)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!Authentication.ValidateUser(username)) return Unauthorized();

            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/products.txt");
            var product = new Product(CRUD.RetrieveInstance(prod.Id.ToString(), 0, filePath).ToArray());

            filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/users.txt");
            var instance = CRUD.RetrieveInstance(username, 1, filePath).ToArray();
            var user = new User(instance);

            user.FavouriteProductIds.Add(product.Id);

            CRUD.UpdateInstance(username, 1, user, filePath);

            return Ok("Successfully added product to favourites");
        }

        [HttpGet]
        [Route("api/user/{username}/favourites")]
        public IHttpActionResult ListFavourites(string username)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!Authentication.ValidateUser(username)) return Unauthorized();

            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/users.txt");
            var instance = CRUD.RetrieveInstance(username, 1, filePath).ToArray();
            var user = new User(instance);

            return Ok(user.FavouriteProductIds);
        }

        [HttpDelete]
        [Route("api/user/{username}/favourite/{id}")]
        public IHttpActionResult DeleteFavourite(string username, Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!Authentication.ValidateUser(username)) return Unauthorized();

            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/users.txt");
            var instance = CRUD.RetrieveInstance(username, 1, filePath).ToArray();
            var user = new User(instance);

            user.FavouriteProductIds.Remove(id);

            CRUD.UpdateInstance(username, 1, user, filePath);

            return Ok("Removal successfull");
        }

        [HttpPut]
        [Route("api/user/{username}")]
        public IHttpActionResult UpdateUser(string username, User user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!Authentication.ValidateUser(username)) return Unauthorized();

            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/users.txt");

            var oldUser = new User(CRUD.RetrieveInstance(username, 1 , filePath).ToArray());

            user.Id = oldUser.Id;
            user.Role = oldUser.Role;

            CRUD.UpdateInstance(username, 1, user, filePath);

            return Ok("Registration successful!");
        }

        [HttpPost]
        [Route("api/auth/login")]
        public IHttpActionResult Login(User user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/users.txt");

            // Check if the user exists in the text file
            var lines = File.ReadAllLines(filePath);
            var existingUser = lines.FirstOrDefault(line =>
            {
                var data = line.Split(';');
                return data[1] == user.Username && data[2] == user.Password;
            });

            if (existingUser == null)
                return BadRequest("Invalid username or password.");

            // Retrieve the user's role from the existingUser string
            var loggedUser = new User(existingUser.Split(';'));

            Authentication.CreateSession(loggedUser.Username);

            return Ok(loggedUser);
        }

        [HttpPost]
        [Route("api/auth/register")]
        public IHttpActionResult Register(User user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/users.txt");

            // Check if the username is already taken
            var lines = File.ReadAllLines(filePath);
            if (lines.Any(line => line.Split(';')[1] == user.Username))
                return BadRequest("Username is already taken.");

            // Generate a new GUID as the ID
            user.Id = Guid.NewGuid();

            // Serialize the user object to a string
            CRUD.AddInstance(user, filePath);

            return Ok("Registration successful!");
        }

        [HttpPost]
        [Route("api/auth/logout")]
        public IHttpActionResult Logout()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = Authentication.GetUserFromSession();
            if (user == null)
            {
                return Unauthorized();
            }

            Authentication.DeleteSession();

            return Ok("Registration successful!");
        }
    }
}
