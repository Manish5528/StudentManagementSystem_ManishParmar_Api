using System.Linq.Expressions;

namespace StudentManagementSystem.Application.Contract;


public interface IGenericRepository<T> where T : class
{
    Task<T?> GetByIdAsync(object id);
    Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IQueryable<T>>? include = null);

    Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        Expression<Func<T, bool>>? predicate = null,
        int page = 1,
        int pageSize = 10,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IQueryable<T>>? include = null);

    Task AddAsync(T entity);

    Task UpdateAsync(T entity);

    Task RemoveAsync(T entity);

    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);

    Task<IEnumerable<T>> GetAllAsync();
}

