using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Commons.Api.Localization;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Commons.Api.Exceptions
{
    /// <summary>
    /// Representa uma exceção personalizada para erros de API.
    /// </summary>
    public class ApiException : Exception
    {
        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="ApiException"/>.
        /// </summary>

        [ExcludeFromCodeCoverage]
        public ApiException() { }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="ApiException"/>.
        /// </summary>
        /// <param name="httpStatusCodeResponse"></param>
        /// <param name="code"></param>
        /// <param name="message"></param>
        public ApiException(HttpStatusCode httpStatusCodeResponse, string code, string message) : base(message)
        {
            HttpStatusCodeResponse = httpStatusCodeResponse;
            Code = code;
        }
        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="ApiException"/>.
        /// </summary>
        /// <param name="httpStatusCodeResponse"></param>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ApiException(HttpStatusCode httpStatusCodeResponse, string code, string message, Exception innerException) : base(message, innerException)
        {
            HttpStatusCodeResponse = httpStatusCodeResponse;
            Code = code;
        }
        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="ApiException"/>.
        /// </summary>
        /// <param name="httpStatusCodeResponse"></param>
        /// <param name="localizationManager"></param>
        /// <param name="resourceCode"></param>
        /// <param name="args"></param>
        public ApiException(HttpStatusCode httpStatusCodeResponse, ILocalizationManager localizationManager, string resourceCode, params object[] args) :
            base(localizationManager.Get(resourceCode, culture: CommonLocalization.CurrentCulture, args))
        {
            HttpStatusCodeResponse = httpStatusCodeResponse;
            Code = resourceCode;
        }
        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="ApiException"/>.
        /// </summary>
        /// <param name="httpStatusCodeResponse"></param>
        /// <param name="code"></param>
        /// <param name="errors"></param>
        /// <param name="defaultMessage"></param>
        public ApiException(HttpStatusCode httpStatusCodeResponse, string code, IEnumerable<ErrorDTO> errors, string defaultMessage = null) : base(defaultMessage ?? code)
        {
            HttpStatusCodeResponse = httpStatusCodeResponse;
            Code = code;
            Errors = errors;
        }

        #region Internal CommonLocalization constructor

        /// <summary>
        /// Initializes a new instance of the ApiException class with a localized error message, HTTP status code, and
        /// optional formatting arguments.
        /// </summary>
        /// <remarks>This constructor is intended for internal use to create exceptions with standardized,
        /// localized messages based on resource codes. The error message is generated using the specified resource code
        /// and formatting arguments.</remarks>
        /// <param name="httpStatusCodeResponse">The HTTP status code associated with the API error response.</param>
        /// <param name="commonResourceCode">The resource code used to retrieve the localized error message.</param>
        /// <param name="args">An array of objects containing formatting arguments to be applied to the localized error message.</param>
        internal ApiException(HttpStatusCode httpStatusCodeResponse, string commonResourceCode, params object[] args) :
            base(CommonLocalization.Get(commonResourceCode, culture: null, args))
        {
            HttpStatusCodeResponse = httpStatusCodeResponse;
            Code = commonResourceCode;
        }


        internal ApiException(HttpStatusCode httpStatusCodeResponse, CultureInfo cultureInfo, string commonResourceCode,
            params object[] args) :
            base(CommonLocalization.Get(commonResourceCode, cultureInfo, args))
        {
            HttpStatusCodeResponse = httpStatusCodeResponse;
            Code = commonResourceCode;
        }


        internal ApiException(Exception innerException, HttpStatusCode httpStatusCodeResponse, CultureInfo cultureInfo,
            string commonResourceCode, params object[] args) : base(
            CommonLocalization.Get(commonResourceCode, cultureInfo, args), innerException)
        {
            HttpStatusCodeResponse = httpStatusCodeResponse;
            Code = commonResourceCode;
        }
        #endregion Internal CommonLocalization constructor

        public HttpStatusCode HttpStatusCodeResponse { get; set; }

        public string Code { get; set; }

        public IEnumerable<ErrorDTO> Errors { get; set; }
    }
}
