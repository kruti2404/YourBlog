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
                      

    }
}
