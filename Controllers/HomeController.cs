using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using login_and_registration.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace LogReg.Controllers
{
    public class HomeController : Controller
    {
        private MyContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, MyContext dbContext)
        {
            _logger = logger;
            _context = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("register")]
        public IActionResult Register(User newUser)
        {
            if(ModelState.IsValid)
            {
                if(_context.Users.Any(u => u.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use!");
                    return View("Index");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                _context.Users.Add(newUser);
                _context.SaveChanges();
                HttpContext.Session.SetString("UserEmail", newUser.Email);
                return RedirectToAction("Success");
            } else {
                return View("Index");
            }
        }

        [HttpPost("login")]
        public IActionResult Login(LoggedUser logUser)
        {
            if(ModelState.IsValid)
            {
                User userInDb = _context.Users.FirstOrDefault(u => u.Email == logUser.LoggedEmail);
                if(userInDb == null)
                {
                    ModelState.AddModelError("LoggedEmail", "Invalid login attempt, check Email or Password and try again");
                    return View("Login");
                }
                PasswordHasher<LoggedUser> Hasher = new PasswordHasher<LoggedUser>();
                PasswordVerificationResult result = Hasher.VerifyHashedPassword(logUser, userInDb.Password, logUser.LoggedPassword);
                if(result == 0)
                {
                    ModelState.AddModelError("LoggedEmail", "Invalid login attempt, check Email or Password and try again");
                    return View("Login");
                }
                HttpContext.Session.SetString("UserEmail", userInDb.Email);
                return RedirectToAction("Success");
            } else {
                return View("Index");
            }
        }

        [HttpGet("success")]
        public IActionResult Success()
        {
            string email = HttpContext.Session.GetString("UserEmail");
            User loggedIn = _context.Users.FirstOrDefault(u => u.Email == email);
            if(HttpContext.Session.GetString("UserEmail") != null)
            {
                return View(loggedIn);
            } else 
            {
                return RedirectToAction("Index");
            }
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}