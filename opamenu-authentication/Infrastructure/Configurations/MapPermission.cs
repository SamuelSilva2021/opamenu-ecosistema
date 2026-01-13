namespace Authenticator.API.Infrastructure.Configurations
{
    /// <summary>
    /// Atributo personalizado para mapear permissões a módulos e operações específicas.
    /// </summary>
    /// <param name="module"></param>
    /// <param name="operation"></param>
    public class MapPermission(string module, string operation) : Attribute
    {
        /// <summary>
        /// Módulo ao qual a permissão pertence.
        /// </summary>
        public string Module { get; set; } = module;
        /// <summary>
        /// Operação específica dentro do módulo.
        /// </summary>
        public string Operation { get; set; } = operation;
    }
}
