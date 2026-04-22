# Fases do MVP: Unificação da API (opamenu-authentication ➜ opamenu-api)

## Contexto
Hoje o ecossistema está dividido em duas APIs principais:

- `opamenu-api`: API operacional (pedido, cardápio, mesa/comanda, pagamentos, etc.).
- `opamenu-authentication`: API de autenticação, RBAC (roles/permissões), multi-tenant e planos/assinaturas.

Para reduzir custo de infraestrutura no MVP, a estratégia é **consolidar toda a lógica do `opamenu-authentication` dentro do `opamenu-api`**, mantendo a separação por camadas/módulos no código para permitir futura extração em microserviços.

## Objetivo
- Uma única API responsável por:
  - autenticação (login/refresh/logout), emissão/validação de JWT
  - multi-tenant (tenant, produtos do tenant)
  - RBAC (usuários, roles, módulos, permissões)
  - planos e assinaturas (incluindo habilitação de módulos por tenant)
  - domínio operacional do OpaMenu (pedidos, pagamentos, etc.)

## Princípios (para não “virar um monolito caótico”)
- Manter **fronteiras internas** claras (pastas/namespaces e contratos) para futura extração.
- Definir **fonte única de verdade** para:
  - JWT/Refresh tokens
  - permissões e operações
  - módulos habilitados por tenant (entitlements)
- Garantir **compatibilidade de contrato** com os frontends (painel, cardápio, gestor) durante a migração.

## Fase 1 — Abstrair/migrar toda a regra do `opamenu-authentication` para o `opamenu-api`

### Resultado esperado (Definition of Done)
- `opamenu-api` oferece endpoints equivalentes aos do `opamenu-authentication` para:
  - `POST /api/auth/login`, `POST /api/auth/refresh`, `POST /api/auth/logout`, `GET /api/auth/permissions`
  - `api/plans`, `api/subscriptions`, `api/tenants`, `api/modules`, `api/roles`, `api/users`, etc.
- `opamenu-api` **não depende** de `Authentication:ExternalAuthUrl` e não faz proxy para a API antiga.
- O token JWT emitido contém os claims necessários para os apps (user, tenant, roles/permissões).
- O enforcement de permissões e de módulos habilitados do tenant acontece dentro do `opamenu-api`.
- `opamenu-authentication` pode ser desligado sem quebrar login/uso do sistema.

### Inventário do que precisa migrar
O `opamenu-authentication` possui:

- Autenticação e emissão/refresh/logout + endpoint de permissões do usuário:
  - [AuthController.cs](file:///d:/dev/opamenu-ecosistema/opamenu-authentication/UserEntry/Auth/AuthController.cs)
- RBAC e módulos:
  - controllers em `UserEntry/AccessControl/*`
- Multi-tenant + planos + assinaturas:
  - controllers em `UserEntry/MultiTenant/*` (ex.: [PlanController.cs](file:///d:/dev/opamenu-ecosistema/opamenu-authentication/UserEntry/MultiTenant/PlanController.cs))
- Regra importante de “entitlements por módulo”:
  - o `GetUserInfo` faz interseção entre permissões do usuário e módulos habilitados no tenant
  - [AuthenticationService.cs](file:///d:/dev/opamenu-ecosistema/opamenu-authentication/Core/Application/Implementation/AuthenticationService.cs#L327-L346)

O `opamenu-api` hoje:

- Valida JWT localmente e faz refresh via serviço externo:
  - [ExternalAuthenticationService.cs](file:///d:/dev/opamenu-ecosistema/opamenu-api/OpaMenu.Infrastructure/Authentication/ExternalAuthenticationService.cs)
  - [AuthController.cs](file:///d:/dev/opamenu-ecosistema/opamenu-api/OpaMenu.Web/UserEntry/Auth/AuthController.cs)

### Estratégia recomendada (mínimo risco)
1) **Trazer o código do `opamenu-authentication` para dentro do `opamenu-api` sem “reescrever tudo”**
   - preservar classes/serviços/repositórios, mudando apenas namespaces e registros de DI.
2) **Unificar os DbContexts necessários no `opamenu-api`**
   - `AccessControlDbContext` e `MultiTenantDbContext` já existem no repositório (via `opamenu-commons`).
   - padronizar connection strings e migrations.
3) **Substituir o `ExternalAuthenticationService` por uma implementação interna**
   - o `opamenu-api` deve emitir tokens e gerir refresh tokens internamente.
4) **Manter compatibilidade de endpoints e payloads**
   - os frontends usam o contrato atual; alterar contrato agora aumenta risco de lançamento.

### Passos sugeridos (ordem de execução)

#### 1. Congelar contrato atual
- Mapear endpoints consumidos por:
  - `opamenu-painel`, `opamenu-cardapio`, `opamenu-gestor`, `access-control-web`, `opamenu-business`
- Congelar paths/payloads e headers/claims necessários.

#### 2. Consolidar autenticação (JWT + refresh)
- Trazer para o `opamenu-api`:
  - geração de JWT + refresh token + revogação
  - endpoint de permissões (`/api/auth/permissions`)
- Ajustar o `opamenu-api` para parar de chamar `Authentication:ExternalAuthUrl`.

#### 3. Consolidar RBAC + multi-tenant + planos/assinaturas
- Migrar controllers e services de:
  - módulos, roles, users, access groups
  - tenants, plans, subscriptions, tenant products
- Garantir que o “entitlement por módulo” continue válido:
  - módulos habilitados do tenant devem limitar permissões retornadas e também limitar acesso em runtime.

#### 4. Ajustar enforcement no `opamenu-api`
- Unificar a política de autorização:
  - `MapPermission(module, operation)`
  - validação de tenant e validação de módulo habilitado para o tenant
- Evitar “UI-only gating”: o backend deve bloquear.

#### 5. Desligar o `opamenu-authentication`
- Atualizar configurações e deployment removendo o serviço.
- Smoke tests do fluxo completo:
  - login ➜ carregar permissões ➜ usar painéis ➜ criar pedidos ➜ atualizar status ➜ gerar PIX

### Riscos e como mitigar
- **Risco: quebrar login/claims**
  - mitigar mantendo os mesmos claims e rotas inicialmente.
- **Risco: migrations e dados divergentes**
  - mitigar com ambiente de staging e migração controlada (dump/restore ou migrations consistentes).
- **Risco: monolito sem fronteiras**
  - mitigar isolando código em módulos internos (Auth/AccessControl/MultiTenant) e contratos.

## Fase 2 — Correções para lançamento
Todas as correções críticas e requisitos de segurança/robustez do lançamento estão detalhadas no documento:

- [Correções para o lançamento.md](file:///d:/dev/opamenu-ecosistema/Correções%20para%20o%20lançamento.md)

