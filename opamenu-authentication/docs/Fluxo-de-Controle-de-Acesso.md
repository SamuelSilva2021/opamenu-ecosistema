# Fluxo de Controle de Acesso de Usuários

Este documento descreve o fluxo fim-a-fim para autenticação, autorização e gerenciamento de acesso dos usuários no sistema.

## Objetivo

- Garantir que cada usuário possua as permissões corretas dentro do tenant.
- Padronizar a criação, atribuição e manutenção de acessos.
- Apoiar a implementação de autenticação (JWT) e autorização por policies.

## Entidades e relações principais

- Tenant: escopo lógico de isolamento (multi-tenant).
- UserAccount: usuário do sistema, pertencente a um tenant.
- AccessGroup: agrupador de acesso dentro do tenant.
- AccountAccessGroup: relação Usuário ↔ Grupo de Acesso.
- Role: papel que representa um conjunto de permissões.
- RoleAccessGroup: relação Grupo de Acesso ↔ Papel.
- Permission: recurso/ação de alto nível.
- Operation: operação específica (CRUD/ações) atrelada a permissões.
- RolePermission: relação Papel ↔ Permissão (com operações).
- Application/Module: contexto funcional onde permissões e operações se aplicam.

Relação em cadeia na prática:

Usuário → AccountAccessGroup → RoleAccessGroup → Role → RolePermission → Permission/Operation → Módulos/Aplicações.

## Fluxo passo a passo

1) Criar usuário
- Criar UserAccount no tenant correto.
- Se aplicável, verificar e-mail para ativar a conta.
- Definir Status (ex.: Ativo/Inativo) e dados básicos (nome, e-mail, username).

2) Atribuir grupos de acesso ao usuário
- Associar o usuário a um ou mais AccessGroups via AccountAccessGroup.
- Sempre escopar por tenant.

3) Configurar papéis e vincular aos grupos
- Definir Roles (papéis) com base nas necessidades do tenant/aplicação.
- Vincular os Roles aos AccessGroups via RoleAccessGroup.
- O usuário herda papéis por estar nos grupos.

4) Definir permissões e operações e ligar aos papéis
- Criar Permissions e Operations.
- Relacioná-las aos Roles via RolePermission.
- Isso determina o que cada papel pode executar (ex.: ler/criar/editar/excluir) em determinados módulos/aplicações.

5) Autenticação (login) e emissão de tokens
- Usuário faz login com credenciais válidas.
- A API emite JWT com claims: userId, tenantId, e (de acordo com a estratégia) grupos/papéis/permissões.
- Atualizar LastLoginAt do usuário.

6) Autorização nas rotas
- Controllers utilizam [Authorize] e policies específicas.
- Policies checam claims de roles/permissões.
- Middleware/Contexto de Tenant garante que consultas e autorizações estejam dentro do tenant correto.

7) Manutenção e auditoria
- Gerenciar mudanças de acessos: adicionar/remover grupos, papéis e permissões.
- Desativar relações (IsActive) ou usuário quando necessário.
- Reset de senha, verificação de e-mail, auditoria de acesso (último login, logs).

## Considerações de multi-tenant

- Sempre incluir tenantId nas consultas de usuários, grupos e papéis.
- Evitar vazamento de dados entre tenants.
- Policies e validações devem considerar o tenant corrente (via TenantContext / claims).

## JWT e claims sugeridas

- sub (userId), tid (tenantId), name/email.
- Opcional: roles, permissions, groups (dependendo do tamanho do token e estratégia).
- Expiração adequada e refresh tokens quando aplicável.

## Exemplo de fluxo fim-a-fim

1. Admin do tenant cria o usuário João e ativa seu e‑mail.
2. João é adicionado aos grupos “Financeiro” e “Relatórios”.
3. “Financeiro” está vinculado aos papéis “FinanceManager” e “InvoiceViewer”.
4. “FinanceManager” possui permissões de “Faturas” com operações de ler/criar/editar; “InvoiceViewer” apenas ler.
5. João faz login, recebe JWT com claims do tenant e, conforme estratégia, papéis/permissões.
6. Ao acessar rotas de faturamento, policies verificam se João possui as permissões/operações exigidas.

## Boas práticas

- Manter o mínimo necessário de claims no JWT; para detalhes, consultar no backend por id e tenant.
- Usar caching quando apropriado (ex.: usuários por tenant), invalidando ao alterar acessos.
- Padronizar respostas com ResponseDTO e registrar eventos de auditoria.
- Garantir que enums (ex.: Status) sejam retornados como strings (configurado globalmente no JSON).

## Próximos passos

- Testes fim‑a‑fim (feliz/erro) nas rotas /auth e /api/*.
- Revisar policies e garantir cobertura completa dos módulos críticos.
- Documentar endpoints específicos de criação/atribuição (Users, AccessGroups, Roles, Permissions) por tenant.