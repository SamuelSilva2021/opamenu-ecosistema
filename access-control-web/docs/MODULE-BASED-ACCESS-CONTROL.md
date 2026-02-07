# Controle de Acesso Baseado em Módulos (Flat RBAC 3-Level)

## Visão Geral

Este documento descreve o sistema de controle de acesso simplificado (3 níveis). O modelo remove a hierarquia complexa de 9 níveis (**AccessGroups -> GroupTypes -> Roles -> Permissions -> Operations**) em favor de um modelo direto: **User -> Role -> Permission**.

Uma "Permissão" agora é a combinação de um `ModuleKey` e um conjunto de `Actions` (JSON).

## Estrutura de Permissões

### Resposta da API `/auth/login` ou `/user-info`

```json
{
  "userInfo": {
    "id": "uuid",
    "fullName": "Samuel Silva",
    "role": {
      "id": "uuid", 
      "name": "Administrador",
      "permissions": [
        {
          "module": "USER_MODULE",
          "actions": ["READ", "CREATE", "UPDATE", "DELETE"]
        },
        {
          "module": "ORDER_MODULE",
          "actions": ["READ"]
        }
      ]
    }
  }
}
```

### Mapeamento de Módulos

| Módulo Key | Descrição |
|------------|-----------|
| `USER_MODULE` | Gestão de Usuários e Perfis |
| `ORDER_MODULE` | Gestão de Pedidos do Restaurante |
| `TENANT_MODULE` | Configuração de Restaurantes/Tenants |
| `PRODUCT_MODULE` | Catálogo de Produtos e Categorias |

### Ações Padrão (Actions)

O sistema utiliza 4 ações fundamentais para cada módulo:
- `READ` - Visualizar listagem e detalhes
- `CREATE` - Cadastrar novos registros
- `UPDATE` - Editar registros existentes
- `DELETE` - Remover registros (normalmente soft-delete)

## Implementação Frontend

### 1. Tipos TypeScript (`permission.types.ts`)

```typescript
export interface SimplifiedPermission {
  module: string;    // Ex: 'USER_MODULE'
  actions: string[]; // Ex: ['READ', 'CREATE']
}

export interface SimplifiedRole {
  id: string;
  name: string;
  permissions: SimplifiedPermission[];
}
```

### 2. Store de Permissões (`permission.store.ts`)

O store centraliza a lógica de verificação para evitar bugs distribuídos:

```typescript
const usePermissions = usePermissionStore(state => ({
  isAllowed: (module: string, action: string) => {
    // Busca permissões do role ativo e verifica se contém o módulo e a ação
  }
}));
```

### 3. Permission Gate (`PermissionGate.tsx`)

Componente utilitário para envolver partes da UI:

```tsx
<PermissionGate module="USER_MODULE" action="CREATE">
  <Button>Novo Usuário</Button>
</PermissionGate>
```

## Benefícios do Novo Modelo

1. **Performance**: O backend busca todas as permissões em uma única query SQL simples (Dapper) sem múltiplos JOINs.
2. **Manutenibilidade**: Menos tabelas no banco e menos código no frontend.
3. **UX**: Gestão de perfis através de uma matriz intuitiva de Módulo x Ação.
4. **Segurança**: Verificação centralizada e robusta.

## Considerações de Migração

Caso encontre código referenciando `accessGroups`, `operations` ou `roles` como array de strings, este é considerado **legado** e deve ser refatorado para usar o campo `role` direto no `userInfo`.