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
using BlogProject.Repository;

namespace BlogProject.Controllers;

public class HomeController : Controller
{
    private readonly BlogServices _blogServices;
    private readonly GenreServices _genreServices;
    public HomeController(BlogServices blogServices, GenreServices genreServices)
    {
        _blogServices = blogServices;
        _genreServices = genreServices;
    }
    [HttpGet]
    public IActionResult Index(string SearchTerm, string FilterGenre, int PageSize = 3, int PageNumber = 1)
    {



        IEnumerable<Genre> Genres = _genreServices.GetAll();

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

        var PaginatedBlogs = _blogServices.GetAll(SearchTerm, FilterGenre, totalRecordsParam, PageSize, PageNumber);

        int totalRecordsCount = (int)totalRecordsParam.Value;
        var TotalPages = (int)Math.Ceiling((double)totalRecordsCount / PageSize);

        ViewBag.TotalPages = TotalPages;
        ViewBag.SearchTerm = SearchTerm;
        ViewBag.PageSize = PageSize;
        ViewBag.PageNumber = PageNumber;
        ViewBag.FilterGenre = FilterGenre;



        return View(PaginatedBlogs);
    }
    public IActionResult Details(int id)
    {


        var records = _blogServices.GetById(id);
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
