using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace CustomerService.Data.Base
{
    public interface IRepository<T>
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
        Task<IEnumerable<T>> SearchAsync(Func<T, bool> predicate);
        Task<List<T>> SearchExtendedAsync(Expression<Func<T, bool>> predicate,Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);
    }
}
