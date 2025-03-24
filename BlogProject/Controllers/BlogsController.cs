using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BlogProject.Models;
using Microsoft.AspNetCore.Authorization;
using BlogProject.Repository;
using System.Security.Claims;
using BlogProject.Data;
using Microsoft.Data.SqlClient;

namespace BlogsProject.Controllers
{
    [Authorize(Roles = "User")]
    public class BlogsController : Controller
    {
        private readonly BlogServices _BlogsRepository;
        private readonly ProgramDbContext _context;
        private readonly GenreServices _GenreRepository;


        public BlogsController(BlogServices context, ProgramDbContext context2, GenreServices GenreRepo)
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

            IEnumerable<BlogsGenreDTO> PaginatedBlogs = _BlogsRepository.GetAll(SearchTerm, totalRecordsParam, PageSize, PageNumber);

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
        public async Task<IActionResult> Details(int id)
        {

            Console.WriteLine($"Blog ID received: {id}");

            Blogs records = await _BlogsRepository.Details(id);
            if (records == null)
            {
                return NotFound();
            }
            var UserId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var isLiked = records.Likes.Any(l => l.UserId == UserId);

            ViewBag.IsLiked = isLiked;

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
            await _BlogsRepository.Save();

            return RedirectToAction("Details", new { id = BlogId });
        }

        [HttpPost]
        public async Task<IActionResult> Like(int BlogId, int UserId)
        {
            Console.WriteLine("Like Page : with " + BlogId + " And User id : " + UserId);

            var existing = await _context.Likes.FirstOrDefaultAsync(l => l.UserId == UserId && l.BlogId == BlogId);

            if (existing == null)
            {
                var like = new Likes
                {
                    BlogId = BlogId,
                    UserId = UserId,
                    IsActive = true,
                    LikedAt = DateTime.Now
                };
                await _context.Likes.AddAsync(like);
                await _BlogsRepository.Save();
                return Json(new { success = true, liked = true, message = "Liked successfully!" });
            }
            else
            {
                _context.Likes.Remove(existing);
                await _BlogsRepository.Save();
                return Json(new { success = true, liked = false, message = "Unliked successfully!" });
            }
        }
        public async Task<IActionResult> OwnBlogs()
        {
            var Id = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);

            Console.WriteLine("UserID: " + Id);
            var records = _BlogsRepository.GetAll();
            var blogs = await _context.Blogs
                                   .Include(b => b.Genres)
                                   .Where(b => b.UserId == Id)
                                   .ToListAsync();
            var blogsDic = blogs.ToDictionary(x => x.Id, x => x.Genres);

            return View(blogs);

        }

        // GET: Blogs1/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Genres = new SelectList(_GenreRepository.GetAll(), "Id", "Name");
            return View();
        }

        // POST: Blogs1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Blogs blogs, List<int> SelectedGenreIds, IFormFile ImageFile)
        {
            ViewData["Genres"] = new SelectList(_GenreRepository.GetAll(), "Id", "Name");

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
                var genre = _GenreRepository.GetById(genreId);
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
        public IActionResult Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogs = _BlogsRepository.GetById(id);
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
                    await _BlogsRepository.Save();
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
