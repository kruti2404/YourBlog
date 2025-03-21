using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BlogProject.Models;
using BlogProject.Data;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using NuGet.ProjectModel;
using System.Drawing.Printing;
using Microsoft.Data.SqlClient;

namespace BlogProject.Controllers;

public class HomeController : Controller
{
    private readonly ProgramDbContext _context;
    public HomeController(ProgramDbContext context)
    {
        _context = context;
    }
    [HttpGet]
    public IActionResult Index(string SearchTerm, int PageSize = 3, int PageNumber = 1)
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

        //Console.WriteLine(PaginatedBlogs)
        int totalRecordsCount = (int)totalRecordsParam.Value;
        //Console.WriteLine(totalRecordsCount);
        var TotalPages = (int)Math.Ceiling((double)totalRecordsCount / PageSize);

        ViewBag.TotalPages = TotalPages;
        ViewBag.SearchTerm = SearchTerm;
        ViewBag.PageSize = PageSize;
        ViewBag.PageNumber = PageNumber;

        return View(PaginatedBlogs);
    }
    public async Task<IActionResult> Details(int id)
    {
        //Console.WriteLine($"Blog ID received: {id}");

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
