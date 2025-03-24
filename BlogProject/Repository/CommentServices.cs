using System.Threading.Tasks;
using BlogProject.Data;
using BlogProject.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BlogProject.Repository
{
    public class CommentServices
    {

        private readonly ProgramDbContext _context;
        public CommentServices(ProgramDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Blogcomments> GetAll()
        {
            return _context.Comments.ToList();
        }

        public async Task Insert(Blogcomments comments)
        {
            await _context.Comments.AddAsync(comments);
        }
        public void Update(Blogcomments comments)
        {
            _context.Comments.Update(comments);
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }

    }
}
