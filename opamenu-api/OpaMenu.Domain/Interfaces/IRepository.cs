using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;
using System.Linq.Expressions;

namespace OpaMenu.Domain.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    //Métodos básicos de CRUD
    Task<IEnumerable<T>> GetAllAsync(Guid tenantId);
    Task<T?> GetByIdAsync(Guid id, Guid tenantId);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteVirtualAsync(Guid id, Guid tenantId);
    Task DeleteAsync(T Entity);
    Task<bool> ExistsAsync(Guid id);

    // Métodos com predicates e consultas complexas
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);

    // Métodos com paginação
    Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int pageSize);
    Task<IEnumerable<T>> GetPagedAsync(Expression<Func<T, bool>> predicate, int pageNumber, int pageSize);

    // Métodos com ordenação
    Task<IEnumerable<T>> GetAllOrderedAsync<TKey>(Expression<Func<T, TKey>> orderBy, bool ascending = true);
    Task<IEnumerable<T>> FindOrderedAsync<TKey>(Expression<Func<T, bool>> predicate, Expression<Func<T, TKey>> orderBy, bool ascending = true);

    // Métodos com includes para relacionamentos
    Task<IEnumerable<T>> GetAllWithIncludesAsync(params Expression<Func<T, object>>[] includes);
    Task<T?> GetByIdWithIncludesAsync(Guid id, params Expression<Func<T, object>>[] includes);
    Task<IEnumerable<T>?> GetAllByTenantIdWithIncludesAsync(Guid tenantId, params Expression<Func<T, object>>[] includes);
    Task<IEnumerable<T>> FindWithIncludesAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
    Task<IEnumerable<T>> GetPagedByTenantIdWithIncludesAsync(Guid tenantId, int pageNumber, int pageSize, params Expression<Func<T, object>>[] includes);
    Task<int> CountByTenantIdAsync(Guid tenantId);

    // operações em lote
    Task AddRangeAsync(IEnumerable<T> entities);
    Task UpdateRangeAsync(IEnumerable<T> entities);
    Task DeleteRangeAsync(IEnumerable<T> entities);

    // Métodos de agregação
    Task<TResult> MaxAsync<TResult>(Expression<Func<T, TResult>> selector);
    Task<TResult> MinAsync<TResult>(Expression<Func<T, TResult>> selector);
    Task<decimal> SumAsync(Expression<Func<T, decimal>> selector);
    Task<double> AverageAsync(Expression<Func<T, decimal>> selector);
}


