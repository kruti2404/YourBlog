
using BlogProject.Data;
using Microsoft.EntityFrameworkCore;

namespace BlogProject.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ProgramDbContext _context;
        private readonly DbSet<T> table;

        public GenericRepository(ProgramDbContext context)
        {
            _context = context;
            table = context.Set<T>();
        }


        public async Task<IEnumerable<T>> GetAll()
        {
            return await table.ToListAsync();
        }

        public async Task<T> GetById(object id)
        {
            return await table.FindAsync(id);

        }

        //public async Task<T> GetByUserName(string str)
        //{
        //    var record = await table.FirstOrDefaultAsync<T>(x => x.)
        //}

        public async Task Insert(T entity)
        {
            await table.AddAsync(entity);
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }

        public Task Update(T entity)
        {
            table.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            return Task.CompletedTask;
        }

        public async Task Delete(object id)
        {
            var entity = await table.FindAsync(id);
            if (entity != null)
            {
                table.Remove(entity);
            }
        }

       
    }
}
