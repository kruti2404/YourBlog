using BlogProject.Data;
using BlogProject.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogProject.Repository
{
    public class LikeServices
    {
        private readonly ProgramDbContext _context;
        public LikeServices(ProgramDbContext context)
        {
            _context = context;
        }


        public async Task<Likes> GetById(int id)
        {
            return await _context.Likes.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task Insert(Likes likes)
        {
            await _context.Likes.AddAsync(likes);
        }
        
        public void Remove(Likes likes)
        {
            _context.Likes.Update(likes);
        }


    }
}
