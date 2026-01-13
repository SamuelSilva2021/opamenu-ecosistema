
namespace Authenticator.API.Core.Domain.Api
{
    public class ExceptionDTO
    {
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? Source { get; set; }
        public string? StackTrace { get; set; }
        public string? TargetSite { get; set; }
        public ExceptionDTO? InnerException { get; set; }
        public List<ExceptionDTO>? InnerExceptions { get; set; }

        /// <summary>
        /// Converte uma Exception em ExceptionDTO (recursivamente).
        /// Limita profundidade para evitar loops/recursões infinitas.
        /// </summary>
        public static ExceptionDTO? FromException(Exception? ex, int maxDepth = 5)
        {
            if (ex is null || maxDepth <= 0) return null;

            var dto = new ExceptionDTO
            {
                Message = ex.Message,
                Type = ex.GetType().FullName ?? ex.GetType().Name,
                Source = ex.Source,
                StackTrace = ex.StackTrace,
                TargetSite = ex.TargetSite is null ? null
                            : $"{ex.TargetSite.DeclaringType?.FullName}.{ex.TargetSite.Name}"
            };

            if (ex is AggregateException agg)
            {
                dto.InnerExceptions = agg.InnerExceptions?
                    .Select(e => FromException(e, maxDepth - 1))
                    .Where(x => x != null)
                    .Select(x => x!)
                    .ToList();
            }
            else if (ex.InnerException != null)
            {
                dto.InnerException = FromException(ex.InnerException, maxDepth - 1);
            }

            return dto;
        }

        public static implicit operator ExceptionDTO?(Exception? ex) => FromException(ex);
    }
}
