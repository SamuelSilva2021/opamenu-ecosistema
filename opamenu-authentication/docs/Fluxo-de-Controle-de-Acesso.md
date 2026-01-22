# Fluxo de Controle de Acesso e Autenticação (Multi-Tenant & Plan-Based)

Este documento descreve a arquitetura de segurança, autenticação e autorização do sistema Opamenu, com foco no modelo **Multi-Tenant** e na restrição de acesso baseada em **Planos de Assinatura**.

## 🎯 Visão Geral da Arquitetura

O sistema utiliza uma abordagem híbrida de **RBAC (Role-Based Access Control)** com **Feature Toggling por Tenant**.

A premissa fundamental é:
> **"Um usuário nunca pode ter permissão para acessar um módulo que seu Tenant não contratou, independente de seu cargo (Role)."**

### A Fórmula de Acesso
O acesso final de um usuário é calculado dinamicamente pela interseção:

```
Permissões Efetivas = (Permissões da Role do Usuário) ∩ (Módulos Ativos do Plano do Tenant)
```

---

## 🏗️ Entidades e Hierarquia

1.  **Tenant (Restaurante)**: A entidade raiz. Possui um **Plano** (ex: Basic, Premium).
2.  **TenantModule**: Módulos que o Tenant contratou (ex: `FINANCIAL`, `STOCK`, `ORDERS`).
3.  **GroupType**: Categorias de grupos (ex: `TENANT_ADMIN`, `WAITER`, `MANAGER`).
4.  **AccessGroup**: Grupos concretos dentro de um tenant (ex: "Garçons do Restaurante X").
5.  **Role**: Papéis funcionais (ex: `ADMIN` - tem acesso a tudo; `WAITER` - só pedidos).
6.  **UserAccount**: O usuário final.

---

## 🚀 Fluxo de Registro de Tenant (Onboarding)

Quando um novo restaurante se registra (`AddTenantAsync`), o sistema executa automaticamente:

1.  **Criação do Tenant**: Salva os dados básicos e slug.
2.  **Definição de Módulos**: Baseado no plano escolhido, popula a tabela `TenantModules`.
    *   *Ex: Plano Basic -> Adiciona apenas módulos `ORDERS` e `CATALOG`.*
3.  **Setup de Permissões Iniciais (`ConfigureInitialPermissionsAsync`)**:
    *   Busca o `GroupType` com código **`TENANT_ADMIN`**.
    *   Busca a `Role` template **`ADMIN`** (que possui acesso a *todos* os módulos do sistema).
    *   Cria um **AccessGroup Dinâmico** exclusivo para o tenant:
        *   Nome: `Administradores - {Nome do Tenant}`
        *   Código: `GRP_ADMIN_{SLUG_DO_TENANT}` (Garante unicidade).
    *   VIncula: `User` -> `AccessGroup` -> `Role ADMIN`.

---

## 🔒 Fluxo de Autenticação e Autorização (Runtime)

### 1. Login e Token JWT
O usuário faz login e recebe um JWT contendo `sub` (UserId) e `tenant` (Slug). O token **NÃO** contém a lista completa de permissões para manter o payload leve.

### 2. Recuperação de Informações (`GetUserInfo`)
Quando o frontend (ou uma API protegida) solicita as permissões do usuário:

1.  **Carregamento de Roles**: O sistema carrega todas as permissões atreladas às Roles do usuário.
    *   *Cenário*: O usuário é Admin, então sua Role diz que ele pode acessar `FINANCIAL`, `STOCK`, `ORDERS`.
2.  **Validação de Contrato (Tenant Modules)**:
    *   O sistema verifica quais módulos o Tenant possui ativos no banco (`TenantModuleRepository`).
    *   *Cenário*: O Tenant é plano "Basic" e só tem `ORDERS`.
3.  **Filtragem (Interseção)**:
    *   O `AuthenticationService` remove da lista do usuário qualquer permissão ligada a módulos que o Tenant **não** possui.
    *   *Resultado*: O usuário recebe apenas permissões de `ORDERS`. As permissões de `FINANCIAL` e `STOCK` são suprimidas.

### 3. Proteção de Rotas (`PermissionAuthorizationFilter`)
Para garantir segurança no Backend (caso alguém tente forçar uma requisição):

*   Toda Action crítica é decorada com `[MapPermission(Module = "FINANCIAL", Operation = "Read")]`.
*   O filtro intercepta a requisição.
*   Verifica se o usuário tem a permissão.
*   **Crucial**: Como a lista de permissões do usuário já foi filtrada pelo plano do tenant no passo anterior, o acesso é negado (`403 Forbidden`) se o plano não cobrir aquele módulo.

---

## 💡 Exemplos Práticos

### Cenário A: Upgrade de Plano
1.  **Situação**: Tenant "Pizza Place" está no plano **Basic** (sem Financeiro).
2.  **Admin**: Tem Role `ADMIN`. Tenta acessar `/api/financial/reports`.
3.  **Resultado**: Acesso Negado (O módulo `FINANCIAL` não existe para o tenant).
4.  **Ação**: Tenant faz upgrade para **Premium**.
5.  **Sistema**: Insere `FINANCIAL` na tabela `TenantModules`.
6.  **Imediato**: No próximo login/refresh, a interseção `ADMIN ∩ Premium` agora inclui `FINANCIAL`. O acesso é liberado sem precisar editar o usuário ou a role.

### Cenário B: Funcionário Limitado
1.  **Situação**: Tenant **Premium** (tem tudo).
2.  **Usuário**: Garçom (Role `WAITER`).
3.  **Acesso**: Tenta acessar Financeiro.
4.  **Lógica**:
    *   Tenant tem módulo Financeiro? **Sim**.
    *   Role `WAITER` tem permissão Financeiro? **Não**.
5.  **Resultado**: Acesso Negado (Falta de privilégio da Role).

---

## 🛠️ Manutenção e Extensibilidade

*   **Novos Módulos**: Ao criar um novo módulo no sistema, basta adicioná-lo à Role `ADMIN` via seed e aos planos correspondentes. Nenhuma migração de dados de usuário é necessária.
*   **Personalização**: Se um tenant específico precisar de uma exceção (ex: um módulo beta), basta adicionar o registro na `TenantModules` manualmente para aquele TenantId.

## 📄 Referências de Código

*   **Setup Inicial**: `TenantService.ConfigureInitialPermissionsAsync`
*   **Lógica de Filtro**: `AuthenticationService.GetUserInfoAsync`
*   **Segurança Global**: `PermissionAuthorizationFilter.OnActionExecutionAsync`
*   **Entidades**: `TenantModuleEntity`, `AccessGroupEntity`
