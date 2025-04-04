using BlogProject.Data;
using BlogProject.Models;
using BlogProject.Repository;
using BlogProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogProject.Controllers
{
    public class AuthController : Controller
    {
        private JwtServices _service;

        private readonly UserServices _userServices;
        public AuthController(JwtServices services, UserServices userservices)
        {
            _service = services;
            _userServices = userservices;

        }

        public ActionResult Index()
        {
            var users = _userServices.GetAll();
            return View(users);
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {

            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(User user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            var GatedUSer = _userServices.GetByUserName(user.UserName);
            if (GatedUSer == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid UserName or Password");
                return View(user);
            }
            Console.WriteLine("Gated user name " + GatedUSer.UserName + " " + GatedUSer.Password);
            if (GatedUSer.UserName == user.UserName && user.Password == GatedUSer.Password)
            {
                var token = _service.GenerateToken(GatedUSer);
                Response.Cookies.Append("AuthToken", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Strict
                });
                Response.HttpContext.Session.SetString("Token", token);
            }

            if (GatedUSer.Role == "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                return RedirectToAction("Index", "Blogs");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(Registration user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            //still to implement the error for the UserName and email duplication 
            var UserMatch = _userServices.GetByUserName(user.UserName);
            if (UserMatch != null)
            {
                ModelState.AddModelError("UserName", "UserName is already taken ");
                return View(user);
            }
            var EmailMatch = _userServices.GetByUserName(user.Email);
            if (EmailMatch != null)
            {
                ModelState.AddModelError("Email", "You have already register with this email");
                return View(user);
            }

            var newUser = new User();
            newUser.Name = user.Name;
            newUser.UserName = user.UserName;
            newUser.Email = user.Email;
            newUser.Password = user.Password;
            newUser.Role = user.Role;

            await _userServices.Insert(newUser);
            await _userServices.Save();

            return RedirectToAction("Login");
        }
        [AllowAnonymous]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete("AuthToken");
            return RedirectToAction("Index", "Home");
        }


        public IActionResult SendOtp(string email)
        {
            Random random = new Random();
            string Otp = random.Next(100000, 999999).ToString();


            return View();
        }
        public IActionResult VarifyOtp(string Otp)
        {
            return View();
        }


    }
}
