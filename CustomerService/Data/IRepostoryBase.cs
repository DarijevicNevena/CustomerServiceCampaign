using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CustomerService.Data
{
    public interface IRepository<T>
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
        Task<IEnumerable<T>> SearchAsync(Func<T, bool> predicate);
    }
}
