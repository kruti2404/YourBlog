using System.Threading.Tasks;
using BlogProject.Data;
using BlogProject.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogProject.Repository
{
    public class GenreServices
    {
        private readonly ProgramDbContext _context;
        public GenreServices(ProgramDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Genre> GetAll()
        {
            return _context.Genres.ToList();
        }

        public async Task<Genre> GetById(int id)
        {
            return await _context.Genres.FirstOrDefaultAsync(g => g.Id == id);

        }



    }
}
