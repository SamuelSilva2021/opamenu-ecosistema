# 🤖 Diretrizes e Arquitetura do Ecossistema Opamenu (AGENTS.md)

Este documento é a fonte primária de verdade e contexto para agentes de IA operando no **Antigravity IDE** dentro do repositório `opamenu-ecosistema`. Qualquer agente ou subagente trabalhando neste workspace deve seguir rigorosamente as orientações, restrições e padrões arquiteturais descritos abaixo.

---

## 1. Visão Geral do Produto e Ecossistema

O **Opamenu** é uma plataforma completa de gestão para restaurantes, bares e delivery (SaaS multi-tenant), desenhada para suportar desde pequenas operações até grandes redes.

### Pilares de Negócio e Funcionalidades Principais
* **Atendimento Presencial:** Mesas e comandas individuais com divisão de conta e controle por cliente.
* **Aplicativo do Garçom:** Lançamento rápido de pedidos em tempo real.
* **App Gestor (Mobile):** Gestão operacional em tempo real nativa (Flutter).
* **Cardápio Digital & Delivery:** Cardápio interativo via web com pagamentos Pix automatizados (conciliação via Webhook).
* **PDV / Caixa & Impressão Local:** Aplicação desktop (WPF) para controle de caixa, sangrias, suprimentos e impressão térmica local (ESC/POS) com baixa latência.
* **KDS (Kitchen Display System):** Gestão de pedidos na cozinha com roteamento por setor.

---

## 2. Mapa do Monorepo e Tecnologias

O repositório é composto por subsistemas especializados organizados por pastas:

