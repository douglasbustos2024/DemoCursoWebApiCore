using WebAppApiArq.Contract;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebAppApiArq.Data;


namespace WebAppApiArq.Repositories
{

    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public IQueryable<T> GetAll()
        {
            return _dbSet.AsQueryable();
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync(); // Ejecuta la consulta y obtiene los resultados
        }

        public IQueryable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await SaveChangesAsync();
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
            _context.SaveChanges();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await SaveChangesAsync();
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }




        // Método para ejecutar procedimientos almacenados que no devuelven resultados
        public async Task ExecuteStoredProcedureAsync(string procedureName, params SqlParameter[] parameters)
        {
            await _context.Database.ExecuteSqlRawAsync(procedureName, parameters);
        }

        // Método para ejecutar procedimientos almacenados que devuelven resultados
        public async Task<List<T>> ExecuteStoredProcedureWithResultsAsync(string procedureName,
            params SqlParameter[] parameters)
        {                         

            return await _context.Set<T>().FromSqlRaw(procedureName, parameters).ToListAsync();
        }


    }



}
