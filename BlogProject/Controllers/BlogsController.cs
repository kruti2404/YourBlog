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
        private readonly GenreServices _GenreRepository;
        private readonly CommentServices _CommentRepository;
        private readonly LikeServices _LikesRepository;


        public BlogsController(BlogServices context, ProgramDbContext context2, GenreServices GenreRepo, CommentServices commentRepository, LikeServices likeServices)
        {
            _BlogsRepository = context;
            _GenreRepository = GenreRepo;
            _CommentRepository = commentRepository;
            _LikesRepository = likeServices;
        }

        // GET: Blogs
        public async Task<IActionResult> Index(string SearchTerm, string FilterGenre, int PageSize = 3, int PageNumber = 1)
        {


            IEnumerable<Genre> Genres = _GenreRepository.GetAll();

            List<string> genresName = new List<string>();
            foreach (var genre in Genres)
            {
                genresName.Add(genre.Name);
            }
            ViewBag.Genres = genresName;
            Console.WriteLine("Selected Genre" + FilterGenre);

            var totalRecordsParam = new SqlParameter
            {
                ParameterName = "@TotalRecords",
                DbType = System.Data.DbType.Int32,
                Direction = System.Data.ParameterDirection.Output,
                Size = sizeof(int)
            };

            IEnumerable<BlogsGenreDTO> PaginatedBlogs = _BlogsRepository.GetAll(SearchTerm, FilterGenre, totalRecordsParam, PageSize, PageNumber);

            int totalRecordsCount = (int)totalRecordsParam.Value;
            var TotalPages = (int)Math.Ceiling((double)totalRecordsCount / PageSize);

            ViewBag.TotalPages = TotalPages;
            ViewBag.SearchTerm = SearchTerm;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            ViewBag.FilterGenre = FilterGenre;


            return View(PaginatedBlogs);
        }

        // GET: Blogs1/Details/5
        public async Task<IActionResult> Details(int id)
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

            await _CommentRepository.Insert(comment);
            await _BlogsRepository.Save();

            return RedirectToAction("Details", new { id = BlogId });
        }

        [HttpPost]
        public async Task<IActionResult> Like(int BlogId, int UserId)
        {
            Console.WriteLine("Like Page : with " + BlogId + " And User id : " + UserId);

            var existing = await _LikesRepository.GetByUSerBlogID(UserId, BlogId);

            Console.WriteLine("User Id: " + UserId);
            if (existing == null)
            {
                var like = new Likes
                {
                    BlogId = BlogId,
                    UserId = UserId,
                    IsActive = true,
                    LikedAt = DateTime.Now
                };
                await _LikesRepository.Insert(like);
                await _LikesRepository.Save();
                Console.WriteLine("Added into Database ");
                return Json(new { success = true, liked = true, message = "Liked successfully!" });
            }
            else
            {

                Console.WriteLine("Removed into Database ");
                _LikesRepository.Remove(existing);

                await _LikesRepository.Save();
                return Json(new { success = true, liked = false, message = "Unliked successfully!" });
            }
        }
        public async Task<IActionResult> OwnBlogs()
        {
            var Id = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var blogs = await _BlogsRepository.AllOwnBlogs(Id);

            return View(blogs);

        }

        // GET: Blogs/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Genres = new SelectList(_GenreRepository.GetAll(), "Id", "Name");
            return View();
        }

        // POST: Blogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Blogs blogs, List<int> SelectedGenreIds, IFormFile ImageFile)
        {
            ViewData["Genres"] = new SelectList(_GenreRepository.GetAll(), "Id", "Name");

            if (ImageFile != null && ImageFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await ImageFile.CopyToAsync(ms);
                blogs.Image = ms.ToArray();
            }
            blogs.CreatedAt = DateTime.Now;
            blogs.UpdatedAt = DateTime.Now;

            blogs.Genres = new List<Genre>();
            if (SelectedGenreIds != null && SelectedGenreIds.Any())
            {
                foreach (var genreId in SelectedGenreIds)
                {
                    var genre = await _GenreRepository.GetById(genreId);

                    if (genre != null)
                    {
                        blogs.Genres.Add(genre);
                    }
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
                await _BlogsRepository.Insert(blogs);
                await _BlogsRepository.Save();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }

        // GET: Blogs/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Genres = new SelectList(_GenreRepository.GetAll(), "Id", "Name");

            if (id == null)
            {
                return NotFound();
            }

            var blogs = await _BlogsRepository.GetById(id);
            if (blogs == null)
            {
                return NotFound();
            }
            return View(blogs);
        }

        // POST: Blogs1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, List<int> SelectedGenreIds, Blogs blogs, IFormFile? ImageFile)
        {
            ViewBag.Genres = new SelectList(_GenreRepository.GetAll(), "Id", "Name");

            if (id != blogs.Id)
            {
                return NotFound();
            }
            var existings = await _BlogsRepository.GetById(id);
            if (existings == null)
            {
                return NotFound();
            }
            if (ImageFile != null && ImageFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await ImageFile.CopyToAsync(ms);
                blogs.Image = ms.ToArray();
                existings.Image = blogs.Image;
            }
            else
            {
                blogs.Image = existings.Image;
            }
            if (ModelState.IsValid)
            {
                try
                {
                    existings.Genres.Clear();
                    foreach (var genreId in SelectedGenreIds)
                    {
                        Console.WriteLine("Geners : " + genreId);
                        var genre = await _GenreRepository.GetById(genreId);

                        if (genre != null)
                        {
                            existings.Genres.Add(genre);
                        }
                    }
                    existings.UpdatedAt = DateTime.Now;
                    existings.Title = blogs.Title;
                    existings.Content = blogs.Content;

                    await _BlogsRepository.Save();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (await BlogsExists(blogs.Id))
                    {
                        throw;
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(blogs);

        }

        // GET: Blogs/Delete/5
        public async Task<IActionResult> Delete(int id)
        {

            var blog = await _BlogsRepository.GetById(id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // POST: Blogs1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blogs = await _BlogsRepository.GetById(id);
            if (blogs != null)
            {
                _BlogsRepository.Remove(blogs);
            }

            await _BlogsRepository.Save();
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> BlogsExists(int id)
        {
            var blogs = await _BlogsRepository.GetById(id);
            return blogs == null;
        }
    }
}
