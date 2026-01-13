using OpaMenu.Infrastructure.Shared.Entities;
using System.Linq.Expressions;

namespace OpaMenu.Domain.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    // MÃ©todos bÃ¡sicos existentes
    Task<IEnumerable<T>> GetAllAsync(Guid tenantId);
    Task<T?> GetByIdAsync(int id, Guid tenantId);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteVirtualAsync(int id, Guid tenantId);
    Task DeleteAsync(T Entity);
    Task<bool> ExistsAsync(int id);

    // MÃ©todos com predicates e consultas complexas
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
    
    // MÃ©todos com paginaÃ§Ã£o
    Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int pageSize);
    Task<IEnumerable<T>> GetPagedAsync(Expression<Func<T, bool>> predicate, int pageNumber, int pageSize);
    
    // MÃ©todos com ordenaÃ§Ã£o
    Task<IEnumerable<T>> GetAllOrderedAsync<TKey>(Expression<Func<T, TKey>> orderBy, bool ascending = true);
    Task<IEnumerable<T>> FindOrderedAsync<TKey>(Expression<Func<T, bool>> predicate, Expression<Func<T, TKey>> orderBy, bool ascending = true);
    
    // MÃ©todos com includes para relacionamentos
    Task<IEnumerable<T>> GetAllWithIncludesAsync(params Expression<Func<T, object>>[] includes);
    Task<T?> GetByIdWithIncludesAsync(int id, params Expression<Func<T, object>>[] includes);
    Task<IEnumerable<T>?> GetAllByTenantIdWithIncludesAsync(Guid tenantId, params Expression<Func<T, object>>[] includes);
    Task<IEnumerable<T>> FindWithIncludesAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
    Task<IEnumerable<T>> GetPagedByTenantIdWithIncludesAsync(Guid tenantId, int pageNumber, int pageSize, params Expression<Func<T, object>>[] includes);
    Task<int> CountByTenantIdAsync(Guid tenantId);

    // OperaÃ§Ãµes em lote
    Task AddRangeAsync(IEnumerable<T> entities);
    Task UpdateRangeAsync(IEnumerable<T> entities);
    Task DeleteRangeAsync(IEnumerable<T> entities);
    
    // MÃ©todos de agregaÃ§Ã£o
    Task<TResult> MaxAsync<TResult>(Expression<Func<T, TResult>> selector);
    Task<TResult> MinAsync<TResult>(Expression<Func<T, TResult>> selector);
    Task<decimal> SumAsync(Expression<Func<T, decimal>> selector);
    Task<double> AverageAsync(Expression<Func<T, decimal>> selector);
}


