using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Resources;
using Serilog;
using System.Net;
using System.Text.Json;
using Xunit.Sdk;
using FluentValidation;

using ValidationFailure = FluentValidation.Results.ValidationFailure;

namespace Authenticator.API.Core.Domain.Api.Commons
{
    public class StaticResponseBuilder<T>
    {

        /// <summary>
        /// BuildCreated 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ResponseDTO<T> BuildCreated(T value)
            => new()
            {
                Succeeded = true,
                Code = (int)HttpStatusCode.Created,
                Data = value
            };

        /// <summary>
        /// BuildOk
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ResponseDTO<T> BuildOk(T value)
            => new()
            {
                Succeeded = true,
                Code = (int)HttpStatusCode.OK,
                Data = value
            };

        /// <summary>
        /// BuildNotContent
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ResponseDTO<T> BuildNoContent(T value)
            => new()
            {
                Succeeded = true,
                Code = (int)HttpStatusCode.NoContent,
                Data = value
            };

        public static ResponseDTO<T> BuildNotFound(T value) =>
            new()
            {
                Succeeded = false,
                Code = (int)HttpStatusCode.NotFound,
                Data = value
            };

        /// <summary>
        /// BuildError with single message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ResponseDTO<T> BuildError(string message)
            => new()
            {
                Succeeded = false,
                Code = (int)HttpStatusCode.BadRequest,
                Errors = new List<ErrorDTO> { new ErrorDTO { Message = message, Code = "ERROR" } }
            };

        /// <summary>
        /// BuildError with list of messages
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns>
        public static ResponseDTO<T> BuildError(IList<string> messages)
            => new()
            {
                Succeeded = false,
                Code = (int)HttpStatusCode.BadRequest,
                Errors = messages.Select(m => new ErrorDTO { Message = m, Code = "ERROR" }).ToList()
            };

        /// <summary>
        /// BuildPagedOk
        /// </summary>
        /// <param name="data"></param>
        /// <param name="totalItems"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static PagedResponseDTO<TItem> BuildPagedOk<TItem>(IEnumerable<TItem> data, int totalItems, int pageNumber, int pageSize)
        {
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return new PagedResponseDTO<TItem>
            {
                Succeeded = true,
                Code = (int)HttpStatusCode.OK,
                Items = data,
                Total = totalItems,
                TotalPages = totalPages,
                CurrentPage = pageNumber,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// BuildErrorResponse
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static ResponseDTO<T> BuildErrorResponse(Exception exception)
        {
            var code = (int)HttpStatusCode.InternalServerError;

            ResponseDTO<T> responseError = new() { Errors = new List<ErrorDTO>(), Headers = new Dictionary<string, string>(), Exception = exception };

            if (exception is ValidationException validationException)
            {
                code = (int)HttpStatusCode.BadRequest;
                BuildValidationExceptionError(validationException, responseError.Errors);
            }
            else if (exception is JsonException jsonException)
            {
                code = (int)HttpStatusCode.BadRequest;
                BuildJsonExceptionError(jsonException, responseError.Errors);
            }
            else if (exception is RedirectionException redirectionException)
            {
                code = (int)redirectionException.HttpStatusCodeResponse;
                BuildRedirectionExceptionError(redirectionException, responseError);
            }
            else if (exception is ApiException apiException)
            {
                code = (int)apiException.HttpStatusCodeResponse;
                BuildApiExceptionError(apiException, responseError.Errors);
            }
            else
            {
                Log.Error(exception, exception.Message);

                var errorDto = new ErrorDTO
                {
                    Code = CommonResource.INTERNAL_SERVER_ERROR,
                    Message = CommonLocalization.Get(CommonResource.INTERNAL_SERVER_ERROR)
                };

                var details = new List<string>();
                if (!string.IsNullOrEmpty(exception.Message)) details.Add(exception.Message);
                if (exception.InnerException != null && !string.IsNullOrEmpty(exception.InnerException.Message)) details.Add(exception.InnerException.Message);

                if (details.Any()) errorDto.Details = details;

                responseError.Errors.Add(errorDto);
            }

            responseError.Code = code;

            return responseError;
        }

        /// <summary>
        /// BuildRedirectionExceptionError
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="errors"></param>
        private static void BuildRedirectionExceptionError(RedirectionException exception, ResponseDTO<T> response)
        {
            response.Errors.Add(new ErrorDTO { Code = exception.Code, Message = exception.Message });
            response.Headers.Add("location", exception.Location);
        }

        /// <summary>
        /// BuildValidationExceptionError
        /// </summary>
        /// <param name="validationException"></param>
        /// <param name="errors"></param>
        private static void BuildValidationExceptionError(ValidationException validationException, IList<ErrorDTO> errors)
        {
            if (null != validationException.Message)
            {
                using IEnumerator<ValidationFailure> validationFailure = validationException.Errors.GetEnumerator();
                while (validationFailure.MoveNext())
                {
                    errors.Add(new ErrorDTO
                    {
                        Code = validationFailure.Current.ErrorCode,
                        Property = validationFailure.Current.PropertyName,
                        Message = validationFailure.Current.ErrorMessage
                    });
                }
            }
            else
            {
                errors.Add(new ErrorDTO
                {
                    Code = CommonResource.VALIDATION_ERROR,
                    Message = validationException.Message
                });
            }
        }

        private static void BuildJsonExceptionError(JsonException jsonException, IList<ErrorDTO> errors)
        {
            var jsonExceptionErrors = ParseJsonExceptionMessage(jsonException.Message);

            if (jsonExceptionErrors.Any())
            {
                foreach (var error in jsonExceptionErrors)
                {
                    errors.Add(error);
                }
                return;
            }

            errors.Add(new ErrorDTO
            {
                Code = CommonResource.FIELD_REQUIRED,
                Message = jsonException.Message,
            });
        }

        /// <summary>
        /// BuildApiExceptionError
        /// </summary>
        /// <param name="apiException"></param>
        /// <param name="errors"></param>
        private static void BuildApiExceptionError(ApiException apiException, IList<ErrorDTO> errors)
        {
            if (apiException.Errors is null || !apiException.Errors.Any())
            {
                // Regular ApiException with code and message
                errors.Add(new ErrorDTO { Code = apiException.Code, Message = apiException.Message });
            }
            else
            {
                // ApiException with error list
                foreach (var error in apiException.Errors)
                {
                    errors.Add(new ErrorDTO { Code = error.Code, Message = error.Message, Property = error.Property });
                }
            }
        }

        public static IList<ErrorDTO> ParseJsonExceptionMessage(string jsonExceptionMessage)
        {
            var errors = new List<ErrorDTO>();

            var containsRequiredErrors = jsonExceptionMessage?.Contains("missing required");

            if (containsRequiredErrors is true)
            {
                // JsonException required properties message pattern: "JSON deserialization for type CLASS
                //    was missing required properties, including the following: Property1; Property2; Property3"
                var split = jsonExceptionMessage!.Split(':', ';');

                if (split.Length > 1)
                {

                    // Ignores first item describing the error, the next ones are the property names
                    for (var i = 1; i < split.Length; i++)
                    {
                        var property = split[i].Trim();
                        errors.Add(new ErrorDTO
                        {
                            Code = CommonResource.FIELD_REQUIRED,
                            Property = property,
                            Message = CommonLocalization.Get(CommonResource.FIELD_REQUIRED, property),
                        });
                    }
                }
            }

            return errors;
        }
    }
}
