using System;
using System.Collections.Generic;

namespace Authenticator.API.Core.Domain.AccessControl.Roles.DTOs;

public class SimplifiedRoleDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<SimplifiedPermissionDTO> Permissions { get; set; } = [];
}

public class SimplifiedPermissionDTO
{
    public string Module { get; set; } = string.Empty;
    public List<string> Actions { get; set; } = [];
}
