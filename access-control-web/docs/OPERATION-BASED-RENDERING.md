# Renderização Baseada em Operações

## Visão Geral

Este documento descreve a implementação do sistema de renderização condicional baseado nas operações específicas que o usuário possui em cada módulo do sistema de controle de acesso.

## Conceito

Além do controle de rotas por módulos, agora implementamos controle granular de funcionalidades dentro de cada tela baseado nas operações disponíveis:

- **SELECT**: Visualizar dados (leitura)
- **INSERT**: Criar novos registros
- **UPDATE**: Editar registros existentes
- **DELETE**: Remover registros

## Arquitetura da Solução

### 1. Hook de Operações (`useOperationPermissions`)

Hook customizado que estende o `usePermissions` para verificar operações específicas:

```typescript
interface OperationPermissions {
  canRead: boolean;      // SELECT
  canCreate: boolean;    // INSERT  
  canUpdate: boolean;    // UPDATE
  canDelete: boolean;    // DELETE
  hasAnyOperation: (ops: OperationType[]) => boolean;
  hasAllOperations: (ops: OperationType[]) => boolean;
}
```

### 2. Componentes de Controle

#### `OperationGuard`
Componente wrapper que renderiza filhos apenas se o usuário tiver as operações necessárias:

```typescript
<OperationGuard module={ModuleKey.ACCESS_GROUP} operations={['UPDATE']}>
  <EditButton />
</OperationGuard>
```

#### `ConditionalRender`
Componente para renderização condicional mais complexa:

```typescript
<ConditionalRender
  module={ModuleKey.ACCESS_GROUP}
  renderIf={(permissions) => permissions.canUpdate && permissions.canDelete}
  fallback={<ReadOnlyView />}
>
  <AdminActions />
</ConditionalRender>
```

### 3. Hooks Específicos por Módulo

Para cada módulo principal, teremos hooks específicos:

```typescript
// Hook para ACCESS_GROUP
const useAccessGroupOperations = () => {
  return useOperationPermissions(ModuleKey.ACCESS_GROUP);
};

// Hook para USER_MODULE  
const useUserOperations = () => {
  return useOperationPermissions(ModuleKey.USER_MODULE);
};
```

## Implementação

### Estrutura de Arquivos

```
src/shared/
├── hooks/
│   ├── useOperationPermissions.ts     # Hook principal
│   └── operations/
│       ├── useAccessGroupOperations.ts
│       ├── useUserOperations.ts
│       └── index.ts
├── components/
│   └── permissions/
│       ├── OperationGuard.tsx
│       ├── ConditionalRender.tsx
│       └── index.ts
└── types/
    └── operation.types.ts
```

### Exemplo de Uso Prático

Na tela de Grupos de Acesso:

```typescript
const AccessGroupsPage = () => {
  const { canCreate, canUpdate, canDelete } = useAccessGroupOperations();

  return (
    <Container>
      <PageHeader>
        <Typography variant="h4">Grupos de Acesso</Typography>
        
        {/* Botão de criar - só aparece se pode INSERT */}
        <OperationGuard module={ModuleKey.ACCESS_GROUP} operations={['INSERT']}>
          <Button startIcon={<AddIcon />} onClick={handleCreate}>
            Novo Grupo
          </Button>
        </OperationGuard>
      </PageHeader>

      <DataGrid
        rows={groups}
        columns={[
          // Colunas básicas sempre visíveis
          { field: 'name', headerName: 'Nome' },
          { field: 'description', headerName: 'Descrição' },
          
          // Coluna de ações condicionais
          {
            field: 'actions',
            headerName: 'Ações',
            renderCell: (params) => (
              <Box>
                {/* Visualizar - sempre disponível se tem SELECT */}
                <IconButton onClick={() => handleView(params.row.id)}>
                  <VisibilityIcon />
                </IconButton>

                {/* Editar - só se tem UPDATE */}
                <OperationGuard module={ModuleKey.ACCESS_GROUP} operations={['UPDATE']}>
                  <IconButton onClick={() => handleEdit(params.row.id)}>
                    <EditIcon />
                  </IconButton>
                </OperationGuard>

                {/* Deletar - só se tem DELETE */}
                <OperationGuard module={ModuleKey.ACCESS_GROUP} operations={['DELETE']}>
                  <IconButton 
                    color="error"
                    onClick={() => handleDelete(params.row.id)}
                  >
                    <DeleteIcon />
                  </IconButton>
                </OperationGuard>
              </Box>
            )
          }
        ]}
      />
    </Container>
  );
};
```

## Padrões de Implementação

### 1. Renderização de Botões de Ação

```typescript
// ✅ Padrão recomendado
<OperationGuard module={ModuleKey.ACCESS_GROUP} operations={['DELETE']}>
  <DeleteButton />
</OperationGuard>

// ❌ Evitar verificação manual
{canDelete && <DeleteButton />}
```

### 2. Formulários Condicionais

```typescript
const { canUpdate } = useAccessGroupOperations();

return (
  <Form>
    <TextField
      value={name}
      onChange={handleNameChange}
      disabled={!canUpdate}  // Campo readonly se não pode editar
    />
    
    <OperationGuard module={ModuleKey.ACCESS_GROUP} operations={['UPDATE']}>
      <Button type="submit">Salvar</Button>
    </OperationGuard>
  </Form>
);
```

### 3. Menus Contextuais

```typescript
<Menu>
  <MenuItem onClick={handleView}>Visualizar</MenuItem>
  
  <OperationGuard module={ModuleKey.ACCESS_GROUP} operations={['UPDATE']}>
    <MenuItem onClick={handleEdit}>Editar</MenuItem>
  </OperationGuard>
  
  <OperationGuard module={ModuleKey.ACCESS_GROUP} operations={['DELETE']}>
    <MenuItem onClick={handleDelete}>Excluir</MenuItem>
  </OperationGuard>
</Menu>
```

## Considerações de Performance

1. **Memoização**: Todos os hooks utilizam `useMemo` para cache de resultados
2. **Lazy Loading**: Componentes só renderizam quando necessário
3. **Batch Updates**: Verificações agrupadas para reduzir re-renders

## Testes

### Testes Unitários
- Hook `useOperationPermissions`
- Componentes `OperationGuard` e `ConditionalRender`
- Hooks específicos por módulo

### Testes de Integração
- Cenários completos de permissões
- Interação entre módulos
- Fallbacks e estados de erro

## Migração

### Fase 1: Infraestrutura
- [ ] Implementar hooks base
- [ ] Criar componentes de controle
- [ ] Documentar padrões

### Fase 2: Aplicação
- [ ] Migrar tela de Grupos de Acesso
- [ ] Aplicar em demais telas do módulo
- [ ] Testes e validação

### Fase 3: Expansão
- [ ] Aplicar em todos os módulos
- [ ] Otimizações de performance
- [ ] Documentação final

## Benefícios

1. **Segurança**: Controle granular de acesso
2. **UX**: Interface adapta-se às permissões do usuário
3. **Manutenibilidade**: Lógica centralizada e reutilizável
4. **Escalabilidade**: Fácil adição de novos módulos/operações
5. **Testabilidade**: Componentes isolados e testáveis