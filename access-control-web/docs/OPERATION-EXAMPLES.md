# Exemplos Práticos - Controle de Operações

## Exemplo 1: Botões de Ação Condicionais

```tsx
import { OperationGuard } from '../../shared/components/permissions';
import { ModuleKey } from '../../shared/types/permission.types';

const ActionButtons = () => {
  return (
    <Box sx={{ display: 'flex', gap: 2 }}>
      {/* Sempre visível para quem tem SELECT */}
      <Button variant="outlined">
        Visualizar
      </Button>

      {/* Só aparece se tem CREATE */}
      <OperationGuard module={ModuleKey.ACCESS_GROUP} operations={['CREATE']}>
        <Button variant="contained" color="primary">
          Criar Novo
        </Button>
      </OperationGuard>

      {/* Só aparece se tem UPDATE */}
      <OperationGuard module={ModuleKey.ACCESS_GROUP} operations={['UPDATE']}>
        <Button variant="contained" color="secondary">
          Editar
        </Button>
      </OperationGuard>

      {/* Só aparece se tem DELETE */}
      <OperationGuard module={ModuleKey.ACCESS_GROUP} operations={['DELETE']}>
        <Button variant="contained" color="error">
          Excluir
        </Button>
      </OperationGuard>
    </Box>
  );
};
```

## Exemplo 2: Menu Contextual com Operações

```tsx
const ContextMenu = ({ accessGroup }: { accessGroup: AccessGroup }) => {
  return (
    <Menu>
      {/* Sempre disponível */}
      <MenuItem>
        <ListItemIcon><ViewIcon /></ListItemIcon>
        <ListItemText>Visualizar Detalhes</ListItemText>
      </MenuItem>

      {/* Condicional por operação */}
      <OperationGuard module={ModuleKey.ACCESS_GROUP} operations={['UPDATE']}>
        <MenuItem>
          <ListItemIcon><EditIcon /></ListItemIcon>
          <ListItemText>Editar</ListItemText>
        </MenuItem>
      </OperationGuard>

      <OperationGuard module={ModuleKey.ACCESS_GROUP} operations={['DELETE']}>
        <MenuItem sx={{ color: 'error.main' }}>
          <ListItemIcon><DeleteIcon color="error" /></ListItemIcon>
          <ListItemText>Excluir</ListItemText>
        </MenuItem>
      </OperationGuard>
    </Menu>
  );
};
```

## Exemplo 3: Formulário com Campos Condicionais

```tsx
const AccessGroupForm = () => {
  const { canUpdate } = useAccessGroupOperations();

  return (
    <Form>
      {/* Campo sempre visível */}
      <TextField
        label="Nome"
        value={name}
        disabled={!canUpdate} // Readonly se não pode editar
      />

      {/* Campos administrativos - só para quem tem UPDATE */}
      <OperationGuard module={ModuleKey.ACCESS_GROUP} operations={['UPDATE']}>
        <TextField
          label="Código"
          value={code}
          onChange={handleCodeChange}
        />
        
        <FormControlLabel
          control={<Switch checked={isActive} onChange={handleToggle} />}
          label="Ativo"
        />
      </OperationGuard>

      {/* Botão de salvar - só para quem tem UPDATE */}
      <OperationGuard module={ModuleKey.ACCESS_GROUP} operations={['UPDATE']}>
        <Button type="submit" variant="contained">
          Salvar Alterações
        </Button>
      </OperationGuard>
    </Form>
  );
};
```

## Exemplo 4: Usando ConditionalRender para Lógica Complexa

```tsx
const AdminSection = () => {
  return (
    <ConditionalRender
      module={ModuleKey.ACCESS_GROUP}
      renderIf={(permissions) => 
        permissions.canUpdate && permissions.canDelete
      }
      fallback={
        <Alert severity="info">
          Seção administrativa requer permissões avançadas
        </Alert>
      }
    >
      <Paper sx={{ p: 2 }}>
        <Typography variant="h6">Configurações Avançadas</Typography>
        <Button color="warning">Resetar Permissões</Button>
        <Button color="error">Excluir em Lote</Button>
      </Paper>
    </ConditionalRender>
  );
};
```

## Exemplo 5: DataGrid com Colunas Condicionais

```tsx
const createColumns = (): DataTableColumn<AccessGroup>[] => {
  const baseColumns = [
    { id: 'name', label: 'Nome' },
    { id: 'code', label: 'Código' },
    { id: 'status', label: 'Status' },
  ];

  // Coluna de ações sempre presente, mas com botões condicionais
  const actionsColumn = {
    id: 'actions',
    label: 'Ações',
    format: (_, row) => (
      <Box sx={{ display: 'flex', gap: 1 }}>
        {/* Ver sempre disponível */}
        <IconButton size="small">
          <ViewIcon />
        </IconButton>

        {/* Editar condicional */}
        <OperationGuard module={ModuleKey.ACCESS_GROUP} operations={['UPDATE']}>
          <IconButton size="small" onClick={() => handleEdit(row)}>
            <EditIcon />
          </IconButton>
        </OperationGuard>

        {/* Deletar condicional */}
        <OperationGuard module={ModuleKey.ACCESS_GROUP} operations={['DELETE']}>
          <IconButton size="small" color="error" onClick={() => handleDelete(row)}>
            <DeleteIcon />
          </IconButton>
        </OperationGuard>
      </Box>
    )
  };

  return [...baseColumns, actionsColumn];
};
```

## Exemplo 6: Hook Personalizado para Módulo Específico

```tsx
// Hook específico para facilitar uso
const useAccessGroupOperations = () => {
  return useOperationPermissions(ModuleKey.ACCESS_GROUP);
};

// Uso no componente
const AccessGroupPage = () => {
  const { canRead, canCreate, canUpdate, canDelete } = useAccessGroupOperations();

  if (!canRead) {
    return <AccessDenied />;
  }

  return (
    <Container>
      <PageHeader
        title="Grupos de Acesso"
        action={canCreate ? (
          <Button startIcon={<AddIcon />}>
            Novo Grupo
          </Button>
        ) : undefined}
      />
      
      {/* Lista com ações condicionais */}
      <AccessGroupList
        canEdit={canUpdate}
        canDelete={canDelete}
      />
    </Container>
  );
};
```

## Benefícios da Implementação

### 1. Segurança Granular
- Controle preciso sobre cada operação
- Impossível contornar verificações no frontend
- Alinhamento com permissões do backend

### 2. UX Intuitiva
- Usuário só vê o que pode usar
- Menos confusão e cliques desnecessários
- Interface adapta-se automaticamente

### 3. Manutenibilidade
- Lógica centralizada nos hooks
- Fácil de testar componentes isoladamente
- Padrão consistente em toda aplicação

### 4. Performance
- Componentes só renderizam quando necessário
- Hooks com memoização evitam recálculos
- Menos DOM nodes desnecessários