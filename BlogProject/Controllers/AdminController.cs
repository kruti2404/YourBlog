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
        private IGenericRepository<User> _UserRepository;
        private IGenericRepository<Blogs> _BlogsRepository;
        public AdminController(IGenericRepository<User> userRepo, IGenericRepository<Blogs> blogRepo)
        {
            _UserRepository = userRepo;
            _BlogsRepository = blogRepo;

        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Allusers()
        {

            var users = await _UserRepository.GetAll();
            return View(users);
        }
        public async Task<IActionResult> AllBlogs()
        {

            var blogs = await _BlogsRepository.GetAll();
            return View(blogs);
        }
    }
}
