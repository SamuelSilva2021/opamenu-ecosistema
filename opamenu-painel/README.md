# 🍕 Opamenu Painel - Gestão para Restaurantes

Painel administrativo moderno para gestão de pedidos, cardápio e configurações do ecossistema **Opamenu**. Desenvolvido com **React**, **TypeScript**, **Vite** e **Shadcn/ui**.

## 🚀 Funcionalidades Principais

- **Gestão de Pedidos em Tempo Real**: Kanban board com atualizações instantâneas via SignalR.
- **Gerenciamento de Cardápio**:
  - Produtos, Categorias e Adicionais.
  - Grupos de Adicionais e Controle de Estoque.
- **Configurações da Loja**: Horários, Taxas de Entrega, Impressoras Térmicas.
- **Gestão de Usuários e Permissões**: Controle de acesso granular (RBAC).
- **Cupons e Fidelidade**: Ferramentas de marketing para retenção de clientes.

## 🏗 Arquitetura e Tecnologias

- **Framework**: React 18 + Vite
- **Linguagem**: TypeScript
- **Estado Global**: Zustand (Auth) + TanStack Query (Server State)
- **UI Components**: Shadcn/ui + Tailwind CSS
- **Comunicação Real-time**: @microsoft/signalr
- **HTTP Client**: Axios

### 📡 Comunicação em Tempo Real

O painel utiliza uma arquitetura híbrida de WebSocket + Polling para garantir que nenhum pedido seja perdido.
Para detalhes técnicos da implementação, consulte: [Arquitetura de Tempo Real](./docs/REALTIME_ARCHITECTURE.md).

## 🛠 Configuração do Ambiente

1. **Pré-requisitos**: Node.js 18+

2. **Instalação**:
   ```bash
   npm install
   ```

3. **Executar em Desenvolvimento**:
   ```bash
   npm run dev
   ```

4. **Variáveis de Ambiente**:
   Crie um arquivo `.env` na raiz baseado no `.env.example`:
   ```env
   VITE_API_URL=https://seu-backend.com/api
   ```

## 📦 Estrutura do Projeto

```
src/
├── components/     # Componentes Reutilizáveis (UI, Layout, Auth)
├── features/       # Módulos Funcionais (Orders, Products, Settings)
│   ├── orders/
│   ├── products/
│   └── ...
├── hooks/          # Custom Hooks (usePermission, useToast)
├── lib/            # Utilitários e Configurações (Axios, Utils)
├── services/       # Camada de Integração com API
│   ├── signalr.service.ts  # Gerenciador de WebSocket
│   └── ...
├── store/          # Zustand Stores
└── layouts/        # Layouts de Página (Dashboard, Auth)
```

## 🤝 Contribuição

Este projeto faz parte do ecossistema Opamenu. Siga os padrões de código estabelecidos (ESLint + Prettier) e utilize Conventional Commits.
