using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OpaMenu.Application.Implementation.Security
{
    public static partial class SensitivePropertyValidator
    {
        //TODO: Load from appsettings
        private static List<string>? propertiesFromAppSettings = new List<string>();

        private static HashSet<string> DefaultSensitiveProperties => new(StringComparer.OrdinalIgnoreCase)
    {
        "Password",
        "Token",
        "Key",
        "Secret",
        "CVV",
        "Pin",
        "OTP",
        "Senha",
        "CodVerificador",
        "CypherId",
        "DataExpircao",
        "CodigoCartao",
        "PinBlock",
        "DataVencimento",
        "Birthday",
        "Father",
        "Mother",
        "ExpirationDate",
        "NumCartao"
    };

        private static HashSet<string> CpfDocumentSensitiveProperties => new(StringComparer.OrdinalIgnoreCase) { "Cpf" };

        private static HashSet<string> CnpjDocumentSensitiveProperties => new(StringComparer.OrdinalIgnoreCase) { "Cnpj" };

        private static HashSet<string> CardNumberSensitiveProperties => new(StringComparer.OrdinalIgnoreCase) { "CardNumber", "NumCartao" };

        [GeneratedRegex("(_|-)*")]
        private static partial Regex WordSeparatorRegex();

        public static HashSet<string> GetSensitiveProperties()
        {
            var response = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var property in DefaultSensitiveProperties)
                _ = response.Add(property);

            try
            {
                foreach (var property in propertiesFromAppSettings ?? [])
                {
                    _ = response.Add(property);
                }
            }
            catch (Exception)
            {
                // Do nothing
            }

            return response;
        }

        public static bool IsSensitive(string key)
        {
            return GetSensitiveProperties().Any(ContainsKey(key));
        }

        public static IEnumerable<string> GetSensitivePropertiesInString(string key)
        {
            return GetSensitiveProperties().Where(ContainsKey(key)).Distinct();
        }

        private static Func<string, bool> ContainsKey(string key)
        {
            return e => Normalize(key).Contains(Normalize(e));
        }

        private static string Normalize(string value)
        {
            return WordSeparatorRegex().Replace(value.ToUpper().Trim(), "");
        }

        public static bool IsCpfDocumentProperty(string key)
        {
            return CpfDocumentSensitiveProperties.Any(ContainsKey(key));
        }

        public static bool IsCnpjDocumentProperty(string key)
        {
            return CnpjDocumentSensitiveProperties.Any(ContainsKey(key));
        }

        public static bool IsCardNumberProperty(string key)
        {
            return CardNumberSensitiveProperties.Any(ContainsKey(key));
        }

        public static bool ShouldMask(string key)
        {
            return IsSensitive(key) || IsCpfDocumentProperty(key) || IsCnpjDocumentProperty(key) || IsCardNumberProperty(key);
        }

        /// <summary>
        /// Checks if a property is marked with SensitiveKeyAttribute.
        /// </summary>
        /// <param name="propertyInfo">Property to check</param>
        /// <returns>True if the property has SensitiveKeyAttribute</returns>
        public static bool HasSensitiveKeyAttribute(PropertyInfo propertyInfo)
        {
            return propertyInfo.GetCustomAttribute<SensitiveKeyAttribute>() != null;
        }

        /// <summary>
        /// Checks if a property should be masked based on name patterns or SensitiveKeyAttribute.
        /// </summary>
        /// <param name="propertyInfo">Property to check</param>
        /// <returns>True if the property should be masked</returns>
        public static bool ShouldMask(PropertyInfo propertyInfo)
        {
            return ShouldMask(propertyInfo.Name) || HasSensitiveKeyAttribute(propertyInfo);
        }

        /// <summary>
        /// Gets property names marked with SensitiveKeyAttribute from a given type.
        /// </summary>
        /// <param name="type">Type to scan for sensitive properties</param>
        /// <returns>Collection of property names marked as sensitive</returns>
        public static IEnumerable<string> GetSensitivePropertyNamesFromType(Type type)
        {
            return type.GetProperties()
                       .Where(HasSensitiveKeyAttribute)
                       .Select(p => p.Name);
        }

        /// <summary>
        /// Gets all property names that should be masked for a given type (both pattern-based and attribute-based).
        /// </summary>
        /// <param name="type">Type to scan</param>
        /// <returns>Collection of property names that should be masked</returns>
        public static IEnumerable<string> GetAllSensitivePropertyNamesFromType(Type type)
        {
            var attributeBasedProperties = GetSensitivePropertyNamesFromType(type);
            var patternBasedProperties = type.GetProperties()
                                            .Where(p => ShouldMask(p.Name))
                                            .Select(p => p.Name);

            return attributeBasedProperties.Concat(patternBasedProperties).Distinct();
        }
    }
    /// <summary>
    /// Sensitive key annotation to mark properties as sensitive for logging and history masking.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SensitiveKeyAttribute : Attribute
    {
    }
}
