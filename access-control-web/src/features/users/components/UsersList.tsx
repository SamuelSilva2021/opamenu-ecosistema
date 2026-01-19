import { 
  Paper, 
  Table, 
  TableBody, 
  TableCell, 
  TableContainer, 
  TableHead, 
  TableRow,
  IconButton,
  Chip,
  Tooltip,
  Typography,
  Box,
  TablePagination,
  Avatar
} from '@mui/material';
import {
  Edit as EditIcon,
  Delete as DeleteIcon,
  ToggleOff as InactiveIcon,
  ToggleOn as ActiveIcon,
  Person as PersonIcon,
  Email as EmailIcon,
  Phone as PhoneIcon,
  VerifiedUser as VerifiedIcon,
  Cancel as UnverifiedIcon,
  Security as SecurityIcon
} from '@mui/icons-material';
import { type UserAccount, UserAccountStatus } from '../../../shared/types';

interface UsersListProps {
  users: UserAccount[];
  loading?: boolean;
  onEdit: (user: UserAccount) => void;
  onDelete: (user: UserAccount) => void;
  onToggleStatus: (user: UserAccount) => void;
  onManageGroups: (user: UserAccount) => void;
  
  // Paginação
  totalItems: number;
  currentPage: number;
  pageSize: number;
  onPageChange: (page: number) => void;
  onPageSizeChange: (pageSize: number) => void;
}

/**
 * Componente de listagem de usuários com tabela paginada
 * 
 * Features:
 * - Tabela responsiva com Material-UI
 * - Chips coloridos para status
 * - Ações inline (editar, excluir, toggle status)
 * - Paginação integrada
 * - Indicadores visuais (avatars, badges)
 * - Tooltips informativos
 * - Loading states
 */
