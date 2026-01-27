using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Data.Context.Opamenu;
using OpaMenu.Infrastructure.Shared.Data.Context.MultTenant;
using OpaMenu.Infrastructure.Shared.Data.Context.AccessControl;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;

namespace OpaMenu.Infrastructure.Repositories;

/// <summary>
/// Implementação base do repositório genérico
/// </summary>
/// <typeparam name="T">Entidade que herda de BaseEntity</typeparam>
public abstract class BaseRepository<T>(DbContext context) : IRepository<T> where T : BaseEntity
{
    protected readonly DbContext _context = context;
    protected readonly DbSet<T> _dbSet = context.Set<T>();

    public virtual async Task<IEnumerable<T>> GetAllAsync(Guid tenantId) => 
        await _dbSet.Where(x => x.TenantId == tenantId).ToListAsync();

    public virtual async Task<T?> GetByIdAsync(Guid id, Guid tenantId) => 
        await _dbSet.FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId);

    public virtual async Task<T> AddAsync(T entity)
    {
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        
        return entity;
    }

    public virtual async Task UpdateAsync(T entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public virtual async Task DeleteVirtualAsync(Guid id, Guid tenantId)
    {
        var entity = await GetByIdAsync(id, tenantId);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public virtual async Task<bool> ExistsAsync(Guid id)
    {
        return await _dbSet.AnyAsync(x => x.Id == id);
    }

    public async Task DeleteAsync(T Entity)
    {
        _dbSet.Remove(Entity);
        await _context.SaveChangesAsync();
    }

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate);
    }

    public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }

    public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
    {
        return predicate == null 
            ? await _dbSet.CountAsync() 
            : await _dbSet.CountAsync(predicate);
    }

    public virtual async Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int pageSize)
    {
        return await _dbSet
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> GetPagedAsync(Expression<Func<T, bool>> predicate, int pageNumber, int pageSize)
    {
        return await _dbSet
            .Where(predicate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> GetAllOrderedAsync<TKey>(Expression<Func<T, TKey>> orderBy, bool ascending = true)
    {
        return ascending 
            ? await _dbSet.OrderBy(orderBy).ToListAsync()
            : await _dbSet.OrderByDescending(orderBy).ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> FindOrderedAsync<TKey>(Expression<Func<T, bool>> predicate, Expression<Func<T, TKey>> orderBy, bool ascending = true)
    {
        var query = _dbSet.Where(predicate);
        return ascending 
            ? await query.OrderBy(orderBy).ToListAsync()
            : await query.OrderByDescending(orderBy).ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> GetAllWithIncludesAsync(params Expression<Func<T, object>>[] includes)
    {
        var query = includes.Aggregate(_dbSet.AsQueryable(), (current, include) => current.Include(include));
        return await query.ToListAsync();
    }

    public virtual async Task<T?> GetByIdWithIncludesAsync(Guid id, params Expression<Func<T, object>>[] includes)
    {
        var query = includes.Aggregate(_dbSet.AsQueryable(), (current, include) => current.Include(include));
        return await query.FirstOrDefaultAsync(x => x.Id == id);
    }
    public virtual async Task<IEnumerable<T>?> GetAllByTenantIdWithIncludesAsync(Guid tenantId, params Expression<Func<T, object>>[] includes)
    {
        var query = includes.Aggregate(_dbSet.AsQueryable(), (current, include) => current.Include(include));
        return await query.Where(x => x.TenantId == tenantId).ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> FindWithIncludesAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
    {
        var query = includes.Aggregate(_dbSet.AsQueryable(), (current, include) => current.Include(include));
        return await query.Where(predicate).ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> GetPagedByTenantIdWithIncludesAsync(Guid tenantId, int pageNumber, int pageSize, params Expression<Func<T, object>>[] includes)
    {
        var query = includes.Aggregate(_dbSet.AsQueryable(), (current, include) => current.Include(include));
        return await query
            .Where(x => x.TenantId == tenantId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public virtual async Task<int> CountByTenantIdAsync(Guid tenantId) => 
        await _dbSet.CountAsync(x => x.TenantId == tenantId);

    public virtual async Task AddRangeAsync(IEnumerable<T> entities)
    {
        var entitiesList = entities.ToList();
        var now = DateTime.UtcNow;
        
        foreach (var entity in entitiesList)
        {
            entity.CreatedAt = now;
            entity.UpdatedAt = now;
        }
        
        await _dbSet.AddRangeAsync(entitiesList);
        await _context.SaveChangesAsync();
    }

    public virtual async Task UpdateRangeAsync(IEnumerable<T> entities)
    {
        var entitiesList = entities.ToList();
        var now = DateTime.UtcNow;
        
        foreach (var entity in entitiesList)
        {
            entity.UpdatedAt = now;
        }
        
        _dbSet.UpdateRange(entitiesList);
        await _context.SaveChangesAsync();
    }

    public virtual async Task DeleteRangeAsync(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
        await _context.SaveChangesAsync();
    }

    public virtual async Task<TResult> MaxAsync<TResult>(Expression<Func<T, TResult>> selector)
    {
        return await _dbSet.MaxAsync(selector);
    }

    public virtual async Task<TResult> MinAsync<TResult>(Expression<Func<T, TResult>> selector)
    {
        return await _dbSet.MinAsync(selector);
    }

    public virtual async Task<decimal> SumAsync(Expression<Func<T, decimal>> selector)
    {
        return await _dbSet.SumAsync(selector);
    }

    public virtual async Task<double> AverageAsync(Expression<Func<T, decimal>> selector)
    {
        return (double)await _dbSet.AverageAsync(selector);
    }
}

/// <summary>
/// Repositório específico para OpamenuDbContext
/// </summary>
public class OpamenuRepository<T> : BaseRepository<T> where T : BaseEntity
{
    public OpamenuRepository(OpamenuDbContext context) : base(context)
    {
    }
}

/// <summary>
/// Repositório específico para MultiTenantDbContext
/// </summary>
public class MultiTenantRepository<T> where T : class
{
    protected readonly MultiTenantDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public MultiTenantRepository(MultiTenantDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }
}

/// <summary>
/// Repositório específico para AccessControlDbContext
/// </summary>
public class AccessControlRepository<T> : BaseRepository<T> where T : BaseEntity
{
    public AccessControlRepository(AccessControlDbContext context) : base(context)
    {
    }
}

