using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BlogProject.Models;
using BlogProject.Data;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using NuGet.ProjectModel;

namespace BlogProject.Controllers;

public class HomeController : Controller
{
    private readonly ProgramDbContext _context;
    public HomeController(ProgramDbContext context)
    {
        _context = context;
    }
    [HttpGet]
    public IActionResult Index(string SearchTerm)
    {
        ViewData["SearchTerm"] = SearchTerm;
        if (SearchTerm != null)
        {
            Console.WriteLine("SearchTerm is " + SearchTerm);
            var blogs = _context.Blogs.Include(b => b.Genres)
                                        .Where(b => b.Title.Contains(SearchTerm)
                                                    || b.Genres.Any(g => g.Name.Contains(SearchTerm)))
                                        .ToList();
            if (blogs != null)
            {
                return View(blogs);
            }
           
        }

        var records = _context.Blogs
           .Include(b => b.Genres);
        Console.WriteLine("Executing the records without the searchterm ");
        return View(records);
    }
    public async Task<IActionResult> Details(int id)
    {
        Console.WriteLine($"Blog ID received: {id}");

        var records = await _context.Blogs
            .Include(b => b.Genres)
            .FirstOrDefaultAsync(b => b.Id == id);
        if (records == null)
        {
            return NotFound();
        }

        return View(records);
    }
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
