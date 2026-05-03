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
  Paper,
  Avatar,
  Stack,
} from '@mui/material';
import { 
  Edit as EditIcon, 
  Delete as DeleteIcon,
  LocalOffer as LocalOfferIcon,
  Schedule as ScheduleIcon,
  CheckCircle as CheckCircleIcon,
  PauseCircle as PauseCircleIcon,
} from '@mui/icons-material';
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
    return (
      <Box sx={{ p: 6, textAlign: 'center' }}>
        <Typography color="text.secondary">Carregando planos...</Typography>
      </Box>
    );
  }

  const getStatusChip = (status?: string) => {
    const s = status || 'Ativo';
    switch (s) {
      case 'Ativo':
        return <Chip label="Ativo" size="small" color="success" icon={<CheckCircleIcon />} />;
      case 'Inativo':
        return <Chip label="Inativo" size="small" color="warning" icon={<PauseCircleIcon />} />;
      default:
        return <Chip label={s} size="small" />;
    }
  };

  const getCycleLabel = (cycle: string) => {
    switch (cycle) {
      case 'Monthly': return 'Mensal';
      case 'Yearly': return 'Anual';
      case 'Weekly': return 'Semanal';
      case 'Daily': return 'Diário';
      default: return cycle;
    }
  };

  return (
    <Box>
      <TableContainer>
        <Table sx={{ minWidth: 650 }}>
          <TableHead sx={{ bgcolor: 'grey.50' }}>
            <TableRow>
              <TableCell sx={{ fontWeight: 700 }}>Plano</TableCell>
              <TableCell sx={{ fontWeight: 700 }}>Preço</TableCell>
              <TableCell sx={{ fontWeight: 700 }}>Ciclo</TableCell>
              <TableCell sx={{ fontWeight: 700 }}>Trial</TableCell>
              <TableCell sx={{ fontWeight: 700 }}>Status</TableCell>
              <TableCell align="center" sx={{ fontWeight: 700 }}>Ações</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {plans.map((plan) => (
              <TableRow key={plan.id} hover sx={{ '&:last-child td, &:last-child th': { border: 0 } }}>
                <TableCell>
                  <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
                    <Avatar sx={{ bgcolor: 'primary.light', width: 40, height: 40 }}>
                      <LocalOfferIcon />
                    </Avatar>
                    <Box>
                      <Typography variant="subtitle2" sx={{ fontWeight: 700 }}>
                        {plan.name}
                      </Typography>
                      <Typography variant="caption" color="textSecondary" sx={{ display: 'block' }}>
                        {plan.slug}
                      </Typography>
                    </Box>
                  </Box>
                </TableCell>
                <TableCell>
                  <Typography variant="body2" sx={{ fontWeight: 600 }}>
                    {new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(plan.price)}
                  </Typography>
                </TableCell>
                <TableCell>
                  <Stack direction="row" alignItems="center" spacing={0.5}>
                    <ScheduleIcon sx={{ fontSize: 16, color: 'text.secondary' }} />
                    <Typography variant="body2">{getCycleLabel(plan.billingCycle)}</Typography>
                  </Stack>
                </TableCell>
                <TableCell>
                  {plan.isTrial ? (
                    <Tooltip title={`${plan.trialPeriodDays} dias de teste`}>
                      <Chip label={`${plan.trialPeriodDays}d`} size="small" variant="outlined" color="info" />
                    </Tooltip>
                  ) : (
                    <Typography variant="caption" color="text.disabled">Sem trial</Typography>
                  )}
                </TableCell>
                <TableCell>
                  {getStatusChip(plan.status)}
                </TableCell>
                <TableCell align="center">
                  <Box sx={{ display: 'flex', justifyContent: 'center', gap: 1 }}>
                    <Tooltip title="Editar Configurações">
                      <IconButton 
                        size="small" 
                        onClick={() => onEdit(plan)}
                        sx={{ 
                          color: 'primary.main',
                          bgcolor: 'primary.50',
                          '&:hover': { bgcolor: 'primary.100' }
                        }}
                      >
                        <EditIcon fontSize="small" />
                      </IconButton>
                    </Tooltip>
                    <Tooltip title="Excluir Plano">
                      <IconButton 
                        size="small" 
                        color="error" 
                        onClick={() => onDelete(plan)}
                        sx={{ 
                          bgcolor: 'error.50',
                          '&:hover': { bgcolor: 'error.100' }
                        }}
                      >
                        <DeleteIcon fontSize="small" />
                      </IconButton>
                    </Tooltip>
                  </Box>
                </TableCell>
              </TableRow>
            ))}
            {plans.length === 0 && !loading && (
              <TableRow>
                <TableCell colSpan={6} align="center" sx={{ py: 8 }}>
                  <Typography color="text.secondary">Nenhum plano configurado no momento.</Typography>
                </TableCell>
              </TableRow>
            )}
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
        sx={{ borderTop: 1, borderColor: 'divider' }}
      />
    </Box>
  );
}
