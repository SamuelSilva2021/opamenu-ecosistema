import {
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
import { Edit as EditIcon, Delete as DeleteIcon } from '@mui/icons-material';
import type { SubscriptionPlan } from '../../../shared/types';

interface PlansListProps {
  plans: SubscriptionPlan[];
  loading?: boolean;
  totalItems: number;
  currentPage: number;
  pageSize: number;
  onPageChange: (page: number) => void;
  onPageSizeChange: (pageSize: number) => void;
  onEdit: (plan: SubscriptionPlan) => void;
  onDelete: (plan: SubscriptionPlan) => void;
}

export function PlansList({
  plans,
  loading = false,
  totalItems,
  currentPage,
  pageSize,
  onPageChange,
  onPageSizeChange,
  onEdit,
  onDelete,
}: PlansListProps) {
  const handlePageChange = (_: unknown, newPage: number) => {
    onPageChange(newPage + 1);
  };

  const handlePageSizeChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    onPageSizeChange(parseInt(event.target.value, 10));
  };

  if (loading && plans.length === 0) {
    return <Typography sx={{ p: 3, textAlign: 'center' }}>Carregando planos...</Typography>;
  }

  return (
    <>
      <TableContainer>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Nome</TableCell>
              <TableCell>Preço</TableCell>
              <TableCell>Ciclo</TableCell>
              <TableCell>Status</TableCell>
              <TableCell align="center">Ações</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {plans.map((plan) => (
              <TableRow key={plan.id} hover>
                <TableCell>
                  <Typography variant="subtitle2">{plan.name}</Typography>
                  <Typography variant="caption" color="textSecondary">{plan.slug}</Typography>
                </TableCell>
                <TableCell>
                  {new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(plan.price)}
                </TableCell>
                <TableCell>{plan.billingCycle === 'Monthly' ? 'Mensal' : 'Anual'}</TableCell>
                <TableCell>
                  <Chip 
                    label="Ativo" 
                    size="small" 
                    color="success" 
                    variant="outlined" 
                  />
                </TableCell>
                <TableCell align="center">
                  <Box sx={{ display: 'flex', justifyContent: 'center', gap: 1 }}>
                    <Tooltip title="Editar">
                      <IconButton size="small" onClick={() => onEdit(plan)}>
                        <EditIcon fontSize="small" />
                      </IconButton>
                    </Tooltip>
                    <Tooltip title="Remover">
                      <IconButton size="small" color="error" onClick={() => onDelete(plan)}>
                        <DeleteIcon fontSize="small" />
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
        rowsPerPageOptions={[5, 10, 25]}
        labelRowsPerPage="Itens por página:"
      />
    </>
  );
}
