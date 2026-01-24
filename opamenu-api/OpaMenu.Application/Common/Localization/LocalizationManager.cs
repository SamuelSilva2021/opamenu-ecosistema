using OpaMenu.Application.Services.Interfaces.Opamenu;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Text;

namespace OpaMenu.Application.Common.Localization
{
    public class LocalizationManager : ILocalizationManager
    {
        private static readonly IOrderedDictionary ResourceManagers = new OrderedDictionary();

        public LocalizationManager(Assembly assembly, bool fallbackToCommonLocalization = true, params string[] resourceNamespaces)
        {
            if ((resourceNamespaces?.Length > 0) is false)
                throw new ArgumentException(CommonLocalization.Get(CommonResource.LOCALIZATION_NAMESPACE_REQUIRED));

            foreach (var resourceNamespace in resourceNamespaces)
            {
                if (string.IsNullOrWhiteSpace(resourceNamespace) is false)
                    AddResourceManager(resourceNamespace, assembly);
            }

            if (fallbackToCommonLocalization)
            {
                AddResourceManager(CommonLocalization.ResourceNamespace, typeof(CommonLocalization).Assembly);
            }
        }

        public string Get(string resourceCode, CultureInfo? culture = null)
        {
            return GetFirstResourceValueFromAllNamespaces(resourceCode, culture);
        }

        public string Get(string resourceCode, params object?[] args)
        {
            return Get(resourceCode, culture: null, args);
        }

        public string Get(string resourceCode, CultureInfo? culture, params object?[] args)
        {
            var message = GetFirstResourceValueFromAllNamespaces(resourceCode, culture);
            return string.IsNullOrWhiteSpace(message) ? message : string.Format(message, args);
        }

        public string GetFromNamespace(string resourceNamespace, string resourceCode, CultureInfo? culture = null, params object?[] args)
        {
            var message = GetResourceValueFromNamespace(resourceNamespace, resourceCode, culture);
            return string.IsNullOrWhiteSpace(message) ? message : string.Format(message, args);
        }

        private static ResourceManager GetResourceManager(string resourceNamespace)
        {
            if (ResourceManagers.Contains(resourceNamespace))
                return (ResourceManager)ResourceManagers[resourceNamespace]!;

            throw new KeyNotFoundException(CommonLocalization.Get(CommonResource.LOCALIZATION_NAMESPACE_REQUIRED));
        }

        private static string GetResourceValueFromNamespace(string resourceNamespace, string resourceCode,
            CultureInfo? culture = null)
        {
            var cultureInfo = culture ?? CommonLocalization.CurrentCulture;

            var resourceValue = GetResourceManager(resourceNamespace).GetString(resourceCode, cultureInfo) ?? string.Empty;

            // If empty value for current culture, then get the default culture resource value
            return string.IsNullOrEmpty(resourceValue) && !CommonLocalization.DefaultCulture.Equals(cultureInfo)
                ? GetDefaultResourceValueFromNamespace(resourceNamespace, resourceCode)
                : resourceValue;
        }

        private static string GetDefaultResourceValueFromNamespace(string resourceNamespace, string resourceCode)
        {
            return GetResourceManager(resourceNamespace).GetString(resourceCode, CommonLocalization.CurrentCulture) ??
                   string.Empty;
        }

        private static string GetFirstResourceValueFromAllNamespaces(string resourceCode, CultureInfo? culture = null)
        {
            var cultureInfo = culture ?? CommonLocalization.CurrentCulture;

            var resourceValue = GetFirstResourceValueByCultureFromAllNamespaces(resourceCode, cultureInfo);

            // If empty value for current culture, then get the default culture resource value
            return string.IsNullOrEmpty(resourceValue) && !CommonLocalization.DefaultCulture.Equals(cultureInfo)
                ? GetFirstResourceValueByCultureFromAllNamespaces(resourceCode, CommonLocalization.DefaultCulture)
                : resourceValue;
        }

        private static string GetFirstResourceValueByCultureFromAllNamespaces(string resourceCode, CultureInfo culture)
        {
            foreach (DictionaryEntry dictionaryEntry in ResourceManagers)
            {
                var resourceManager = (ResourceManager)dictionaryEntry.Value!;
                var resourceValue = resourceManager.GetString(resourceCode, culture) ?? string.Empty;

                if (string.IsNullOrWhiteSpace(resourceValue) is false)
                    return resourceValue;
            }

            return string.Empty;
        }

        private static void AddResourceManager(string resourceNamespace, Assembly assembly)
        {
            if (ResourceManagers.Contains(resourceNamespace))
                return;

            var resourceManager = new ResourceManager(resourceNamespace, assembly);
            ResourceManagers.Add(resourceNamespace, resourceManager);
        }
    }
}
