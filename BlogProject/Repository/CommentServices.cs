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
        public async Task Insert(Blogcomments comments)
        {
            await _context.Comments.AddAsync(comments);
        }

        public async Task<int> GetCommentsCount(int BlogId)
        {
            return await _context.Comments
                .Where(c => c.BlogId == BlogId).CountAsync();
        }

        public void RemoveRange(IEnumerable<Blogcomments> comments)
        {
            _context.RemoveRange(comments);

        }
        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
