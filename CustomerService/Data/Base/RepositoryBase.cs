using Microsoft.EntityFrameworkCore;

namespace CustomerService.Data.Base
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly CustomerServiceDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(CustomerServiceDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>(); // Initialize _dbSet
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
            {
                throw new InvalidOperationException($"Entity with id {id} not found.");
            }

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> SearchAsync(Func<T, bool> predicate)
        {
            return await Task.Run(() => _dbSet.Where(predicate).ToList());
        }
    }
}