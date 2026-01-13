namespace Authenticator.API.Core.Domain.AccessControl.Modules.DTOs
{
    public class ModuleBasicDTO
    {
        public Guid Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public List<string> Operations { get; set; } = new();
    }
}
