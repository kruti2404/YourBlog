namespace BlogProject.Repository
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(object id);
        Task Insert(T entity);
        Task Update(T entity);
        Task Delete(object id);
        Task Save();
    }
}
