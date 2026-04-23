# Implementação de Autenticação JWT - OpaMenu API

## 📋 Resumo

Foi implementada a autenticação JWT na API, com emissão e validação de tokens **na própria `opamenu-api`** (login/refresh/logout e contexto do usuário). A implementação mantém o modelo de claims (tenant/roles/permissões) para suportar RBAC e multi-tenant.

## 🔐 Fluxo de Autenticação

```mermaid
sequenceDiagram
    participant Cliente as Frontend/Cliente
    participant API as opamenu-api
    
    Cliente->>API: POST /api/auth/login (usernameOrEmail, password)
    API->>API: Validar credenciais
    API->>API: Gerar JWT Token + Refresh Token
    API->>Cliente: { accessToken, refreshToken, ... }
    
    Cliente->>API: Request com Authorization: Bearer {token}
    API->>API: Validar JWT Token
    API->>API: Extrair claims do usuário
    API->>Cliente: Response autorizada
```

## 🛠️ Componentes Implementados

### 1. **Configuração JWT**
- **Arquivo**: `appsettings.json` / `appsettings.Development.json`
- **Configurações**:
  ```json
  {
    "Authentication": {
      "JwtSecret": "S2B7aeyc4VTzaqFWeELDygnSD92PdVna5uSkdnkP3FFHUcmGgMENuUkQPMNJ9WGA",
      "JwtIssuer": "Authenticator.API",
      "JwtAudience": "opamenu-ecosystem"
    }
  }
  ```

### 2. **Serviços de Autenticação**

#### **IAuthenticationService**
```csharp
public interface IAuthenticationService
{
    Task<bool> ValidateTokenAsync(string token);
    Task<ClaimsPrincipal> GetClaimsFromTokenAsync(string token);
    Task<string> RefreshTokenAsync(string refreshToken);
}
```

#### **LocalAuthenticationService**
- Valida tokens JWT localmente
- Extrai claims dos tokens
- Refresh token delega para o serviço interno de autenticação

#### **ICurrentUserService**
```csharp
public interface ICurrentUserService
{
    string UserId { get; }
    string UserName { get; }
    string Email { get; }
    bool IsAuthenticated { get; }
    IEnumerable<string> Roles { get; }
    string TenantId { get; }
    string TenantSlug { get; }
    IEnumerable<string> Permissions { get; }
    IEnumerable<string> AccessGroups { get; }
}
```

#### **CurrentUserService**
- Fornece acesso ao contexto do usuário autenticado
- Extrai informações de claims do JWT
- Suporte a multi-tenancy

### 3. **Configuração do Pipeline**
- JWT Bearer Authentication configurado
- Middleware de autenticação e autorização habilitados
- Swagger configurado com suporte a JWT

### 4. **Controller de Teste**
**AuthTestController** com endpoints:
- `GET /api/authtest/public` - Público (sem autenticação)
- `GET /api/authtest/protected` - Protegido (requer autenticação)
- `GET /api/authtest/admin-only` - Restrito (requer role admin)
- `POST /api/authtest/validate-token` - Validação manual de token

## 🔧 Como Usar

### 1. **Obter Token de Acesso**

Faça login na própria API:
```bash
POST http://localhost:5072/api/auth/login
Content-Type: application/json

{
  "usernameOrEmail": "admin@teste.com",
  "password": "senha123"
}
```

Resposta:
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "base64refreshtoken...",
  "expiresIn": 1800
}
```

### 2. **Usar Token na API**

Incluir o token no header Authorization:
```bash
GET http://localhost:5072/api/authtest/protected
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### 3. **Swagger UI**

1. Acesse: http://localhost:5072/swagger
2. Clique em "Authorize"
3. Insira apenas o token (sem "Bearer ")
4. Teste endpoints protegidos

## 📝 Claims Suportadas

O sistema extrai automaticamente as seguintes claims dos tokens:

| Claim | Descrição | Exemplo |
|-------|-----------|---------|
| `user_id` ou `sub` | ID do usuário | `"123e4567-e89b-12d3-a456-426614174000"` |
| `username` ou `name` | Nome de usuário | `"admin"` |
| `email` | Email do usuário | `"admin@teste.com"` |
| `tenant_id` | ID do tenant | `"tenant-uuid"` |
| `tenant_slug` | Slug do tenant | `"pizzaria-bella"` |
| `role` | Roles do usuário | `["admin", "manager"]` |
| `permission` | Permissões específicas | `["create_products", "read_orders"]` |
| `access_group` | Grupos de acesso | `["admins", "operators"]` |

## 🚀 Endpoints de Teste

### Público (sem autenticação)
```bash
GET http://localhost:5072/api/authtest/public
```

### Protegido (requer token válido)
```bash
GET http://localhost:5072/api/authtest/protected
Authorization: Bearer {token}
```

### Restrito por Role
```bash
GET http://localhost:5072/api/authtest/admin-only
Authorization: Bearer {token}
# Requer role "admin" no token
```

