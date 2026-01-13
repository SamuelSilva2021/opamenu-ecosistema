using Microsoft.AspNetCore.Mvc;
using OpaMenu.Domain.DTOs;

namespace OpaMenu.Web.Utils
{
    public static class ResponseBuilder
    {
        public static async Task<ActionResult<ApiResponse<T>>> HandleRequestAsync<T>(
            Func<Task<T>> action,
            string successMessage = "Operação realizada com sucesso")
        {
            try
            {
                var result = await action();
                return new OkObjectResult(ApiResponse<T>.SuccessResponse(result, successMessage));
            }
            catch (ArgumentException ex)
            {
                return new BadRequestObjectResult(ApiResponse<T>.ErrorResponse(ex.Message));
            }
            catch (KeyNotFoundException ex)
            {
                return new NotFoundObjectResult(ApiResponse<T>.ErrorResponse(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                return new UnauthorizedObjectResult(ApiResponse<T>.ErrorResponse(ex.Message));
            }
            catch (Exception)
            {
                // Em produção, o ideal é não expor a mensagem da exceção diretamente no 500
                // mas logar o erro. Como este é um método estático, o log deve ser feito
                // via middleware de exceção ou passando o logger.
                return new ObjectResult(ApiResponse<T>.ErrorResponse("Erro interno do servidor"))
                {
                    StatusCode = 500
                };
            }
        }

        public static async Task<ActionResult<ApiResponse>> HandleRequestAsync(
            Func<Task> action,
            string successMessage = "Operação realizada com sucesso")
        {
            try
            {
                await action();
                return new OkObjectResult(ApiResponse.SuccessResponse(successMessage));
            }
            catch (ArgumentException ex)
            {
                return new BadRequestObjectResult(ApiResponse.ErrorResponse(ex.Message));
            }
            catch (KeyNotFoundException ex)
            {
                return new NotFoundObjectResult(ApiResponse.ErrorResponse(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                return new UnauthorizedObjectResult(ApiResponse.ErrorResponse(ex.Message));
            }
            catch (Exception)
            {
                return new ObjectResult(ApiResponse.ErrorResponse("Erro interno do servidor"))
                {
                    StatusCode = 500
                };
            }
        }
    }
}
