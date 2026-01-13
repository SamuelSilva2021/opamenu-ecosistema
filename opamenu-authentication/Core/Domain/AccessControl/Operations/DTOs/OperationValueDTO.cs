namespace Authenticator.API.Core.Domain.AccessControl.Operations.DTOs
{
    /// <summary>
    /// DTO para resposta de valor de operação
    /// </summary>
    public class OperationValueDTO
    {
        /// <summary>
        /// Valor da operação (ex: 'CREATE', 'SELECT', 'UPDATE','DELETE')
        /// </summary>
        public string Value { get; set; } = string.Empty;
    }
}
