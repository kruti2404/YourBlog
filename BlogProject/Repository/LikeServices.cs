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

        public IEnumerable<Likes> GetAll()
        {
            return _context.Likes.ToList();
        }

        public async Task<Likes> GetById(int id)
        {
            return await _context.Likes.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task Insert(Likes likes)
        {
            await _context.Likes.AddAsync(likes);
        }
        public void Update(Likes likes)
        {
            _context.Likes.Remove(likes);
        }
        public void Remove(Likes likes)
        {
            _context.Likes.Update(likes);
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }

    }
}
