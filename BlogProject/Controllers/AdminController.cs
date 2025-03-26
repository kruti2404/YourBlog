using System.Security.Claims;
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
        private LikeServices _LikesRepository;
        private CommentServices _CommentRepository;
        public AdminController(UserServices userRepo, BlogServices blogRepo, LikeServices likeServices, CommentServices commentServices)
        {
            _UserRepository = userRepo;
            _BlogsRepository = blogRepo;
            _LikesRepository = likeServices;
            _CommentRepository = commentServices;
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
        public async Task<IActionResult> UserDetails(int id)
        {
            return View(await _UserRepository.GetById(id));
        }

        [HttpGet]
        public async Task<IActionResult> UserEdit(int id)
        {

            return View(await _UserRepository.GetById(id));
        }
        [HttpPost]
        public async Task<IActionResult> UserEdit(User user)
        {
            var existing = await _UserRepository.GetById(user.Id);
            if (existing == null)
            {
                return NotFound();
            }
            else
            {
                existing.Name = user.Name;
                existing.UserName = user.UserName;
                existing.Email = user.Email;
                _UserRepository.Update(existing);
                await _UserRepository.Save();
            }
            Console.WriteLine("User Details Name: " + user.Name + " UserName: " + user.UserName);
            Console.WriteLine("Edited Run ");
            var User = await _UserRepository.GetById(user.Id);
            return RedirectToAction("AllUsers");
        }

        [HttpGet]
        public async Task<IActionResult> Userdelete(int id)
        {

            return View(await _UserRepository.GetById(id));
        }
        [ActionName("UserDelete")]
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _UserRepository.GetById(id);
            if (user != null)
            {
                _UserRepository.Remove(user);
                await _UserRepository.Save();
            }
            return RedirectToAction("AllUsers");
        }



        public async Task<IActionResult> BlogDetails(int id)
        {
            Blogs blogs = await _BlogsRepository.Details(id);
            if (blogs == null)
            {
                return NotFound();
            }
            var UserId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var isLiked = blogs.Likes.Any(l => l.UserId == UserId);

            ViewBag.IsLiked = isLiked;

            return View(blogs);
        }

        [HttpGet]
        public async Task<IActionResult> BlogDelete(int id)
        {
            //Console.WriteLine("Delete Page");
            Blogs blogs = await _BlogsRepository.Details(id);
            if (blogs == null)
            {
                return NotFound();
            }

            var UserId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var isLiked = blogs.Likes.Any(l => l.UserId == UserId);

            ViewBag.IsLiked = isLiked;

            return View(blogs);
        }



        [HttpPost]
        //[ActionName("BlogDelete")]
        public async Task<IActionResult> DeleteBlogsConfirmed(int id)
        {
            Console.WriteLine("Delete Page");
            Blogs blogs = await _BlogsRepository.Details(id);
            if (blogs != null)
            {
                Console.WriteLine("Blog deleted");

                // comments deleting 
                if (blogs.Comments != null && blogs.Comments.Any())
                {
                    _CommentRepository.RemoveRange(blogs.Comments);
                    await _CommentRepository.Save();

                }

                if (blogs.Likes != null && blogs.Likes.Any())
                {

                    _LikesRepository.RemoveRange(blogs.Likes);
                    await _LikesRepository.Save();

                }

                _BlogsRepository.Remove(blogs);
                await _BlogsRepository.Save();
            }
            else
            {
                Console.WriteLine("The blog doest not exists ");

                return View(id);
            }
            return RedirectToAction("AllBlogs");
        }


    }
}
