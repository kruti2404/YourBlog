using BlogProject.Data;
using BlogProject.Models;

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





    }
}
