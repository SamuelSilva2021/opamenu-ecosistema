namespace Authenticator.API.Core.Domain.Api
{
    public interface IResponseDTO
    {
        public bool Succeeded { get; }
        public int Code { get; }
        public IList<ErrorDTO> Errors { get; }
        public IDictionary<string, string> Headers { get; }
        dynamic? GetData();
        public string? RequestUrl { get; }
        public string? RequestBody { get; }
    }
}
