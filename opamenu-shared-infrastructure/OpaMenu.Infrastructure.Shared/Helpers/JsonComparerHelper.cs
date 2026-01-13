using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpaMenu.Infrastructure.Shared.Helpers
{
    /// <summary>
    /// Helper para criar ValueComparers para propriedades JSON no Entity Framework Core
    /// </summary>
    public static class JsonComparerHelper
    {
        /// <summary>
        /// Cria um ValueComparer para Dictionary<string, object> serializado como JSON
        /// </summary>
        public static ValueComparer<Dictionary<string, object>> GetDictionaryComparer()
        {
            return new ValueComparer<Dictionary<string, object>>(
                (c1, c2) => c1 == null && c2 == null ||
                            (c1 != null && c2 != null &&
                            JsonSerializer.Serialize(c1, (JsonSerializerOptions?)null) == JsonSerializer.Serialize(c2, (JsonSerializerOptions?)null)),

                c => c == null ? 0 : c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),

                c => c == null ? new Dictionary<string, object>() :
                     JsonSerializer.Deserialize<Dictionary<string, object>>(
                         JsonSerializer.Serialize(c, (JsonSerializerOptions?)null),
                         (JsonSerializerOptions?)null) ?? new Dictionary<string, object>()
            );
        }

        /// <summary>
        /// Cria um ValueComparer genérico para qualquer tipo que pode ser serializado como JSON
        /// </summary>
        public static ValueComparer<T> GetJsonComparer<T>() where T : class, new()
        {
            return new ValueComparer<T>(
                (c1, c2) => c1 == null && c2 == null ||
                            (c1 != null && c2 != null
                            && JsonSerializer.Serialize(c1, (JsonSerializerOptions?)null) == JsonSerializer.Serialize(c2, (JsonSerializerOptions?)null)),

                c => c == null ? 0 : JsonSerializer.Serialize(c, (JsonSerializerOptions?)null).GetHashCode(),

                c => c == null ? new T() :
                     JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(c, (JsonSerializerOptions?)null),
                         (JsonSerializerOptions?)null) ?? new T()
            );
        }

        /// <summary>
        /// Cria um ValueComparer para List<T> serializado como JSON
        /// </summary>
        public static ValueComparer<List<T>> GetListComparer<T>()
        {
            return new ValueComparer<List<T>>(
                (c1, c2) => c1 == null && c2 == null ||
                            (c1 != null && c2 != null && JsonSerializer.Serialize(c1, (JsonSerializerOptions?)null) == JsonSerializer.Serialize(c2, (JsonSerializerOptions?)null)),

                c => c == null ? 0 : JsonSerializer.Serialize(c, (JsonSerializerOptions?)null).GetHashCode(),

                c => c == null ? new List<T>() :
                     JsonSerializer.Deserialize<List<T>>(JsonSerializer.Serialize(c, (JsonSerializerOptions?)null),
                         (JsonSerializerOptions?)null) ?? new List<T>()
            );
        }
    }
}

