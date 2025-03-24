using System.Threading.Tasks;
using BlogProject.Models;
using BlogProject.Repository;
using BlogProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private UserServices _UserRepository;
        private BlogServices _BlogsRepository;
        public AdminController(UserServices userRepo, BlogServices blogRepo)
        {
            _UserRepository = userRepo;
            _BlogsRepository = blogRepo;

        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Allusers()
        {

            return View(_UserRepository.GetAll());
        }
        public IActionResult AllBlogs()
        {

            return View(_BlogsRepository.GetAll());
        }
    }
}
