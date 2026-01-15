using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Commons.Api.Localization
{
    /// <summary>
    /// Provê métodos para localização de recursos de acordo com a cultura.
    /// </summary>
    public interface ILocalizationManager
    {
        /// <summary>
        /// Returns a resource value from a resource code in all configured resource namespaces (.resx files).
        /// </summary>
        /// <param name="resourceCode"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        string Get(string resourceCode, CultureInfo? culture = null);
        string Get(string resourceCode, params object?[] args);
        string Get(string resourceCode, CultureInfo? culture, params object?[] args);

        /// <summary>
        /// Returns a resource value from a resource code in a specifc resource namespaces (the indicated .resx).
        /// </summary>
        /// <param name="resourceNamespace"></param>
        /// <param name="resourceCode"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        string GetFromNamespace(string resourceNamespace, string resourceCode, CultureInfo? culture = null, params object?[] args);
    }
}