| Diretório | Tecnologia Principal | Papel no Ecossistema |
| :--- | :--- | :--- |
| **`opamenu-api/`** | .NET 8 (C#), EF Core, PostgreSQL | Backend central, APIs RESTful, autenticação JWT, regras de negócio multi-tenant. |
| **`opamenu-commons/`** | .NET 8 (C#) | Bibliotecas compartilhadas, migrações de banco (`Opamenu.DatabaseMigrator`) e contratos. |
| **`opamenu-shared-infrastructure/`** | .NET 8 (C#) | Infraestrutura compartilhada entre serviços e aplicações desktop/backend. |
| **`opamenu-gestor/`** | Flutter / Dart (iOS/Android/Web) | Aplicativo operacional do dono/gerente do restaurante. |
| **`opamenu-garcom-mobile/`** | Flutter / Dart (iOS/Android) | Aplicativo móvel para garçons registrarem pedidos em mesas/comandas. |
| **`opamenu-desktop/`** | WPF / .NET (C# / XAML) | PDV local Windows, contingência offline e integração com impressoras térmicas. |
| **`opamenu-cardapio/`** | React, Vite, TypeScript, Tailwind | Cardápio digital voltado para o cliente final (pedidos mesa/delivery). |
| **`opamenu-business/`** | React, Vite, TypeScript, Tailwind | Portal comercial, landing pages e onboarding de novos lojistas. |
| **`opamenu-painel/`** | React, Vite, TypeScript, Tailwind | Dashboard administrativo web para operadores e lojistas. |
| **`access-control-web/`** | React, Vite, TypeScript, Tailwind | Portal de controle de acesso, identidades e permissões (RBAC). |
| **`scripts/`** | Shell / PowerShell / SQL | Scripts de infraestrutura, banco de dados e suporte ao deploy. |

---

## 3. Diretrizes de Arquitetura e Padrões de Código

### 3.1. Backend (`opamenu-api`)
* **Clean Architecture / DDD:**
  * `Opamenu.Domain`: Entidades, enums, contratos de repositórios e regras de domínio puras (sem dependências de frameworks).
  * `Opamenu.Application`: DTOs, casos de uso, serviços de aplicação e validações.
  * `Opamenu.Infrastructure`: Implementações de banco (Entity Framework Core), repositórios concretos, integrações de pagamento (Pix/EFI/Mercado Pago) e serviços externos.
  * `Opamenu.Api`: Controllers REST, middlewares (tratamento global de erros, multi-tenancy, logs) e injeção de dependências.
* **Multi-tenancy:** Toda consulta e persistência de dados de restaurante **deve** respeitar o isolamento por `TenantId` / `RestaurantId`.
* **Banco de Dados & Migrations:** Nunca faça alterações manuais de schema sem a respectiva migration via EF Core / `Opamenu.DatabaseMigrator`.

### 3.2. Clientes Mobile (`opamenu-gestor` e `opamenu-garcom-mobile`)
* **Padrões Flutter:**
  * Separar camadas de UI (Widgets), Gerenciamento de Estado e Serviços de API (`http` / `dio`).
  * Tratamento de falhas de conexão de rede gracioso: o garçom e o gestor precisam receber feedbacks claros em caso de instabilidade.
  * Layout responsivo para telas compactas de celulares até tablets (usados frequentemente em balcões).

### 3.3. Frontends Web (`React + Vite + TypeScript`)
* **Componentização e Design System:**
  * Utilizar **Tailwind CSS** para estilização utilitária.
  * Ícones padronizados via `lucide-react`.
  * Manter tipagem estrita no TypeScript (evitar o uso de `any`).
  * Centralizar chamadas de API em serviços com interceptors de token e tratamento unificado de erros HTTP.

### 3.4. Aplicação Desktop (`opamenu-desktop`)
* **WPF / MVVM:**
  * Manter separação estrita entre `Views` (XAML), `ViewModels` e `Services`.
  * Atenção máxima à estabilidade do driver de impressão térmica local (ESC/POS) e spoolers.

### 3.5. Comentários no Código
* Não adicionar comentários (`//`, `///`, `/* */`) em código novo ou alterado, em nenhuma linguagem do monorepo.
* Não remover comentários já existentes, a menos que solicitado.

---

## 4. Regras Críticas de Compatibilidade Cross-Service

> [!WARNING]
> Como a `opamenu-api` atende simultaneamente clientes Web, Mobile (Flutter) e Desktop (WPF):
> 1. **Não quebre contratos de API existentes:** Adicione novos campos opcionais em vez de renomear ou remover propriedades de DTOs já em produção.
> 2. **Validação de Impacto:** Ao alterar um endpoint no backend, verifique quais clientes o utilizam antes de aprovar a alteração.
> 3. **Consistência de Enums e Status:** Status de pedidos (`Pendente`, `EmPreparo`, `Pronto`, `Entregue`, `Cancelado`) devem permanecer sincronizados em todos os clientes.

---

## 5. Comandos de Build, Execução e Verificação

### Backend (.NET 8)
```bash
# Na pasta opamenu-api
dotnet build Opamenu.sln
dotnet test
```

### Frontends Web (executar dentro da pasta do respectivo app)
```bash
npm install
npm run lint
npm run build
```

### Mobile (Flutter)
```bash
# Na pasta opamenu-gestor ou opamenu-garcom-mobile
flutter pub get
flutter analyze
flutter test
```

---

## 6. Convenções de Git e Commits

Seguir o padrão **Conventional Commits**:
* `feat:` Nova funcionalidade (ex: `feat(api): adiciona rota de estorno de comanda`)
* `fix:` Correção de bug (ex: `fix(gestor): ajusta cálculo do total da comanda`)
* `refactor:` Refatoração sem alteração de comportamento
* `docs:` Alterações em documentação
* `chore:` Atualizações de dependências, builds ou scripts

---

## 7. Navegação de Documentações Internas

A documentação fica junto de cada subprojeto:
* [Plano de Escalabilidade da API](opamenu-api/docs/Scalability-Improvements-Plan.md)
* [Docs do Access Control](access-control-web/docs/)
* [Docs do Cardápio Digital](opamenu-cardapio/docs/)
