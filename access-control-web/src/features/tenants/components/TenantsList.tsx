import {
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  IconButton,
  Tooltip,
  Typography,
  Box,
  TablePagination,
  Chip,
} from '@mui/material';
import {
  Edit as EditIcon,
  Delete as DeleteIcon,
  Business as BusinessIcon,
  Apps as AppsIcon,
} from '@mui/icons-material';
import type { TenantSummary } from '../../../shared/types';
import { CopyToClipboard } from '../../../shared/components';

interface TenantsListProps {
  tenants: TenantSummary[];
  loading?: boolean;
  totalItems: number;
  currentPage: number;
  pageSize: number;
  onPageChange: (page: number) => void;
  onPageSizeChange: (pageSize: number) => void;
  onEdit: (tenant: TenantSummary) => void;
  onDelete: (tenant: TenantSummary) => void;
  onManageModules?: (tenant: TenantSummary) => void;
}

export function TenantsList({
  tenants,
  loading = false,
  totalItems,
  currentPage,
  pageSize,
  onPageChange,
  onPageSizeChange,
  onEdit,
  onDelete,
  onManageModules,
}: TenantsListProps) {
  const handlePageChange = (_: unknown, newPage: number) => {
    onPageChange(newPage + 1);
  };

  const handlePageSizeChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    onPageSizeChange(parseInt(event.target.value, 10));
  };

  if (loading) {
    return (
      <Paper sx={{ p: 3, textAlign: 'center' }}>
        <Typography color="text.secondary">Carregando tenants...</Typography>
      </Paper>
    );
  }

  if (!tenants || tenants.length === 0) {
    return (
      <Paper sx={{ p: 3, textAlign: 'center' }}>
        <BusinessIcon sx={{ fontSize: 48, color: 'text.secondary', mb: 2 }} />
        <Typography variant="h6" color="text.secondary" gutterBottom>
          Nenhum tenant encontrado
        </Typography>
        <Typography color="text.secondary">Nenhum cliente cadastrado no momento.</Typography>
      </Paper>
    );
  }

  return (
    <Paper>
      <TableContainer>
        <Table stickyHeader>
          <TableHead>
            <TableRow>
              <TableCell>ID</TableCell>
              <TableCell>Nome</TableCell>
              <TableCell>Slug</TableCell>
              <TableCell>Domínio</TableCell>
              <TableCell>Status</TableCell>
              <TableCell>Email</TableCell>
              <TableCell>Telefone</TableCell>
              <TableCell align="center">Ações</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {tenants.map(tenant => (
              <TableRow key={tenant.id} hover>
                <TableCell>
                  <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                    <Typography variant="caption" sx={{ fontFamily: 'monospace', color: 'text.secondary' }}>
                      {tenant.id.substring(0, 8)}...
                    </Typography>
                    <CopyToClipboard content={tenant.id} tooltipTitle="Copiar ID" />
                  </Box>
                </TableCell>
                <TableCell>{tenant.name}</TableCell>
                <TableCell>{tenant.slug}</TableCell>
                <TableCell>{tenant.domain || '-'}</TableCell>
                <TableCell>
                  <Chip
                    label={tenant.status}
                    size="small"
                    color={tenant.status === 'Ativo' ? 'success' : tenant.status === 'Pendente' ? 'warning' : 'default'}
                  />
                </TableCell>
                <TableCell>{tenant.email || '-'}</TableCell>
                <TableCell>{tenant.phone || '-'}</TableCell>
                <TableCell align="center">
                  <Box sx={{ display: 'flex', justifyContent: 'center', gap: 1 }}>
                    {onManageModules && (
                      <Tooltip title="Vincular módulos">
                        <IconButton size="small" onClick={() => onManageModules(tenant)}>
                          <AppsIcon />
                        </IconButton>
                      </Tooltip>
                    )}
                    <Tooltip title="Editar">
                      <IconButton size="small" onClick={() => onEdit(tenant)}>
                        <EditIcon />
                      </IconButton>
                    </Tooltip>
                    <Tooltip title="Remover">
                      <IconButton size="small" color="error" onClick={() => onDelete(tenant)}>
                        <DeleteIcon />
                      </IconButton>
                    </Tooltip>
                  </Box>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>

      <TablePagination
        component="div"
        count={totalItems}
        page={currentPage - 1}
        onPageChange={handlePageChange}
        rowsPerPage={pageSize}
        onRowsPerPageChange={handlePageSizeChange}
        rowsPerPageOptions={[5, 10, 25, 50]}
        labelRowsPerPage="Itens por página:"
        labelDisplayedRows={({ from, to, count }) => `${from}-${to} de ${count !== -1 ? count : `mais de ${to}`}`}
        sx={{
          borderTop: 1,
          borderColor: 'divider',
          '& .MuiTablePagination-toolbar': {
            px: 2,
          },
        }}
      />
    </Paper>
  );
}
