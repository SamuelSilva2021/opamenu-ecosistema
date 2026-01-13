using System.Globalization;

namespace Authenticator.API.Core.Application.Interfaces
{
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
