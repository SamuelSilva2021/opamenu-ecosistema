# Access Control Web - Guia de Teste

## ✅ Status Atual (Modelo 3 Níveis)

A aplicação foi refatorada para o modelo **User -> Role -> Permission**, removendo a complexidade de Grupos de Acesso e Operações granulares.

- URL da API: `https://localhost:7019`
- Endpoint de login: `https://localhost:7019/api/auth/login`

## 🚀 Como Testar

### 1. Inicie sua API
Certifique-se de que sua API está rodando em `https://localhost:7019`

### 2. Inicie o Frontend
```bash
cd access-control-web
npm run dev
```
A aplicação estará disponível em: `http://localhost:5173/`

### 3. Teste o Login
1. Acesse `http://localhost:5173/`
2. Será redirecionado para a tela de login
3. Preencha credenciais válidas.
4. O sistema agora carrega o `role` diretamente no `userInfo`.

## 📋 Estrutura de Request/Response (Novo Modelo)

### Response Esperada (API → Frontend)
```json
{
  "success": true,
  "data": {
    "accessToken": "eyJ...",
    "userInfo": {
      "id": "guid-do-usuario",
      "fullName": "Nome Sobrenome",
      "email": "usuario@email.com",
      "role": {
        "id": "guid-do-role",
        "name": "Administrador",
        "permissions": [
          {
            "module": "USER_MODULE",
            "actions": ["READ", "CREATE", "UPDATE", "DELETE"]
          }
        ]
      }
    }
  }
}
```

## 📊 Funcionalidades Implementadas

1. ✅ **Auth Store**: Persistência do `role` e `permissions` simplificados.
2. ✅ **Permission Gate**: Verificação robusta usando `hasPermission(module, action)`.
3. ✅ **Gestão de Roles**: Interface de matriz para selecionar ações por módulo.
4. ✅ **Dashboard**: Links rápidos para Usuários, Perfis e Módulos.
5. ✅ **Limpeza Arquitetural**: Remoção de GroupTypes, AccessGroups e Operations.

## 📝 Arquitetura

- **Frontend**: React 19 + TypeScript + Material-UI 7
- **Estado**: Zustand + React Query
- **Roteamento**: React Router v7
- **RBAC**: 3 níveis (Flat: User -> Role -> Permission)

### Estrutura de Pastas
```
src/
├── features/auth/          # Autenticação
├── features/roles/         # Gestão de Perfis e Permissões (Matriz)
├── features/users/         # Gestão de Usuários
├── features/modules/       # Configuração de Módulos
├── shared/stores/          # auth.store.ts e permission.store.ts
└── app/routes/             # AppRoutes protegidas
```

---

**🎯 Objetivo**: Garantir um sistema de permissões rápido, legível e de fácil manutenção para o ecossistema Opamenu.
