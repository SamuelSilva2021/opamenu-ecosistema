using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Application.Common.Localization
{
    public static class CommonResource
    {
        /// <summary>
        /// Por padrão, criar textos em "pt-BR" e em "en".
        /// Deixar um comentário no resource.resx indicando onde esse código de erro é usado.
        /// Adicionar um teste ApiResourceTest.cs para garantir retorne resultados.
        /// </summary>

        #region Common errors
        public const string FORBIDDEN = "COMMON_FORBIDDEN";
        public const string UNAUTHORIZED = "COMMON_UNAUTHORIZED";
        public const string UNSUPPORTED_ARGUMENT = "COMMON_UNSUPPORTED_ARGUMENT";
        public const string CONFLICT = "COMMON_CONFLICT";
        public const string NOT_FOUND = "COMMON_NOT_FOUND";
        public const string NOT_FOUND_UNEXPECTED = "COMMON_NOT_FOUND_UNEXPECTED";
        #endregion Common errors

        #region Configuration errors
        public const string LOCALIZATION_NAMESPACE_REQUIRED = "COMMON_LOCALIZATION_NAMESPACE_REQUIRED";
        public const string APPSETTINGS_REQUIRED = "COMMON_APPSETTINGS_REQUIRED";
        public const string APPSETTINGS_INVALID = "COMMON_APPSETTINGS_INVALID";
        public const string ACCESSCONTROL_MISSING_AUXILIAR = "COMMON_ACCESSCONTROL_MISSING_AUXILIAR";
        public const string ACCESSCONTROL_INVALID_AUXILIAR = "COMMON_ACCESSCONTROL_INVALID_AUXILIAR";
        public const string APPSETTINGS_MUST_BE_LOWER_OR_EQUAL = "COMMON_APPSETTINGS_MUST_BE_LOWER_OR_EQUAL";
        public const string APPSETTINGS_MUST_BE_LOWER = "COMMON_APPSETTINGS_MUST_BE_LOWER";
        public const string APPSETTINGS_MUST_BE_GREATER_OR_EQUAL = "COMMON_APPSETTINGS_MUST_BE_GREATER_OR_EQUAL";
        public const string APPSETTINGS_MUST_BE_GREATER = "COMMON_APPSETTINGS_MUST_BE_GREATER";
        public const string MISSING_ENVIRONMENT = "COMMON_MISSING_ENVIRONMENT";
        #endregion Configuration errors

        #region Validation errors
        public const string FIELD_INVALID = "COMMON_FIELD_INVALID";
        public const string FIELD_REQUIRED = "COMMON_FIELD_REQUIRED";
        public const string FIELD_LENGTH_MAX = "COMMON_FIELD_LENGTH_MAX";
        public const string FIELD_LENGTH_MIN = "COMMON_FIELD_LENGTH_MIN";
        public const string FIELD_VALUE_MAX = "COMMON_FIELD_VALUE_MAX";
        public const string FIELD_VALUE_MIN = "COMMON_FIELD_VALUE_MIN";
        public const string FIELD_VALUE_MAX_OR_EQUAL = "COMMON_FIELD_VALUE_MAX_OR_EQUAL";
        public const string FIELD_VALUE_MIN_OR_EQUAL = "COMMON_FIELD_VALUE_MIN_OR_EQUAL";
        public const string FIELD_INVALID_URL = "COMMON_FIELD_INVALID_URL";
        public const string DATE_INTERVAL = "COMMON_DATE_INTERVAL";
        public const string DATE_INTERVAL_INCLUSIVE = "COMMON_DATE_INTERVAL_INCLUSIVE";
        public const string VALIDATION_ERROR = "COMMON_VALIDATION_ERROR";
        #endregion Validation errors

        #region Password validation errors
        public const string PASSWORD_MIN_LENGTH = "COMMON_PASSWORD_MIN_LENGTH";
        public const string PASSWORD_CONFIRMATION = "COMMON_PASSWORD_CONFIRMATION";
        public const string PASSWORD_ALREADY_USED = "COMMON_PASSWORD_ALREADY_USED";
        public const string PASSWORD_INVALID_CHARACTERS = "COMMON_PASSWORD_INVALID_CHARACTERS";
        public const string PASSWORD_LETTERS_AND_NUMBERS = "COMMON_PASSWORD_LETTERS_AND_NUMBERS";
        public const string PASSWORD_SEQUENCE_REPETITION = "COMMON_PASSWORD_SEQUENCE_REPETITION";
        public const string PASSWORD_SEQUENCE_REPETITION_EXAMPLE = "COMMON_PASSWORD_SEQUENCE_REPETITION_EXAMPLE";
        public const string PASSWORD_SEQUENCE = "COMMON_PASSWORD_SEQUENCE";
        public const string PASSWORD_USERNAME_SEQUENCE = "COMMON_PASSWORD_USERNAME_SEQUENCE";
        public const string PASSWORD_USERNAME_SEQUENCE_EXAMPLE = "COMMON_PASSWORD_USERNAME_SEQUENCE_EXAMPLE";
        public const string PASSWORD_REPETITION = "COMMON_PASSWORD_REPETITION";
        public const string PASSWORD_COMMON_USED = "COMMON_PASSWORD_COMMON_USED";
        public const string PASSWORD_COMMON_USED_EXAMPLE = "COMMON_PASSWORD_COMMON_USED_EXAMPLE";
        public const string PASSWORD_MIN_MIXED_CHARACTERS = "COMMON_PASSWORD_MIN_MIXED_CHARACTERS";
        public const string PASSWORD_MIN_UPPER_CHARACTERS = "COMMON_PASSWORD_MIN_UPPER_CHARACTERS";
        public const string PASSWORD_MIN_LOWER_CHARACTERS = "COMMON_PASSWORD_MIN_LOWER_CHARACTERS";
        public const string PASSWORD_MIN_NUMERIC_CHARACTERS = "COMMON_PASSWORD_MIN_NUMERIC_CHARACTERS";
        public const string PASSWORD_MIN_SYMBOL_CHARACTERS = "COMMON_PASSWORD_MIN_SYMBOL_CHARACTERS";
        #endregion Password validation errors

        #region Authorization errors
        public const string MISSING_CREDENTIALS = "COMMON_MISSING_CREDENTIALS";
        public const string TOKEN_EXPIRED = "COMMON_TOKEN_EXPIRED";
        public const string TOKEN_EXPIRED_LAST_EXPIRATION = "COMMON_TOKEN_EXPIRED_LAST_EXPIRATION";
        public const string TOKEN_REFRESH_LIMIT = "COMMON_TOKEN_REFRESH_LIMIT";
        public const string TOKEN_DECRYPT_FAILED = "COMMON_TOKEN_DECRYPT_FAILED";
        public const string TOKEN_INVALID = "COMMON_TOKEN_INVALID";
        public const string TOKEN_INVALID_ENVIRONMENT = "COMMON_TOKEN_INVALID_ENVIRONMENT";
        #endregion Authorization errors

        #region General errors
        public const string INTERNAL_SERVER_ERROR = "COMMON_INTERNAL_SERVER_ERROR";
        public const string INTERNAL_NOT_IMPLEMENTED = "COMMON_INTERNAL_NOT_IMPLEMENTED";
        public const string VALIDATION_NULL_BODY = "COMMON_VALIDATION_NULL_BODY";
        public const string PERMISSION_MODULE_OPERATION_MISSING = "COMMON_PERMISSION_MODULE_OPERATION_MISSING";
        public const string INVALID_OTP = "COMMON_INVALID_OTP";
        #endregion General errors

        #region Common words/terms
        public const string TERM_BIRTHDATE = "TERM_BIRTHDATE";
        public const string TERM_CELLPHONE = "TERM_CELLPHONE";
        public const string TERM_EMAIL = "TERM_EMAIL";
        public const string TERM_NAME = "TERM_NAME";
        public const string TERM_PASSWORD = "TERM_PASSWORD";
        public const string TERM_PHONE = "TERM_PHONE";
        public const string TERM_GENDER = "TERM_GENDER";
        public const string TERM_USERNAME = "TERM_USERNAME";
        public const string TERM_USER = "TERM_USER";
        public const string TERM_CARD = "TERM_CARD";
        public const string TERM_ADDRESS = "TERM_ADDRESS";
        public const string TERM_ADDRESS_STREET = "TERM_ADDRESS_STREET";
        public const string TERM_STATE = "TERM_STATE";
        public const string TERM_CITY = "TERM_CITY";
        public const string TERM_NEIGHBORHOOD = "TERM_NEIGHBORHOOD";
        public const string TERM_NUMBER = "TERM_NUMBER";
        public const string TERM_COMPLEMENT = "TERM_COMPLEMENT";
        public const string TERM_LATITUDE = "TERM_LATITUDE";
        public const string TERM_LONGITUDE = "TERM_LONGITUDE";
        public const string TERM_DATE = "TERM_DATE";
        public const string TERM_TIME = "TERM_TIME";
        public const string TERM_TRANSACTION = "TERM_TRANSACTION";
        public const string TERM_PERMISSIONS = "TERM_PERMISSIONS";
        #endregion Common words/terms

        #region Pagination
        public const string ORDER_BY_REQUIRED = "COMMON_ORDER_BY_REQUIRED";
        public const string NEGATIVE_PAGE_NUMBER = "COMMON_NEGATIVE_PAGE_NUMBER";
        public const string NEGATIVE_PAGE_SIZE = "COMMON_NEGATIVE_PAGE_SIZE";
        public const string PAGE_SIZE_EXCEEDED = "COMMON_PAGE_SIZE_EXCEEDED";
        #endregion

        #region System
        public const string INVALID_TYPE = "COMMON_INVALID_TYPE";
        #endregion

        #region Phone Validation Errors
        public const string INVALID_PHONE_NUMBER = "COMMON_INVALID_PHONE";
        #endregion

        #region Security
        public const string SECURITY_BLOCK = "COMMON_SECURITY_BLOCK";
        public const string MISSING_MAP_PERMISSION = "COMMON_MISSING_MAP_PERMISSION";
        #endregion Security

        #region Authentication
        public const string INVALID_USER_OR_PASSWORD = "COMMON_INVALID_USER_OR_PASSWORD";
        public const string LOGIN_UNAUTHORIZED_MESSAGE = "COMMON_LOGIN_UNAUTHORIZED_MESSAGE";
        public const string EMPTY_USERNAME = "COMMON_EMPTY_USERNAME";
        public const string EMPTY_PASSWORD = "COMMON_EMPTY_PASSWORD";
        public const string INVALID_PASSWORD = "COMMON_INVALID_PASSWORD";
        public const string EMPTY_CPF = "COMMON_EMPTY_CPF";
        public const string INVALID_CPF = "COMMON_INVALID_CPF";
        public const string INVALID_CPF_OR_PASSWORD = "COMMON_INVALID_CPF_OR_PASSWORD";
        public const string EMPTY_TOKEN = "COMMON_EMPTY_TOKEN";
        #endregion Authentication

        #region Distributed Cache
        public const string CACHE_CONNECTION_FAILED = "COMMON_CACHE_CONNECTION_FAILED";
        public const string CACHE_OPERATION_FAILED = "COMMON_CACHE_OPERATION_FAILED";
        public const string CACHE_SERIALIZATION_FAILED = "COMMON_CACHE_SERIALIZATION_FAILED";
        public const string CACHE_DESERIALIZATION_FAILED = "COMMON_CACHE_DESERIALIZATION_FAILED";
        public const string CACHE_KEY_NULL = "COMMON_CACHE_KEY_NULL";
        public const string CACHE_VALUE_NULL = "COMMON_CACHE_VALUE_NULL";
        public const string CACHE_TTL_INVALID = "COMMON_CACHE_TTL_INVALID";
        #endregion Distributed Cache
    }
}
