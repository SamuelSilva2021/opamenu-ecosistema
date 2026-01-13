using System.Linq.Expressions;

namespace Authenticator.API.Core.Application.Interfaces
{
    /// <summary>
    /// Repositório base genérico para operaciones CRUD y consultas comunes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBaseRepository<T> where T : class
    {
        /// <summary>
        /// Obtém todas as entidades.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAllAsync();
        /// <summary>
        /// Obtém todas as entidades incluindo propriedades de navegação especificadas por meio de uma função de inclusão.
        /// </summary>
        /// <param name="include"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAllAsync(Func<IQueryable<T>, IQueryable<T>> include);
        /// <summary>
        /// Obtém todas as entidades que correspondem ao filtro especificado.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="include"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter, Func<IQueryable<T>, IQueryable<T>> include);
        /// <summary>
        /// Obtém uma entidade por seu ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T?> GetByIdAsync(Guid id);
        /// <summary>
        /// Obtém uma entidade por seu ID incluindo propriedades de navegação especificadas por meio de uma função de inclusão.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="include"></param>
        /// <returns></returns>
        Task<T?> GetByIdAsync(Guid id, Func<IQueryable<T>, IQueryable<T>> include);
        /// <summary>
        /// Agrega uma nova entidade.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<T> AddAsync(T entity);
        /// <summary>
        /// Atualiza uma entidade existente.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task UpdateAsync(T entity);
        /// <summary>
        /// Elimina uma entidade por seu ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteVirtualAsync(Guid id);
        /// <summary>
        /// Elimina uma entidade.
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        Task DeleteAsync(T Entity);
        /// <summary>
        /// Verifica se uma entidade existe por seu ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> ExistsAsync(Guid id);
        /// <summary>
        /// Encontra entidades com base em um predicado.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// Obtém a primeira entidade que corresponde ao predicado ou o valor padrão se nenhuma for encontrada.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// Verifica se alguma entidade corresponde ao predicado.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// Conta o número de entidades que correspondem ao predicado. Se o predicado for nulo, conta todas as entidades.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
        /// <summary>
        /// Obtém uma página de entidades com base no número da página e no tamanho da página.
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int pageSize);
        /// <summary>
        /// Obtém uma página de entidades que correspondem ao predicado com base no número da página e no tamanho da página.
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetPagedAsync(Expression<Func<T, bool>> predicate, int pageNumber, int pageSize);
        /// <summary>
        /// Obtém uma página de entidades incluindo propriedades de navegação especificadas.
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetPagedWithIncludesAsync(int pageNumber, int pageSize, params Expression<Func<T, object>>[] includes);
        /// <summary>
        /// Obtém uma página de entidades incluindo propriedades de navegação especificadas por meio de uma função de inclusão.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="include"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize, Func<IQueryable<T>, IQueryable<T>> include);
        /// <summary>
        /// Obtém uma página de entidades que correspondem ao predicado e incluem propriedades de navegação especificadas.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="orderBy"></param>
        /// <param name="ascending"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAllOrderedAsync<TKey>(Expression<Func<T, TKey>> orderBy, bool ascending = true);
        /// <summary>
        /// Obtém entidades que correspondem ao predicado e as ordena com base na chave especificada.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="orderBy"></param>
        /// <param name="ascending"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> FindOrderedAsync<TKey>(Expression<Func<T, bool>> predicate, Expression<Func<T, TKey>> orderBy, bool ascending = true);
        /// <summary>
        /// Obtém todas as entidades incluindo propriedades de navegação especificadas.
        /// </summary>
        /// <param name="includes"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAllWithIncludesAsync(params Expression<Func<T, object>>[] includes);
        /// <summary>
        /// Obtém uma entidade por seu ID incluindo propriedades de navegação especificadas.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        Task<T?> GetByIdWithIncludesAsync(Guid id, params Expression<Func<T, object>>[] includes);
        /// <summary>
        /// Encontra entidades que correspondem ao predicado e incluem propriedades de navegação especificadas.
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> FindWithIncludesAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        /// <summary>
        /// Adiciona várias entidades de uma vez.
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task AddRangeAsync(IEnumerable<T> entities);
        /// <summary>
        /// Atualiza várias entidades de uma vez.
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task UpdateRangeAsync(IEnumerable<T> entities);
        /// <summary>
        /// Elimina várias entidades de uma vez.
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task DeleteRangeAsync(IEnumerable<T> entities);
        /// <summary>
        /// Obtém o valor máximo de uma propriedade especificada.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        Task<TResult> MaxAsync<TResult>(Expression<Func<T, TResult>> selector);
        /// <summary>
        /// Obtém o valor mínimo de uma propriedade especificada.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        Task<TResult> MinAsync<TResult>(Expression<Func<T, TResult>> selector);
        /// <summary>
        /// Calcula a soma de uma propriedade decimal especificada.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        Task<decimal> SumAsync(Expression<Func<T, decimal>> selector);
        /// <summary>
        /// Calcula a média de uma propriedade decimal especificada.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        Task<double> AverageAsync(Expression<Func<T, decimal>> selector);
    }
}
