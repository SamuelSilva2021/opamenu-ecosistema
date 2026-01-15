using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OpaMenu.Commons.Api.Localization
{
    /// <summary>
    /// Provê métodos para localização de mensagens comuns da aplicação.
    /// </summary>
    public static class CommonLocalization
    {
        public const string ResourceNamespace = "OpaMenu.Commons.Api.Localization";
        private static ResourceManager _resourceManager;

        /// <summary>
        /// Representa a cultura padrão utilizada na aplicação (Português - Brasil).
        /// </summary>
        public static readonly CultureInfo DefaultCulture = new CultureInfo("pt-BR");

        /// <summary>
        /// Obtém ou define a cultura atual utilizada para localização de mensagens.
        /// </summary>
        public static CultureInfo CurrentCulture { get; private set; } = DefaultCulture;

        /// <summary>
        /// Obtém o gerenciador de recursos para acessar as mensagens localizadas.
        /// </summary>
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

        /// <summary>
        /// Estabelece a cultura atual utilizada para localização de mensagens.
        /// </summary>
        /// <param name="culture"></param>
        public static void SetCurrentCulture(string culture)
        {
            CurrentCulture = Culture.New(culture);
        }

        /// <summary>
        /// Busca o valor localizado para o código de recurso especificado.
        /// </summary>
        /// <param name="resourceCode"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static string Get(string resourceCode, CultureInfo? culture = null)
        {
            return GetResourceValue(resourceCode, culture);
        }

        /// <summary>
        /// Busca o valor localizado para o código de recurso especificado e formata com os argumentos fornecidos.
        /// </summary>
        /// <param name="resourceCode"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string Get(string resourceCode, params object?[] args)
        {
            return Get(resourceCode, culture: null, args);
        }

        /// <summary>
        /// Busca o valor localizado para o código de recurso especificado e formata com os argumentos fornecidos.
        /// </summary>
        /// <param name="resourceCode"></param>
        /// <param name="culture"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string Get(string resourceCode, CultureInfo? culture, params object?[] args)
        {
            var message = GetResourceValue(resourceCode, culture);
            return string.IsNullOrWhiteSpace(message) ? message : string.Format(message, args);
        }

        /// <summary>
        /// Busca o valor localizado para o código de recurso especificado.
        /// </summary>
        /// <param name="resourceCode"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        private static string GetResourceValue(string resourceCode, CultureInfo? culture = null)
        {
            var cultureInfo = culture ?? CurrentCulture;

            var resourceValue = ResourceManager.GetString(resourceCode, cultureInfo) ?? string.Empty;

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
