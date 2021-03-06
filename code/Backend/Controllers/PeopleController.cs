﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;

namespace MyWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PeopleController : ControllerBase
    {
        private List<Bratkartoffel> people = new List<Bratkartoffel>()
        {
            new Bratkartoffel() {Name = "Alex" },
            new Bratkartoffel() {Name = "Ben" },
            new Bratkartoffel() {Name = "Lukas" },
            new Bratkartoffel() {Name = "Dan" },
            new Bratkartoffel() {Name = "Umberto" },

        };

        [HttpGet]
        public Bratkartoffel GetPerson()
        {
            var rand = new Random();
            return people[rand.Next(0, people.Count)];
        }
    }
}
