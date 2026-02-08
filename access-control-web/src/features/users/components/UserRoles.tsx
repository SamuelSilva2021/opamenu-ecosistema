import { useState, useEffect } from 'react';
import {
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    Button,
    Box,
    Typography,
    Card,
    CardContent,
    Chip,
    Radio,
    FormControlLabel,
    Alert,
    CircularProgress,
    Stack,
    Divider,
    IconButton,
    TablePagination,
    RadioGroup
} from '@mui/material';
import {
    Security as SecurityIcon,
    Close as CloseIcon,
    Badge as RoleIcon
} from '@mui/icons-material';
import { useUsers } from '../hooks/useUsers';
import { RoleService } from '../../../shared/services';
import type { UserAccount, Role } from '../../../shared/types';

interface UserRolesProps {
    open: boolean;
    onClose: () => void;
    user: UserAccount | null;
    onSuccess?: () => void;
}

export function UserRoles({ open, onClose, user, onSuccess }: UserRolesProps) {
    const {
        updateUser,
        loading: userLoading,
        error: userError
    } = useUsers({ autoLoad: false });

    const [roles, setRoles] = useState<Role[]>([]);
    const [selectedRoleId, setSelectedRoleId] = useState<string>('');
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    // Paginação
    const [page, setPage] = useState(0);
    const [rowsPerPage, setRowsPerPage] = useState(5);
    const [totalItems, setTotalItems] = useState(0);

    // Carrega dados quando o dialog é aberto
    useEffect(() => {
        if (open && user) {
            setSelectedRoleId(user.roleId || '');
            loadRoles(1, rowsPerPage);
        }
    }, [open, user, rowsPerPage]);

    const loadRoles = async (pageNumber: number, limit: number) => {
        setLoading(true);
        setError(null);

        try {
            const response = await RoleService.getRoles({
                page: pageNumber,
                limit
            });

            setRoles(response.items || []);
            setTotalItems(response.total || 0);
            setPage(pageNumber - 1);

        } catch (err: any) {
            console.error('Erro ao carregar roles:', err);
            setError(err.message || 'Erro ao carregar roles');
        } finally {
            setLoading(false);
        }
    };

    const handlePageChange = (_event: unknown, newPage: number) => {
        loadRoles(newPage + 1, rowsPerPage);
    };

    const handleRowsPerPageChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        const newLimit = parseInt(event.target.value, 10);
        setRowsPerPage(newLimit);
        loadRoles(1, newLimit);
    };

    const handleRoleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        setSelectedRoleId(event.target.value);
    };

    const handleSave = async () => {
        if (!user) return;

        setLoading(true);
        setError(null);

        try {
            await updateUser(user.id, {
                roleId: selectedRoleId || undefined
            });

            if (onSuccess) onSuccess();
            onClose();

        } catch (err: any) {
            console.error('Erro ao salvar role:', err);
            setError(err.message || 'Erro ao salvar alterações');
        } finally {
            setLoading(false);
        }
    };

    const isLoading = loading || userLoading;
    const displayError = error || userError;

    return (
        <Dialog
            open={open}
            onClose={onClose}
            maxWidth="sm"
            fullWidth
            PaperProps={{
                sx: { minHeight: '500px' }
            }}
        >
            <DialogTitle>
                <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                        <SecurityIcon />
                        <div>
                            <Typography variant="h6">
                                Vincular Perfil (Role)
                            </Typography>
                            <Typography variant="body2" color="text.secondary">
                                {user?.fullName || user?.email}
                            </Typography>
                        </div>
                    </Box>
                    <IconButton onClick={onClose}>
                        <CloseIcon />
                    </IconButton>
                </Box>
            </DialogTitle>

            <DialogContent dividers>
                {displayError && (
                    <Alert severity="error" sx={{ mb: 2 }}>
                        {displayError}
                    </Alert>
                )}

                {isLoading && roles.length === 0 ? (
                    <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}>
                        <CircularProgress />
                    </Box>
                ) : (
                    <Stack spacing={3}>
                        {/* Role atual */}
                        <Box>
                            <Typography variant="subtitle1" gutterBottom sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                                <RoleIcon fontSize="small" />
                                Perfil Atual
                            </Typography>

                            {user?.roleName ? (
                                <Chip
                                    label={user.roleName}
                                    color="primary"
                                    variant="filled"
                                    size="small"
                                    icon={<SecurityIcon />}
                                />
                            ) : (
                                <Typography color="text.secondary" sx={{ fontStyle: 'italic' }}>
                                    Nenhum perfil atribuído
                                </Typography>
                            )}
                        </Box>

                        <Divider />

                        {/* Listagem de roles paginada */}
                        <Box>
                            <Typography variant="subtitle1" gutterBottom>
                                Selecionar Novo Perfil
                            </Typography>

                            <RadioGroup value={selectedRoleId} onChange={handleRoleChange}>
                                <Stack spacing={1}>
                                    {roles.map((role) => (
                                        <Card
                                            key={role.id}
                                            variant="outlined"
                                            sx={{
                                                borderColor: selectedRoleId === role.id ? 'primary.main' : 'divider',
                                                bgcolor: selectedRoleId === role.id ? 'action.hover' : 'background.paper'
                                            }}
                                        >
                                            <CardContent sx={{ p: '8px 16px !important' }}>
                                                <FormControlLabel
                                                    value={role.id}
                                                    control={<Radio size="small" />}
                                                    sx={{ width: '100%', margin: 0 }}
                                                    label={
                                                        <Box sx={{ ml: 1 }}>
                                                            <Typography variant="subtitle2">
                                                                {role.name}
                                                            </Typography>
                                                            {role.description && (
                                                                <Typography variant="caption" color="text.secondary" sx={{ display: 'block' }}>
                                                                    {role.description}
                                                                </Typography>
                                                            )}
                                                        </Box>
                                                    }
                                                />
                                            </CardContent>
                                        </Card>
                                    ))}

                                    {roles.length === 0 && !isLoading && (
                                        <Typography color="text.secondary" sx={{ fontStyle: 'italic', textAlign: 'center', p: 2 }}>
                                            Nenhum perfil disponível
                                        </Typography>
                                    )}
                                </Stack>
                            </RadioGroup>

                            <TablePagination
                                component="div"
                                count={totalItems}
                                page={page}
                                onPageChange={handlePageChange}
                                rowsPerPage={rowsPerPage}
                                onRowsPerPageChange={handleRowsPerPageChange}
                                rowsPerPageOptions={[5, 10, 20]}
                                labelRowsPerPage="Itens:"
                                sx={{ mt: 1 }}
                            />
                        </Box>
                    </Stack>
                )}
            </DialogContent>

            <DialogActions sx={{ p: 2, gap: 1 }}>
                <Button
                    onClick={onClose}
                    disabled={isLoading}
                >
                    Cancelar
                </Button>
                <Button
                    onClick={handleSave}
                    variant="contained"
                    disabled={isLoading || (!selectedRoleId && !user?.roleId)}
                    startIcon={isLoading ? <CircularProgress size={16} color="inherit" /> : <SecurityIcon />}
                >
                    {isLoading ? 'Salvando...' : 'Salvar Alteração'}
                </Button>
            </DialogActions>
        </Dialog>
    );
}
