namespace Authenticator.API.Core.Domain.Api
{
    public class ResponseBuilder<T>
    {
        private readonly ResponseDTO<T> _response = new ResponseDTO<T>();

        private ResponseBuilder() { }

        public static ResponseBuilder<T> Ok(T data)
        {
            var builder = new ResponseBuilder<T>();
            builder._response.Succeeded = true;
            builder._response.Code = 200;
            builder._response.Data = data;
            return builder;
        }

        public static ResponseBuilder<T> Fail(params ErrorDTO[] errors)
        {
            var builder = new ResponseBuilder<T>();
            builder._response.Succeeded = false;
            builder._response.Code = 400;
            builder._response.Errors = errors.ToList();
            return builder;
        }

        public ResponseBuilder<T> WithCode(int code)
        {
            _response.Code = code;
            return this;
        }

        public ResponseBuilder<T> WithHeader(string key, string value)
        {
            _response.Headers[key] = value;
            return this;
        }

        public ResponseBuilder<T> WithRequestInfo(string url, string? body = null, string? rawBody = null)
        {
            _response.RequestUrl = url;
            _response.RequestBody = body;
            _response.RawRequestBody = rawBody;
            return this;
        }

        public ResponseBuilder<T> WithException(Exception ex)
        {
            _response.Exception = ExceptionDTO.FromException(ex);
            return this;
        }

        public ResponseDTO<T> Build() => _response;
    }
}
