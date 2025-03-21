using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BlogProject.Models;
using Microsoft.AspNetCore.Authorization;
using BlogProject.Repository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BlogProject.Data;
using Microsoft.Data.SqlClient;
using System.Drawing.Printing;

namespace BlogsProject.Controllers
{
    [Authorize(Roles = "User")]
    public class BlogsController : Controller
    {
        private readonly IGenericRepository<Blogs> _BlogsRepository;
        private readonly ProgramDbContext _context;
        private readonly IGenericRepository<Genre> _GenreRepository;


        public BlogsController(IGenericRepository<Blogs> context, ProgramDbContext context2, IGenericRepository<Genre> GenreRepo)
        {
            _BlogsRepository = context;
            _context = context2;
            _GenreRepository = GenreRepo;
        }

        // GET: Blogs1
        public async Task<IActionResult> Index(string SearchTerm, int PageSize = 3, int PageNumber = 1)
        {

            var totalRecordsParam = new SqlParameter
            {
                ParameterName = "@TotalRecords",
                DbType = System.Data.DbType.Int32,
                Direction = System.Data.ParameterDirection.Output,
                Size = sizeof(int)
            };


            var PaginatedBlogs = _context.Database
                                        .SqlQueryRaw<BlogsGenreDetails>("Exec SP_PaginatedSeachResult @SearchTerm, @PageSize, @PageNumber, @TotalRecords OUTPUT",
                                        new SqlParameter("@SearchTerm", SearchTerm ?? (object)DBNull.Value), new SqlParameter("@PageSize", PageSize), new SqlParameter("@PageNumber", PageNumber), totalRecordsParam)
                                        .ToList();


            int totalRecordsCount = (int)totalRecordsParam.Value;
            Console.WriteLine(totalRecordsCount);
            var TotalPages = (int)Math.Ceiling((double)totalRecordsCount / PageSize);

            ViewBag.TotalPages = TotalPages;
            ViewBag.SearchTerm = SearchTerm;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;

            return View(PaginatedBlogs);
        }

        // GET: Blogs1/Details/5
        public async Task<IActionResult> Details(int? id)
        {

            Console.WriteLine($"Blog ID received: {id}");

            var records = await _context.Blogs
                .Include(b => b.Genres)
                .Include(b => b.Comments)
                .FirstOrDefaultAsync(b => b.Id == id);
            if (records == null)
            {
                return NotFound();
            }

            return View(records);
        }
        public async Task<IActionResult> AddComment(string CommentText, int BlogId)
        {
            if (string.IsNullOrWhiteSpace(CommentText))
            {
                return BadRequest("Comment cannot be empty");
            }

            int UserID = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);


            var comment = new Blogcomments();
            comment.CreatedAt = DateTime.Now;
            comment.BlogId = BlogId;
            comment.UserID = UserID;
            comment.CommentText = CommentText;

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = BlogId });
        }

        public async Task<IActionResult> OwnBlogs()
        {
            var Id = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            //int Id = (UserId);
            Console.WriteLine("UserID: " + Id);
            var records = await _BlogsRepository.GetAll();
            var blogs = await _context.Blogs
                                   .Include(b => b.Genres)
                                   .Where(b => b.UserId == Id)
                                   .ToListAsync();
            var blogsDic = blogs.ToDictionary(x => x.Id, x => x.Genres);

            return View(blogs);
            //var record = _context.Blogs.Include(u => u.UserId).ToList();
            //return View(records);
        }

        // GET: Blogs1/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Genres = new SelectList(await _GenreRepository.GetAll(), "Id", "Name");
            return View();
        }

        // POST: Blogs1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Blogs blogs, List<int> SelectedGenreIds, IFormFile ImageFile)
        {
            ViewData["Genres"] = new SelectList(await _GenreRepository.GetAll(), "Id", "Name");

            Console.WriteLine(ImageFile);
            if (ImageFile != null && ImageFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await ImageFile.CopyToAsync(ms);
                blogs.Image = ms.ToArray();
            }
            blogs.CreatedAt = DateTime.Now;
            blogs.UpdatedAt = DateTime.Now;

            blogs.Genres = new List<Genre>();
            foreach (var genreId in SelectedGenreIds)
            {
                var genre = await _context.Genres.FindAsync(genreId);
                if (genre != null)
                {
                    _context.Entry(genre).State = EntityState.Unchanged; // Ensures the existing genre is linked
                    blogs.Genres.Add(genre);
                }
            }

            var IdentityClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (IdentityClaim == null)
            {
                Console.WriteLine("The UserId is not being set");
                return View(blogs);
            }
            blogs.UserId = Convert.ToInt32(IdentityClaim);
            try
            {
                _context.Add(blogs);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }

        // GET: Blogs1/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogs = await _context.Blogs.FindAsync(id);
            if (blogs == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id", blogs.UserId);
            return View(blogs);
        }

        // POST: Blogs1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Blogs blogs, IFormFile? ImageFile)
        {
            if (id != blogs.Id)
            {
                return NotFound();
            }
            var existingBlog = _context.Blogs
                                         .Include(b => b.Genres)
                                         .FirstOrDefault(b => b.Id == id);
            if (existingBlog == null)
            {
                return NotFound();
            }
            if (ImageFile != null && ImageFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await ImageFile.CopyToAsync(ms);
                blogs.Image = ms.ToArray();
                existingBlog.Image = blogs.Image;
            }
            else
            {
                blogs.Image = existingBlog.Image;
            }
            if (ModelState.IsValid)
            {
                try
                {
                    blogs.UpdatedAt = DateTime.Now;
                    existingBlog.Title = blogs.Title;
                    existingBlog.Content = blogs.Content;
                    existingBlog.UpdatedAt = blogs.UpdatedAt;
                    //existingBlog.Genres = blogs.gene
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogsExists(blogs.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(blogs);

        }

        // GET: Blogs1/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogs = await _context.Blogs
                .Include(b => b.user)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blogs == null)
            {
                return NotFound();
            }

            return View(blogs);
        }

        // POST: Blogs1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blogs = await _context.Blogs.FindAsync(id);
            if (blogs != null)
            {
                _context.Blogs.Remove(blogs);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogsExists(int id)
        {
            return _context.Blogs.Any(e => e.Id == id);
        }
    }
}
