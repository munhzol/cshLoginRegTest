using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LoginRegTest.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace LoginRegTest.Controllers
{
    public class HomeController : Controller
    {
        private MyContext _context;
        public HomeController(MyContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            // List<User> AllUsers = _context.Users.ToList();
            if(!_isLogged())
                return View();
            else
                return RedirectToAction("Users");
        }

        [HttpGet("users")]
        public IActionResult Users()
        {
            if(!_isLogged())
                return RedirectToAction("Index");
            
            int uid = (int)HttpContext.Session.GetInt32("uid");

            User user = _context.Users.FirstOrDefault(u=>u.UserID==uid);

            return View("Users",user);
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            if(_isLogged()){
               HttpContext.Session.Remove("uid");
            }
            return RedirectToAction("Index");
        }

        [HttpPost("login")]
        public IActionResult Login(LoginUser user)
        {
            if(ModelState.IsValid){
                
            // If inital ModelState is valid, query for a user with provided email
            var userInDb = _context.Users.FirstOrDefault(u => u.Email == user.LoginEmail);
            // If no user exists with provided email
            if(userInDb == null)
            {
                // Add an error to ModelState and return to View!
                ModelState.AddModelError("LoginEmail", "Invalid Email/Password");
                return View("Index");
            }
            
            // Initialize hasher object
            var hasher = new PasswordHasher<LoginUser>();
            
            // verify provided password against hash stored in db
            var result = hasher.VerifyHashedPassword(user, userInDb.Password, user.LoginPassword);
            
            // result can be compared to 0 for failure
            if(result == 0)
            {
                // handle failure (this should be similar to how "existing email" is handled)
                ModelState.AddModelError("Login", "Invalid Email/Password");
                return View("Index");
            }

                HttpContext.Session.SetInt32("uid",userInDb.UserID);
                return RedirectToAction("Users");
            }
            else
                return View("Index");
        }

        [HttpPost("register")]
        public IActionResult Register(User user)
        {
            if(ModelState.IsValid){
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                user.Password = Hasher.HashPassword(user, user.Password);
                _context.Users.Add(user);
                _context.SaveChanges();
                return RedirectToAction("Success");
            }
            else
                return View("Index");
        }

        [HttpGet("success")]
        public IActionResult Success()
        {
            return View("Success");
        }

        private bool _isLogged() {
            return HttpContext.Session.GetInt32("uid")!=null;
        }

    }
}
