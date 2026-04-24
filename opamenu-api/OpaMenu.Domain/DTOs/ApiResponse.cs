using System.Linq;

namespace OpaMenu.Domain.DTOs
{
    public class ApiResponse : ApiResponse<object>
    {
        public static ApiResponse SuccessResponse(string message = "Operação realizada com sucesso")
        {
            return new ApiResponse
            {
                Success = true,
                Message = message
            };
        }

        public static ApiResponse ErrorResponse(string error)
        {
            return new ApiResponse
            {
                Success = false,
                Error = error
            };
        }

        public static ApiResponse ErrorResponse(string[] errors)
        {
            return new ApiResponse
            {
                Success = false,
                Errors = errors,
                Error = errors?.FirstOrDefault()
            };
        }
    }
}
