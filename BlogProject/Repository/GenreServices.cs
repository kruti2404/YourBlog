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
        public Genre GetById(int id)
        {
            var record = _context.Genres.FirstOrDefault(x => x.Id == id);
            return record;
        }

        public async Task Insert(Genre genre)
        {
            await _context.Genres.AddAsync(genre);
        }
        public void Update(Genre genre)
        {
            _context.Genres.Update(genre);
        }
        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
