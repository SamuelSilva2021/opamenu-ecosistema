using System;
using System.Collections.Generic;

namespace Authenticator.API.Core.Domain.AccessControl.Roles.DTOs;

public class SimplifiedModuleDTO
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> AvailableActions { get; set; } = new List<string> { "CREATE", "READ", "UPDATE", "DELETE" };
}
