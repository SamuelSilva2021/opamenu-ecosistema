using OpaMenu.Domain.DTOs;

namespace OpaMenu.Application.Interfaces
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
