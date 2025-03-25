using System.Threading.Tasks;
using BlogProject.Data;
using BlogProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace BlogProject.Repository
{
    public class LikeServices
    {
        private readonly ProgramDbContext _context;
        public LikeServices(ProgramDbContext context)
        {
            _context = context;
        }

        public async Task<Likes> GetByUSerBlogID(int UserId, int BlogId)
        {
            return await _context.Likes
                                .AsTracking()
                                .FirstOrDefaultAsync(l => l.UserId == UserId && l.BlogId == BlogId);
        }

        public async Task Insert(Likes likes)
        {
            await _context.Likes.AddAsync(likes);
        }
        public async Task<int> GetLikeCount(int BlogId)
        {
            return await _context.Likes
                .Where(l => l.BlogId == BlogId).CountAsync();
        }
        public void Remove(Likes likes)
        {
            _context.Likes.Remove(likes);
        }
        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }

    }
}
