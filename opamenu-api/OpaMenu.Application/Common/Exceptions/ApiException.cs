using OpaMenu.Application.Common.Localization;
using OpaMenu.Application.Interfaces;
using OpaMenu.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Application.Common.Exceptions
{
    /// <summary>
    /// ApiException Class
    /// </summary>
    public class ApiException : Exception
    {
        /// <summary>
        /// ApiException Blank Constructor
        /// </summary>
        [ExcludeFromCodeCoverage]
        public ApiException() { }

        /// <summary>
        /// ApiException Constructor
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
        /// ApiException constructor
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
        /// ApiException constructor using LocalizationManager to initialize the error message
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
        /// ApiException Constructor that receives a errorList that can be thrown as a group of errors.
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
        /// ApiException constructor using CommonLocalization to initialize the error message
        /// </summary>
        /// <param name="httpStatusCodeResponse"></param>
        /// <param name="commonResourceCode"></param>
        /// <param name="args"></param>
        internal ApiException(HttpStatusCode httpStatusCodeResponse, string commonResourceCode, params object[] args) :
            base(CommonLocalization.Get(commonResourceCode, culture: null, args))
        {
            HttpStatusCodeResponse = httpStatusCodeResponse;
            Code = commonResourceCode;
        }

        /// <summary>
        /// ApiException constructor using CommonLocalization to initialize the error message
        /// </summary>
        /// <param name="httpStatusCodeResponse"></param>
        /// <param name="cultureInfo"></param>
        /// <param name="commonResourceCode"></param>
        /// <param name="args"></param>
        internal ApiException(HttpStatusCode httpStatusCodeResponse, CultureInfo cultureInfo, string commonResourceCode,
            params object[] args) :
            base(CommonLocalization.Get(commonResourceCode, cultureInfo, args))
        {
            HttpStatusCodeResponse = httpStatusCodeResponse;
            Code = commonResourceCode;
        }

        /// <summary>
        /// ApiException constructor using CommonLocalization to initialize the error message
        /// </summary>
        /// <param name="innerException"></param>
        /// <param name="httpStatusCodeResponse"></param>
        /// <param name="cultureInfo"></param>
        /// <param name="commonResourceCode"></param>
        /// <param name="args"></param>
        internal ApiException(Exception innerException, HttpStatusCode httpStatusCodeResponse, CultureInfo cultureInfo,
            string commonResourceCode, params object[] args) : base(
            CommonLocalization.Get(commonResourceCode, cultureInfo, args), innerException)
        {
            HttpStatusCodeResponse = httpStatusCodeResponse;
            Code = commonResourceCode;
        }
        #endregion Internal CommonLocalization constructor

        /// <summary>
        /// ApiException StatusCodeResponse
        /// </summary>
        public HttpStatusCode HttpStatusCodeResponse { get; set; }

        /// <summary>
        /// Code propertie
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// List of errors
        /// </summary>
        public IEnumerable<ErrorDTO> Errors { get; set; }
    }
}
