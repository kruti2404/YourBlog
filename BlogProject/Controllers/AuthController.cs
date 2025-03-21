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
        private IGenericRepository<User> _UserRepository;

        private ProgramDbContext _context;
        public AuthController(JwtServices services, IGenericRepository<User> userRepo, ProgramDbContext context)
        {
            _service = services;
            _UserRepository = userRepo;
            _context = context;

        }

        public async Task<IActionResult> Index()
        {
            var users = await _UserRepository.GetAll();
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
            Console.WriteLine(user.UserName);
            Console.WriteLine(user.Password);
            if (!ModelState.IsValid)
            {
                return View(user);
            }
            var GatedUSer = await _context.User.FirstOrDefaultAsync(x => x.UserName == user.UserName);
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

            //Console.WriteLine(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role).Value);
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
            var UserMatch = _context.User.FirstOrDefault(x => x.UserName == user.UserName);
            if(UserMatch != null)
            {
                ModelState.AddModelError("UserName", "UserName is already taken ");
                return View(user);
            }
            var EmailMatch = _context.User.FirstOrDefault(x => x.Email == user.Email);
            if(EmailMatch != null)
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

            await _UserRepository.Insert(newUser);
            await _UserRepository.Save();

            return RedirectToAction("Login");
        }
        [AllowAnonymous]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete("AuthToken");
            return RedirectToAction("Index", "Home");
        }



    }
}
