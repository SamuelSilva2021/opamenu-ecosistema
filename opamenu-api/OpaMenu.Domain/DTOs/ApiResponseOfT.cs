using System.Linq;

namespace OpaMenu.Domain.DTOs
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public string? Error { get; set; }
        public string[]? Errors { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public static ApiResponse<T> SuccessResponse(T data, string message = "Operação realizada com sucesso")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Message = message
            };
        }

        public static ApiResponse<T> ErrorResponse(string error, T? data = default)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Error = error,
                Data = data
            };
        }

        public static ApiResponse<T> ErrorResponse(string[] errors, T? data = default)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Errors = errors,
                Error = errors?.FirstOrDefault(),
                Data = data
            };
        }

        public static ApiResponse<T> BadRequest(string error, T? data = default)
        {
            return ErrorResponse(error, data);
        }
    }
}
