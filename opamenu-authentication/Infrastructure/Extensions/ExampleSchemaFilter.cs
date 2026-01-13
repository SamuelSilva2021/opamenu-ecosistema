using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Any;
using Swashbuckle.AspNetCore.SwaggerGen;
using Authenticator.API.Core.Domain.Api;

namespace Authenticator.API.Infrastructure.Extensions;

/// <summary>
/// Filtro para adicionar exemplos aos schemas do Swagger
/// </summary>
public class ExampleSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(LoginRequest))
        {
            schema.Example = new OpenApiObject
            {
                ["usernameOrEmail"] = new OpenApiString("admin@opamenu.com.br"),
                ["password"] = new OpenApiString("Abc@123"),
            };
        }
        else if (context.Type == typeof(RefreshTokenRequest))
        {
            schema.Example = new OpenApiObject
            {
                ["refreshToken"] = new OpenApiString("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...")
            };
        }
        else if (context.Type == typeof(LoginResponse))
        {
            schema.Example = new OpenApiObject
            {
                ["accessToken"] = new OpenApiString("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c"),
                ["refreshToken"] = new OpenApiString("AbCdEf123456789RefreshToken..."),
                ["tokenType"] = new OpenApiString("Bearer"),
                ["expiresIn"] = new OpenApiInteger(1800)
            };
        }
        else if (context.Type == typeof(UserInfo))
        {
            schema.Example = new OpenApiObject
            {
                ["id"] = new OpenApiString("12345678-1234-1234-1234-123456789abc"),
                ["username"] = new OpenApiString("usuario@opamenu.com"),
                ["email"] = new OpenApiString("usuario@opamenu.com"),
                ["fullName"] = new OpenApiString("João da Silva"),
                ["accessGroups"] = new OpenApiArray { new OpenApiString("Operacionais") },
                ["roles"] = new OpenApiArray { new OpenApiString("User"), new OpenApiString("Cashier") },
                ["permissions"] = new OpenApiArray { new OpenApiString("orders.read"), new OpenApiString("products.read") }
            };
        }
    }
}