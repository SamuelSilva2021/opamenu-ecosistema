using Authenticator.API.Core.Application.Interfaces;
using Authenticator.API.Infrastructure.Providers;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Authenticator.API.Infrastructure.Repositories
{
    /// <summary>
    /// Implementação genérica do repositório usando Entity Framework Core
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly IDbContextProvider _dbContextProvider;
        private readonly DbContext _context;
        private readonly DbSet<T> _dbSet;

        public BaseRepository(IDbContextProvider dbContextProvider)
        {
            _dbContextProvider = dbContextProvider;
            _context = _dbContextProvider.GetContext<T>();
            _dbSet = _dbContextProvider.GetDbSet<T>();
        }

        /// <summary>
        /// Adiciona uma nova entidade ao banco de dados
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<T> AddAsync(T entity)
        {            
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Adiciona uma coleção de entidades ao banco de dados
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public virtual async Task AddRangeAsync(IEnumerable<T> entities)
        {
            var entitiesList = entities.ToList();
            
            await _dbSet.AddRangeAsync(entitiesList);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Verifica se alguma entidade existe que satisfaça a condição especificada
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate) => 
            await _dbSet.AnyAsync(predicate);

        /// <summary>
        /// Calcula a média de uma propriedade decimal especificada
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public virtual async Task<double> AverageAsync(Expression<Func<T, decimal>> selector) => 
            (double)await _dbSet.AverageAsync(selector);

        /// <summary>
        /// Conta o número de entidades que satisfazem a condição especificada, ou todas se nenhuma condição for fornecida
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null) => 
            predicate == null 
                ? await _dbSet.CountAsync() 
                : await _dbSet.CountAsync(predicate);

        /// <summary>
        /// Remove uma entidade do banco de dados
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Remove uma coleção de entidades do banco de dados
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public virtual async Task DeleteRangeAsync(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Realiza uma exclusão virtual definindo a propriedade DeletedAt como a data atual, se a propriedade existir na entidade.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task DeleteVirtualAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                var deletedAtProperty = typeof(T).GetProperty("DeletedAt");
                if (deletedAtProperty != null)
                {
                    deletedAtProperty.SetValue(entity, DateTime.UtcNow);
                    await UpdateAsync(entity);
                }
                else
                {
                    await DeleteAsync(entity);
                }
            }
        }

        /// <summary>
        /// Verifica se uma entidade com o ID especificado existe
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public virtual async Task<bool> ExistsAsync(Guid id)
        {
            var idProperty = typeof(T).GetProperty("Id") ?? throw new InvalidOperationException($"Entity {typeof(T).Name} does not have an Id property");

            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, idProperty);
            var constant = Expression.Constant(id);
            var equal = Expression.Equal(property, constant);
            var lambda = Expression.Lambda<Func<T, bool>>(equal, parameter);

            return await _dbSet.AnyAsync(lambda);
        }

        /// <summary>
        /// Encontra entidades que satisfaçam a condição especificada
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) =>
            await _dbSet.Where(predicate).ToListAsync();

        /// <summary>
        /// Encontra entidades que satisfaçam a condição especificada e as ordena com base na chave fornecida
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="orderBy"></param>
        /// <param name="ascending"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> FindOrderedAsync<TKey>(Expression<Func<T, bool>> predicate, Expression<Func<T, TKey>> orderBy, bool ascending = true)
        {
            var query = _dbSet.Where(predicate);
            return ascending 
                ? await query.OrderBy(orderBy).ToListAsync()
                : await query.OrderByDescending(orderBy).ToListAsync();
        }

        /// <summary>
        /// Encontra entidades que satisfaçam a condição especificada e inclui propriedades de navegação relacionadas
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> FindWithIncludesAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            var query = _dbSet.AsQueryable();
            
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            
            return await query.Where(predicate).ToListAsync();
        }

        /// <summary>
        /// Obtém a primeira entidade que satisfaça a condição especificada, ou null se nenhuma for encontrada
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate) =>
            await _dbSet.FirstOrDefaultAsync(predicate);

        /// <summary>
        /// Obtém todas as entidades do banco de dados
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> GetAllAsync() => 
            await _dbSet.ToListAsync();

        /// <summary>
        /// Obtém todas as entidades do banco de dados com includes customizados usando uma função de configuração
        /// </summary>
        /// <param name="include"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> GetAllAsync(Func<IQueryable<T>, IQueryable<T>> include)
        {
            var query = include(_dbSet.AsQueryable());
            return await query.ToListAsync();
        }
        /// <summary>
        /// Obtém todas as entidades que satisfazem o filtro especificado
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter)
        {
            return await _dbSet
                .Where(filter)
                .ToListAsync();
        }
        /// <summary>
        /// Obtém todas as entidades que satisfazem o filtro especificado com includes customizados
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="include"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter,Func<IQueryable<T>, IQueryable<T>> include)
        {
            var query = include(_dbSet.AsQueryable());

            return await query
                .Where(filter)
                .ToListAsync();
        }

        /// <summary>
        /// Obtém todas as entidades do banco de dados ordenadas pela chave especificada
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="orderBy"></param>
        /// <param name="ascending"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> GetAllOrderedAsync<TKey>(Expression<Func<T, TKey>> orderBy, bool ascending = true)
        {
            return ascending 
                ? await _dbSet.OrderBy(orderBy).ToListAsync()
                : await _dbSet.OrderByDescending(orderBy).ToListAsync();
        }

        /// <summary>
        /// Obtém todas as entidades do banco de dados incluindo propriedades de navegação relacionadas
        /// </summary>
        /// <param name="includes"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> GetAllWithIncludesAsync(params Expression<Func<T, object>>[] includes)
        {
            var query = _dbSet.AsQueryable();
            
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            
            return await query.ToListAsync();
        }

        /// <summary>
        /// Obtém uma entidade pelo seu ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<T?> GetByIdAsync(Guid id)
        {
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null)
                throw new InvalidOperationException($"Entity {typeof(T).Name} does not have an Id property");

            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, idProperty);
            var constant = Expression.Constant(id);
            var equal = Expression.Equal(property, constant);
            var lambda = Expression.Lambda<Func<T, bool>>(equal, parameter);

            return await _dbSet.FirstOrDefaultAsync(lambda);
        }

        /// <summary>
        /// Obtém uma entidade pelo seu ID incluindo propriedades de navegação relacionadas
        /// </summary>
        /// <param name="id"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<T?> GetByIdWithIncludesAsync(Guid id, params Expression<Func<T, object>>[] includes)
        {
            var query = includes.Aggregate(_dbSet.AsQueryable(), (current, include) => current.Include(include));
            
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null)
                throw new InvalidOperationException($"Entity {typeof(T).Name} does not have an Id property");

            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, idProperty);
            var constant = Expression.Constant(id);
            var equal = Expression.Equal(property, constant);
            var lambda = Expression.Lambda<Func<T, bool>>(equal, parameter);

            return await query.FirstOrDefaultAsync(lambda);
        }

        /// <summary>
        /// Obtém uma entidade pelo seu ID com includes customizados usando uma função de configuração
        /// </summary>
        /// <param name="id"></param>
        /// <param name="include"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<T?> GetByIdAsync(Guid id, Func<IQueryable<T>, IQueryable<T>> include)
        {
            var query = include(_dbSet.AsQueryable());

            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null)
                throw new InvalidOperationException($"Entity {typeof(T).Name} does not have an Id property");

            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, idProperty);
            var constant = Expression.Constant(id);
            var equal = Expression.Equal(property, constant);
            var lambda = Expression.Lambda<Func<T, bool>>(equal, parameter);

            return await query.FirstOrDefaultAsync(lambda);
        }

        /// <summary>
        /// Obtém uma página de entidades com base no número da página e no tamanho da página especificados
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int pageSize)
        {
            return await _dbSet
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        /// <summary>
        /// Obtém uma página de entidades que satisfaçam a condição especificada, com base no número da página e no tamanho da página
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> GetPagedAsync(Expression<Func<T, bool>> predicate, int pageNumber, int pageSize)
        {
            return await _dbSet
                .Where(predicate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        /// <summary>
        /// Obtém uma página de entidades incluindo propriedades de navegação relacionadas, com base no número da página e no tamanho da página especificados
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> GetPagedWithIncludesAsync(int pageNumber, int pageSize, params Expression<Func<T, object>>[] includes)
        {
            var query = includes.Aggregate(_dbSet.AsQueryable(), (current, include) => current.Include(include));

            return await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        /// <summary>
        /// Obtém uma página de entidades com includes customizados usando uma função de configuração
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="include"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize, Func<IQueryable<T>, IQueryable<T>> include)
        {
            var query = include(_dbSet.AsQueryable());

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        /// <summary>
        /// Calcula o valor máximo de uma propriedade especificada
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        public async Task<TResult> MaxAsync<TResult>(Expression<Func<T, TResult>> selector) =>
            await _dbSet.MaxAsync(selector);

        /// <summary>
        /// Calcula o valor mínimo de uma propriedade especificada
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        public async Task<TResult> MinAsync<TResult>(Expression<Func<T, TResult>> selector) =>
            await _dbSet.MinAsync(selector);

        /// <summary>
        /// Calcula a soma de uma propriedade decimal especificada
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public async Task<decimal> SumAsync(Expression<Func<T, decimal>> selector) => 
            await _dbSet.SumAsync(selector);

        /// <summary>
        /// Atualiza uma entidade existente no banco de dados
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task UpdateAsync(T entity)
        {
            // Evita conflito de rastreamento quando outra instância com o mesmo Id já está sendo rastreada pelo DbContext
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null)
                throw new InvalidOperationException($"Entity {typeof(T).Name} does not have an Id property");

            var idValue = idProperty.GetValue(entity);

            // Procura no ChangeTracker uma entidade local com o mesmo Id
            var localTracked = _dbSet.Local.FirstOrDefault(e =>
            {
                var localId = idProperty.GetValue(e);
                return localId != null && localId.Equals(idValue);
            });

            // Se existir outra instância rastreada diferente da atual, desprende-a para evitar conflito
            if (localTracked != null && !ReferenceEquals(localTracked, entity))
            {
                _context.Entry(localTracked).State = EntityState.Detached;
            }

            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Atualiza uma coleção de entidades existentes no banco de dados
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public async Task UpdateRangeAsync(IEnumerable<T> entities)
        {
            var entitiesList = entities.ToList();
            
            _dbSet.UpdateRange(entitiesList);
            await _context.SaveChangesAsync();
        }
    }
}
