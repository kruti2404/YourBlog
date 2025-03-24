using System.Threading.Tasks;
using BlogProject.Data;
using BlogProject.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogProject.Repository
{
    public class UserServices
    {
        public readonly ProgramDbContext _context;
        public UserServices(ProgramDbContext context)
        {
            _context = context;
        }

        public IEnumerable<User> GetAll()
        {
            return _context.User.ToList();
        }
        public async Task<User> GetById(int id)
        {
            var record = await _context.User.FirstOrDefaultAsync(x => x.Id == id);
            return record;
        }
        public User GetByUserName(string UserName)
        {
            return _context.User.FirstOrDefault(x => x.UserName == UserName);

        } 
        public User GetByUserEmail(string Email)
        {
            return _context.User.FirstOrDefault(x => x.Email == Email);

        }
        public async Task Insert(User user)
        {
            await _context.User.AddAsync(user);
        }
        public void Update(User user)
        {
            _context.User.Update(user);
        }
        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }

    }
}
