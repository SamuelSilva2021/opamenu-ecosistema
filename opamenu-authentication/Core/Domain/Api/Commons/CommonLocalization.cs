using System.Globalization;
using System.Resources;
using System.Text.RegularExpressions;

namespace Authenticator.API.Core.Domain.Api.Commons
{
    public static class CommonLocalization
    {
        public const string ResourceNamespace = "OpaMenu.Application.Common.Localization.Resource";
        private static ResourceManager _resourceManager;

        /// <summary>
        /// Default culture if no culture or no message found
        /// </summary>
        public static readonly CultureInfo DefaultCulture = new CultureInfo("pt-BR");

        /// <summary>
        /// Current selected culture, used when no culture provided for GetResourceValue method
        /// </summary>
        public static CultureInfo CurrentCulture { get; private set; } = DefaultCulture;

        public static ResourceManager ResourceManager
        {
            get
            {
                if (ReferenceEquals(_resourceManager, null))
                {
                    _resourceManager = new ResourceManager(ResourceNamespace, typeof(CommonLocalization).Assembly);
                }

                return _resourceManager;
            }
        }

        public static void SetCurrentCulture(string culture)
        {
            CurrentCulture = Culture.New(culture);
        }

        public static string Get(string resourceCode, CultureInfo? culture = null)
        {
            return GetResourceValue(resourceCode, culture);
        }

        public static string Get(string resourceCode, params object?[] args)
        {
            return Get(resourceCode, culture: null, args);
        }

        public static string Get(string resourceCode, CultureInfo? culture, params object?[] args)
        {
            var message = GetResourceValue(resourceCode, culture);
            return string.IsNullOrWhiteSpace(message) ? message : string.Format(message, args);
        }

        private static string GetResourceValue(string resourceCode, CultureInfo? culture = null)
        {
            var cultureInfo = culture ?? CurrentCulture;

            var resourceValue = ResourceManager.GetString(resourceCode, cultureInfo) ?? string.Empty;

            // If empty value for current culture, then get the default culture resource value
            return string.IsNullOrEmpty(resourceValue) && !DefaultCulture.Equals(cultureInfo)
                ? GetDefaultResourceValue(resourceCode)
                : resourceValue;
        }

        private static string GetDefaultResourceValue(string resourceCode)
        {
            return ResourceManager.GetString(resourceCode, DefaultCulture) ?? string.Empty;
        }
    }
}
