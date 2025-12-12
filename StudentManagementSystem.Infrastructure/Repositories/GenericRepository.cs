using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Application.Contract;
using StudentManagementSystem.Infrastructure.Data;
using System.Linq.Expressions;

namespace StudentManagementSystem.Infrastructure.Repositories
{ 
    public class GenericRepository<T>(ApplicationDbContext context) : IGenericRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
        protected readonly DbSet<T> _set = context.Set<T>();

        public virtual async Task AddAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            await _set.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task UpdateAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            _set.Update(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task RemoveAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            _set.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task<T?> GetByIdAsync(object id)
        {
            if (id == null) return null;
            // FindAsync returns ValueTask<EntityEntry<T>>; awaiting gives entity
            var found = await _set.FindAsync(id);
            return found;
        }

        public virtual async Task<T?> FirstOrDefaultAsync(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            IQueryable<T> query = _set;

            if (include != null) query = include(query);
            if (predicate != null) query = query.Where(predicate);
            if (orderBy != null) query = orderBy(query);

            return await query.AsNoTracking().FirstOrDefaultAsync();
        }

        public virtual async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
            Expression<Func<T, bool>>? predicate = null,
            int page = 1,
            int pageSize = 10,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            IQueryable<T> query = _set;

            if (include != null) query = include(query);
            if (predicate != null) query = query.Where(predicate);

            var total = await query.CountAsync();

            if (orderBy != null) query = orderBy(query);

            var items = await query.Skip((page - 1) * pageSize)
                                   .Take(pageSize)
                                   .AsNoTracking()
                                   .ToListAsync();

            return (items, total);
        }

        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return await _set.AnyAsync(predicate);
        }
    }
}