export function UsersList({ 
  users, 
  loading = false,
  onEdit, 
  onDelete, 
  onToggleStatus,
  onManageGroups,
  totalItems,
  currentPage,
  pageSize,
  onPageChange,
  onPageSizeChange
}: UsersListProps) {

  const getStatusColor = (status: string) => {
    switch (status) {
      case UserAccountStatus.Active: return 'success';
      case UserAccountStatus.Inactive: return 'default';
      case UserAccountStatus.Pending: return 'warning';
      case UserAccountStatus.Suspended: return 'error';
      default: return 'default';
    }
  };

  const getStatusText = (status: string) => {
    return status;
  };

  /**
   * Gera iniciais para o avatar
   */
  const getInitials = (firstName: string, lastName: string) => {
    const first = firstName?.charAt(0)?.toUpperCase() || '';
    const last = lastName?.charAt(0)?.toUpperCase() || '';
    return `${first}${last}` || '?';
  };

  /**
   * Manipula mudança de página
   */
  const handlePageChange = (_event: unknown, newPage: number) => {
    onPageChange(newPage + 1); // Material-UI usa índice zero
  };

  /**
   * Manipula mudança de tamanho da página
   */
  const handlePageSizeChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const newPageSize = parseInt(event.target.value, 10);
    onPageSizeChange(newPageSize);
    onPageChange(1); // Volta para primeira página
  };

  if (loading) {
    return (
      <Paper sx={{ p: 3, textAlign: 'center' }}>
        <Typography color="text.secondary">
          Carregando usuários...
        </Typography>
      </Paper>
    );
  }

  if (!users || users.length === 0) {
    return (
      <Paper sx={{ p: 3, textAlign: 'center' }}>
        <PersonIcon sx={{ fontSize: 48, color: 'text.secondary', mb: 2 }} />
        <Typography variant="h6" color="text.secondary" gutterBottom>
          Nenhum usuário encontrado
        </Typography>
        <Typography color="text.secondary">
          Comece criando seu primeiro usuário do sistema.
        </Typography>
      </Paper>
    );
  }

  return (
    <Paper>
      <TableContainer>
        <Table stickyHeader>
          <TableHead>
            <TableRow>
              <TableCell>Usuário</TableCell>
              <TableCell>Email</TableCell>
              <TableCell>Telefone</TableCell>
              <TableCell>Grupos de Acesso</TableCell>
              <TableCell align="center">Status</TableCell>
              <TableCell align="center">Email Verificado</TableCell>
              <TableCell align="center">Último Login</TableCell>
              <TableCell align="center">Ações</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {users.map((user) => (
              <TableRow 
                key={user.id} 
                hover
                sx={{ 
                  '&:hover': { backgroundColor: 'action.hover' },
                  opacity: user.status === 'Ativo' ? 1 : 0.7
                }}
              >
                {/* Coluna Usuário */}
                <TableCell>
                  <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
                    <Avatar 
                      sx={{ 
                        width: 40, 
                        height: 40,
                        bgcolor: user.status === 'Ativo' ? 'primary.main' : 'grey.400',
                        fontSize: '0.875rem'
                      }}
                    >
                      {getInitials(user.firstName, user.lastName)}
                    </Avatar>
                    <Box>
                      <Typography 
                        variant="subtitle2" 
                        fontWeight={600}
                        sx={{ 
                          color: user.status === 'Ativo' ? 'text.primary' : 'text.secondary'
                        }}
                      >
                        {user.fullName || `${user.firstName} ${user.lastName}`.trim()}
                      </Typography>
                      <Typography 
                        variant="caption" 
                        color="text.secondary"
                        sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}
                      >
                        <PersonIcon sx={{ fontSize: 12 }} />
                        @{user.username}
                      </Typography>
                    </Box>
                  </Box>
                </TableCell>

                {/* Coluna Email */}
                <TableCell>
                  <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                    <EmailIcon sx={{ fontSize: 16, color: 'text.secondary' }} />
                    <Typography variant="body2">
                      {user.email}
                    </Typography>
                  </Box>
                </TableCell>

                {/* Coluna Telefone */}
                <TableCell>
                  {user.phoneNumber ? (
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                      <PhoneIcon sx={{ fontSize: 16, color: 'text.secondary' }} />
                      <Typography variant="body2">
                        {user.phoneNumber}
                      </Typography>
                    </Box>
                  ) : (
                    <Typography variant="body2" color="text.secondary">
                      -
                    </Typography>
                  )}
                </TableCell>

                {/* Coluna Grupos de Acesso */}
                <TableCell>
                  <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.5, maxWidth: 250 }}>
                    {user.accessGroups && user.accessGroups.length > 0 ? (
                      <>
                        {user.accessGroups.slice(0, 2).map((group) => (
                          <Chip
                            key={group.id}
                            label={group.name}
                            size="small"
                            variant="outlined"
                            sx={{ fontSize: '0.75rem' }}
                          />
                        ))}
                        {user.accessGroups.length > 2 && (
                          <Tooltip title={user.accessGroups.slice(2).map(g => g.name).join(', ')}>
                            <Chip
                              label={`+${user.accessGroups.length - 2}`}
                              size="small"
                              variant="outlined"
                              sx={{ fontSize: '0.75rem', bgcolor: 'action.hover', cursor: 'help' }}
                            />
                          </Tooltip>
                        )}
                      </>
                    ) : (
                      <Typography variant="caption" color="text.secondary">
                        -
                      </Typography>
                    )}
                  </Box>
                </TableCell>

                {/* Coluna Status */}
                <TableCell align="center">
                  <Chip
                    label={getStatusText(user.status)}
                    color={getStatusColor(user.status) as any}
                    size="small"
                    variant={user.status === 'Ativo' ? 'filled' : 'outlined'}
                  />
                </TableCell>

                {/* Coluna Email Verificado */}
                <TableCell align="center">
                  <Tooltip title={user.isEmailVerified ? 'Email verificado' : 'Email não verificado'}>
                    {user.isEmailVerified ? (
                      <VerifiedIcon sx={{ color: 'success.main', fontSize: 20 }} />
                    ) : (
                      <UnverifiedIcon sx={{ color: 'warning.main', fontSize: 20 }} />
                    )}
                  </Tooltip>
                </TableCell>

                {/* Coluna Último Login */}
                <TableCell align="center">
                  <Typography variant="body2" color="text.secondary">
                    {user.lastLoginAt 
                      ? new Date(user.lastLoginAt).toLocaleDateString('pt-BR', {
                          day: '2-digit',
                          month: '2-digit',
                          year: 'numeric',
                          hour: '2-digit',
                          minute: '2-digit'
                        })
                      : 'Nunca'
                    }
                  </Typography>
                </TableCell>

                {/* Coluna Ações */}
                <TableCell align="center">
                  <Box sx={{ display: 'flex', justifyContent: 'center', gap: 0.5 }}>
                    {/* Toggle Status */}
                    <Tooltip title={user.status === 'Ativo' ? 'Desativar usuário' : 'Ativar usuário'}>
                      <IconButton
                        size="small"
                        onClick={() => onToggleStatus(user)}
                        color={user.status === 'Ativo' ? 'success' : 'default'}
                      >
                        {user.status === 'Ativo' ? <ActiveIcon /> : <InactiveIcon />}
                      </IconButton>
                    </Tooltip>

                    {/* Editar */}
                    <Tooltip title="Editar usuário">
                      <IconButton
                        size="small"
                        onClick={() => onEdit(user)}
                        color="primary"
                      >
                        <EditIcon />
                      </IconButton>
                    </Tooltip>

                    {/* Gerenciar Grupos */}
                    <Tooltip title="Gerenciar grupos de acesso">
                      <IconButton
                        size="small"
                        onClick={() => onManageGroups(user)}
                        color="secondary"
                      >
                        <SecurityIcon />
                      </IconButton>
                    </Tooltip>

                    {/* Excluir */}
                    <Tooltip title="Excluir usuário">
                      <IconButton
                        size="small"
                        onClick={() => onDelete(user)}
                        color="error"
                      >
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

      {/* Paginação */}
      <TablePagination
        component="div"
        count={totalItems}
        page={currentPage - 1} // Material-UI usa índice zero
        onPageChange={handlePageChange}
        rowsPerPage={pageSize}
        onRowsPerPageChange={handlePageSizeChange}
        rowsPerPageOptions={[5, 10, 25, 50]}
        labelRowsPerPage="Itens por página:"
        labelDisplayedRows={({ from, to, count }) => 
          `${from}-${to} de ${count !== -1 ? count : `mais de ${to}`}`
        }
        sx={{
          borderTop: 1,
          borderColor: 'divider',
          '& .MuiTablePagination-toolbar': {
            px: 2
          }
        }}
      />
    </Paper>
  );
}