### Validação Manual de Token
```bash
POST http://localhost:5072/api/authtest/validate-token
Content-Type: application/json

{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

## 🔒 Controladores Protegidos

Exemplo de como proteger um controller:

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize] // Requer autenticação
public class ProductsController : ControllerBase
{
    private readonly ICurrentUserService _currentUser;
    
    public ProductsController(ICurrentUserService currentUser)
    {
        _currentUser = currentUser;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        // Acesso ao contexto do usuário atual
        var userId = _currentUser.UserId;
        var tenantId = _currentUser.TenantId;
        
        // Lógica filtrada por tenant...
        
        return Ok(products);
    }
    
    [HttpPost]
    [Authorize(Roles = "admin,manager")] // Requer roles específicas
    public async Task<IActionResult> CreateProduct([FromBody] ProductDto product)
    {
        // Apenas admins e managers podem criar produtos
        return Ok();
    }
}
```

## ⚙️ Configuração de Desenvolvimento

### URLs dos Serviços
- **opamenu-api**: http://localhost:5072
- **Swagger UI**: http://localhost:5072/swagger

### Variáveis de Ambiente
```bash
# Para produção, configure via environment variables
Authentication__JwtSecret=sua-chave-secreta-aqui
Authentication__JwtIssuer=Authenticator.API
Authentication__JwtAudience=opamenu-ecosystem
```

## 🧪 Teste Completo

1. **Inicie a opamenu-api** (porta 5072)
2. **Faça login** em `/api/auth/login`
3. **Use o token** no header `Authorization: Bearer {token}`
4. **Verifique** no Swagger UI

## 📚 Arquivos Modificados

### Criados/Modificados:
- `OpaMenu.Web/appsettings.json` - Configuração JWT
- `OpaMenu.Web/appsettings.Development.json` - Configuração JWT dev
- `OpaMenu.Web/Program.cs` - Pipeline de autenticação
- `OpaMenu.Web/Extensions/ServiceCollectionExtensions.cs` - Extensões JWT
- `OpaMenu.Domain/Interfaces/IAuthenticationService.cs` - Interface
- `OpaMenu.Infrastructure/Authentication/LocalAuthenticationService.cs` - Implementação
- `OpaMenu.Infrastructure/Authentication/AuthService.cs` - Emissão de tokens e refresh tokens
- `OpaMenu.Infrastructure/Authentication/CurrentUserService.cs` - Serviço de usuário

## ✅ Status da Implementação

- [x] Configuração JWT Bearer Authentication
- [x] Emissão e validação de tokens JWT na própria API
- [x] Extração de claims e contexto do usuário
- [x] Suporte a multi-tenancy via claims
- [x] Middleware de autenticação e autorização
- [x] Configuração do Swagger com JWT
- [x] Controllers de teste e exemplos
- [x] Documentação completa
- [x] Testes de integração funcionais
- [x] **Autorização adicionada em todos os controllers principais**

## 🔒 Controllers com Autorização

### Protegidos com `[Authorize]` (requer autenticação):
- ✅ **ProductsController** - Gerenciamento de produtos
- ✅ **CategoriesController** - Gerenciamento de categorias
- ✅ **OrdersController** - Gerenciamento de pedidos
- ✅ **AddonsController** - Gerenciamento de adicionais
- ✅ **AddonGroupsController** - Grupos de adicionais
- ✅ **ProductAddonGroupsController** - Relação produtos-grupos
- ✅ **PaymentsController** - Gerenciamento de pagamentos
- ✅ **OrderQueueController** - Fila de pedidos
- ✅ **FilesController** - Upload de arquivos (download público)

### Protegidos com `[Authorize(Roles = "admin")]` (apenas administradores):
- ✅ **MigrationController** - Operações de banco de dados

### Públicos com `[AllowAnonymous]`:
- ✅ **AuthTestController** endpoints `/public` e `/validate-token`
- ✅ **FilesController** endpoints de download (`/url`, `/variants`)
- ✅ **Health Check** endpoints (`/health`, `/api/test`, `/signalr/test`)

## 🔐 Níveis de Autorização Implementados

### 1. **Autenticação Básica** - `[Authorize]`
```csharp
[Authorize]
public class ProductsController : ControllerBase
{
    // Requer token JWT válido
    // Qualquer usuário autenticado pode acessar
}
```

### 2. **Autorização por Roles** - `[Authorize(Roles = "role")]`
```csharp
[Authorize(Roles = "admin")]
public class MigrationController : ControllerBase
{
    // Apenas usuários com role "admin" podem acessar
}

[Authorize(Roles = "admin,manager")]
public async Task<IActionResult> CreateProduct()
{
    // Admins ou managers podem criar produtos
}
```

### 3. **Endpoints Públicos** - `[AllowAnonymous]`
```csharp
[AllowAnonymous]
public async Task<IActionResult> GetFileUrl()
{
    // Acesso público (sem autenticação)
}
```

## 🎯 Próximos Passos

1. **Aplicar enforcement de permissões e módulos por tenant em todos os endpoints sensíveis**
2. **Adicionar logs de auditoria para login/refresh/logout (sem registrar dados sensíveis)**
3. **Configurar rate limiting por usuário/tenant**
4. **Migrar segredos para variáveis de ambiente/secret store**
5. **Formalizar contrato de eventos (SignalR) e permissões**

---

**Implementação concluída com sucesso!**

A `opamenu-api` emite e valida tokens JWT internamente, mantendo o contexto multi-tenant e suporte a RBAC.
