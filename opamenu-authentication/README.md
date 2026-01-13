# Authentication API

API de autenticação e autorização para o ecossistema Pedeja usando JWT e sistema RBAC.

## 📋 Visão Geral

Esta API fornece serviços centralizados de autenticação e autorização para todas as aplicações do ecossistema Pedeja. Implementa um sistema completo de controle de acesso baseado em papéis (RBAC) com suporte a multi-tenancy.

## 🏗️ Arquitetura

### Bancos de Dados
- **access_control_db**: Controle de acesso, usuários, roles e permissões
- **multi_tenant**: Gestão de tenants, planos e assinaturas

### Principais Componentes
- **Controllers**: Endpoints da API
- **Services**: Lógica de negócio
- **Data**: Contextos e configurações do Entity Framework
- **Entities**: Modelos de dados
- **Models**: DTOs para requisições/respostas

## 🚀 Endpoints Principais

### Autenticação
```
POST /api/auth/login          - Login de usuário
POST /api/auth/refresh        - Renovar tokens
POST /api/auth/logout         - Logout/revogar token
GET  /api/auth/validate       - Validar token atual
```

### Permissões
```
GET  /api/auth/permissions    - Obter permissões do usuário
GET  /api/auth/me            - Informações do usuário atual
```

### Sistema
```
GET  /health                 - Status de saúde da API
GET  /info                   - Informações da API
```

## 📝 Exemplo de Uso

### Login
```bash
curl -X POST "https://localhost:5001/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "usernameOrEmail": "usuario@exemplo.com",
    "password": "senha123",
    "tenantSlug": "tenant-exemplo"
  }'
```

### Resposta de Login
```json
{
  "success": true,
  "message": "Login realizado com sucesso",
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "base64RefreshToken...",
    "tokenType": "Bearer",
    "expiresIn": 1800,
    "user": {
      "id": "guid-usuario",
      "username": "usuario@exemplo.com",
      "email": "usuario@exemplo.com",
      "fullName": "João Silva",
      "accessGroups": ["admin"],
      "roles": ["administrator"],
      "permissions": ["users.read", "users.write"],
      "tenant": {
        "id": "guid-tenant",
        "name": "Tenant Exemplo",
        "slug": "tenant-exemplo"
      }
    }
  }
}
```

### Usando o Token
```bash
curl -X GET "https://localhost:5001/api/auth/permissions" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

## 🛠️ Configuração

### Pré-requisitos
- .NET 9.0 SDK
- PostgreSQL 14+
- Entity Framework Core Tools

### Variáveis de Ambiente / appsettings.json
```json
{
  "ConnectionStrings": {
    "AccessControlDatabase": "server=localhost;Database=access_control_db;port=5432;uid=postgres;pwd=admin;Encoding=UTF8;",
    "MultiTenantDatabase": "server=localhost;Database=multi_tenant;port=5432;uid=postgres;pwd=admin;Encoding=UTF8;"
  },
  "JwtSettings": {
    "SecretKey": "sua_chave_secreta_jwt_muito_segura_aqui_com_pelo_menos_256_bits",
    "Issuer": "Authenticator.API",
    "Audience": "pedeja-ecosystem",
    "AccessTokenExpirationMinutes": 30,
    "RefreshTokenExpirationDays": 7
  }
}
```

### Instalação e Execução

1. **Clone e navegue até o projeto**
   ```bash
   cd authentication-api/Authenticator.API
   ```

2. **Restaure os pacotes**
   ```bash
   dotnet restore
   ```

3. **Configure as strings de conexão no appsettings.json**

4. **Execute as migrations dos bancos de dados**
   ```bash
   # Primeiro execute os scripts SQL dos diretórios:
   # - scripts/access-control/
   # - scripts/multi-tenant/
   ```

5. **Execute a aplicação**
   ```bash
   dotnet run
   ```

6. **Acesse a documentação Swagger**
   ```
   https://localhost:5001
   ```

## 🔐 Sistema de Segurança

### JWT Tokens
- **Access Token**: Válido por 30 minutos (configurável)
- **Refresh Token**: Válido por 7 dias (configurável)
- **Claims incluídas**: user_id, username, email, tenant_id, roles, permissions

### RBAC (Role-Based Access Control)
- **Users**: Usuários do sistema
- **Access Groups**: Grupos de acesso organizacionais
- **Roles**: Papéis/funções específicas
- **Permissions**: Permissões granulares por módulo
- **Modules**: Funcionalidades/recursos do sistema

### Hierarquia de Acesso
```
User → AccountAccessGroup → AccessGroup → RoleAccessGroup → Role → Permission → Module
```

## 🏢 Multi-Tenancy

A API suporta multi-tenancy através de:
- **Tenant Isolation**: Dados isolados por tenant
- **Tenant Context**: Informações do tenant no JWT
- **Custom Domains**: Suporte a domínios personalizados
- **Subscription Management**: Controle de assinaturas e planos

## 📊 Monitoramento

### Health Checks
- `/health` - Status dos bancos de dados e serviços

### Logs
- **Console**: Logs em tempo real
- **File**: Arquivos diários em `/logs`
- **Serilog**: Estruturado e configurável

## 🚨 Tratamento de Erros

### Códigos de Status HTTP
- **200**: Sucesso
- **400**: Dados inválidos
- **401**: Não autorizado
- **404**: Recurso não encontrado
- **500**: Erro interno

### Formato de Resposta de Erro
```json
{
  "success": false,
  "message": "Descrição do erro",
  "errors": ["Lista de erros específicos"],
  "timestamp": "2025-09-25T10:30:00Z"
}
```

## 🔧 Desenvolvimento

### Estrutura do Projeto
```
Authenticator.API/
├── Controllers/        # Endpoints da API
├── Services/          # Lógica de negócio
│   ├── IAuthenticationService.cs
│   ├── AuthenticationService.cs
│   ├── IJwtTokenService.cs
│   └── JwtTokenService.cs
├── Data/             # Contextos EF Core
│   ├── AccessControlDbContext.cs
│   └── MultiTenantDbContext.cs
├── Entities/         # Modelos de dados
├── Models/           # DTOs
├── Extensions/       # Configurações de serviços
└── Program.cs        # Configuração da aplicação
```

### Comandos Úteis
```bash
# Build
dotnet build

# Executar testes
dotnet test

# Publicar
dotnet publish

# Verificar dependências
dotnet list package --outdated
```

## 📚 Documentação Adicional

- [Arquitetura Multi-Tenant](../docs/data-structure/Multi-Tenant-Implementation-Plan.md)
- [Implementação JWT](../docs/data-structure/JWT-Authentication-Implementation-Plan.md)
- [Scripts de Banco](../scripts/)

## 🤝 Contribuição

1. Siga os padrões de código C# e .NET
2. Implemente testes unitários
3. Documente APIs com XML comments
4. Use conventional commits
5. Mantenha cobertura de código > 80%

## 📄 Licença

Este projeto é propriedade da Pedeja e está sob licença proprietária.