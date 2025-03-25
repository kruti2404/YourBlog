using System.Security.Claims;
using System.Threading.Tasks;
using BlogProject.Data;
using BlogProject.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BlogProject.Repository
{
    public class BlogServices
    {
        private readonly ProgramDbContext _context;
        public BlogServices(ProgramDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Blogs> GetAll()
        {
            return _context.Blogs.ToList();
        }
        public IEnumerable<BlogsGenreDTO> GetAll(string SearchTerm, SqlParameter totalRecordsParam, int PageSize = 3, int PageNumber = 1)
        {
            var PaginatedBlogs = _context.Database
                                                   .SqlQueryRaw<BlogsGenreDTO>("Exec SP_PaginatedSeachResult @SearchTerm, @PageSize, @PageNumber, @TotalRecords OUTPUT",
                                                   new SqlParameter("@SearchTerm", SearchTerm ?? (object)DBNull.Value), new SqlParameter("@PageSize", PageSize), new SqlParameter("@PageNumber", PageNumber), totalRecordsParam)
                                                   .ToList();

            return PaginatedBlogs ?? new List<BlogsGenreDTO>();
        }
        public async Task<IEnumerable<Blogs>> AllOwnBlogs(int Id)
        {

            return await _context.Blogs
                                  .Include(b => b.Genres)
                                  .Where(b => b.UserId == Id)
                                  .ToListAsync();

        }
        public Blogs GetById(int id)
        {
            var record = _context.Blogs.FirstOrDefault(x => x.Id == id);
            return record;
        }

        public async Task Insert(Blogs blogs)
        {
            await _context.Blogs.AddAsync(blogs);
        }
        public void Update(Blogs blogs)
        {
            _context.Blogs.Update(blogs);
        }
        public void Remove(Blogs blogs)
        {
            _context.Blogs.Remove(blogs);
        }
        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Blogs> Details(int id)
        {
            var records = await _context.Blogs
                .Include(b => b.Genres)
                .Include(b => b.Comments)
                .Include(b => b.Likes)
                .FirstOrDefaultAsync(b => b.Id == id);
            return records;
        }

    }
}
