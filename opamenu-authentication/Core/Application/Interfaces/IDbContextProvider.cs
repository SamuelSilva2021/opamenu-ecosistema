using Microsoft.EntityFrameworkCore;

namespace Authenticator.API.Core.Application.Interfaces;

/// <summary>
/// Interface para prover o contexto de banco de dados correto baseado no tipo da entidade
/// </summary>
public interface IDbContextProvider
{
    /// <summary>
    /// Obtém o contexto de banco de dados apropriado para o tipo de entidade especificado
    /// </summary>
    /// <typeparam name="T">Tipo da entidade</typeparam>
    /// <returns>Contexto de banco de dados apropriado</returns>
    DbContext GetContext<T>() where T : class;

    /// <summary>
    /// Obtém o DbSet apropriado para o tipo de entidade especificado
    /// </summary>
    /// <typeparam name="T">Tipo da entidade</typeparam>
    /// <returns>DbSet da entidade</returns>
    DbSet<T> GetDbSet<T>() where T : class;
}