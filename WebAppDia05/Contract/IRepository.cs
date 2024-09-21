using System.Linq.Expressions;

namespace WebAppApiArq.Contract
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> GetAll();
        Task<List<T>> GetAllAsync(); // Agregado para obtener todos los registros de manera asincrónica

        IQueryable<T> Find(Expression<Func<T, bool>> predicate);
        Task<T> GetByIdAsync(int id);
        Task AddAsync(T entity);
        void Update(T entity);
        Task DeleteAsync(int id);
        Task SaveChangesAsync();

    }
}
