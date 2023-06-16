﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebProject.Models;

namespace WebProject.Controllers
{
    public class UserController : ApiController
    {
        [HttpPost]
        [Route("api/register")]
        public IHttpActionResult Register(User user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/users.txt");

            // Check if the username is already taken
            var lines = System.IO.File.ReadAllLines(filePath);
            if (lines.Any(line => line.Split(';')[1] == user.Username))
                return BadRequest("Username is already taken.");

            // Generate a new GUID as the ID
            user.Id = Guid.NewGuid();

            // Serialize the user object to a string
            var userData = $"{user.Id},{user.Username},{user.Password},{user.Role}";

            // Append the user data to the text file
            using (var writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine(userData);
            }

            return Ok("Registration successful!");
        }

        [HttpPost]
        [Route("api/login")]
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
            var role = existingUser.Split(';')[3];
            Role roleEnum;

            if (role == Role.ADMIN.ToString())
            {
                roleEnum = Role.ADMIN;
            }
            else if (role == Role.SELLER.ToString())
            {
                roleEnum = Role.SELLER;
            }
            else
            {
                roleEnum = Role.BUYER;
            }


            // Create a user object with the retrieved role
            var loggedInUser = new User
            {
                Id = Guid.NewGuid(),
                Username = user.Username,
                Password = user.Password,
                Role = roleEnum
            };

            return Ok(loggedInUser);
        }
    }
}