import { useState, useEffect } from 'react';
import {
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    TextField,
    Button,
    Box,
    Typography,
    Card,
    CardContent,
    Chip,
    Radio,
    Alert,
    CircularProgress,
    Stack,
    Divider,
    IconButton,
    TablePagination,
    RadioGroup,
    InputAdornment
} from '@mui/material';
import {
    Security as SecurityIcon,
    Close as CloseIcon,
    Badge as RoleIcon,
    Search as SearchIcon
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
    const [searchTerm, setSearchTerm] = useState('');

    // Paginação
    const [page, setPage] = useState(0);
    const [rowsPerPage, setRowsPerPage] = useState(5);
    const [totalItems, setTotalItems] = useState(0);

    // Carrega dados quando o dialog é aberto
    useEffect(() => {
        if (open && user) {
            setSelectedRoleId(user.roleId || '');
            setSearchTerm('');
            loadRoles(1, rowsPerPage, '');
        }
    }, [open, user, rowsPerPage]);

    const loadRoles = async (pageNumber: number, limit: number, search: string) => {
        setLoading(true);
        setError(null);

        try {
            const response = await RoleService.getRoles({
                page: pageNumber,
                limit,
                search: search || undefined
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

    const handleSearch = () => {
        loadRoles(1, rowsPerPage, searchTerm);
    };

    const handlePageChange = (_event: unknown, newPage: number) => {
        loadRoles(newPage + 1, rowsPerPage, searchTerm);
    };

    const handleRowsPerPageChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        const newLimit = parseInt(event.target.value, 10);
        setRowsPerPage(newLimit);
        loadRoles(1, newLimit, searchTerm);
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
                                <Typography color="text.secondary" sx={{ fontStyle: 'italic', fontSize: '0.875rem' }}>
                                    Nenhum perfil atribuído
                                </Typography>
                            )}
                        </Box>

                        <Divider />

                        {/* Listagem de roles paginada */}
                        <Box>
                            <Box sx={{ mb: 2 }}>
                                <TextField
                                    fullWidth
                                    size="small"
                                    placeholder="Buscar perfis por nome..."
                                    value={searchTerm}
                                    onChange={(e: React.ChangeEvent<HTMLInputElement>) => setSearchTerm(e.target.value)}
                                    onKeyPress={(e: React.KeyboardEvent) => {
                                        if (e.key === 'Enter') handleSearch();
                                    }}
                                    InputProps={{
                                        startAdornment: (
                                            <InputAdornment position="start">
                                                <SearchIcon fontSize="small" />
                                            </InputAdornment>
                                        ),
                                        endAdornment: (
                                            <InputAdornment position="end">
                                                <Button size="small" onClick={handleSearch} disabled={isLoading}>
                                                    Buscar
                                                </Button>
                                            </InputAdornment>
                                        )
                                    }}
                                />
                            </Box>

                            <RadioGroup value={selectedRoleId} onChange={handleRoleChange}>
                                <Stack spacing={1.5}>
                                    {roles.map((role) => (
                                        <Card
                                            key={role.id}
                                            variant="outlined"
                                            onClick={() => setSelectedRoleId(role.id)}
                                            sx={{
                                                cursor: 'pointer',
                                                transition: 'all 0.2s ease-in-out',
                                                borderColor: selectedRoleId === role.id ? 'primary.main' : 'divider',
                                                bgcolor: selectedRoleId === role.id ? 'primary.50' : 'background.paper',
                                                boxShadow: selectedRoleId === role.id ? '0 0 0 1px #1976d2' : 'none',
                                                '&:hover': {
                                                    borderColor: 'primary.light',
                                                    bgcolor: selectedRoleId === role.id ? 'primary.50' : 'grey.50',
                                                    transform: 'translateY(-2px)'
                                                }
                                            }}
                                        >
                                            <CardContent sx={{ p: '12px 16px !important' }}>
                                                <Box sx={{ display: 'flex', alignItems: 'flex-start', gap: 1 }}>
                                                    <Radio
                                                        size="small"
                                                        value={role.id}
                                                        checked={selectedRoleId === role.id}
                                                        sx={{ p: 0, mt: 0.5 }}
                                                    />
                                                    <Box sx={{ flex: 1 }}>
                                                        <Typography variant="subtitle2" color={selectedRoleId === role.id ? 'primary.main' : 'text.primary'}>
                                                            {role.name}
                                                        </Typography>
                                                        {role.description && (
                                                            <Typography variant="caption" color="text.secondary" sx={{ display: 'block', mt: 0.5 }}>
                                                                {role.description}
                                                            </Typography>
                                                        )}
                                                    </Box>
                                                </Box>
                                            </CardContent>
                                        </Card>
                                    ))}

                                    {roles.length === 0 && !isLoading && (
                                        <Typography color="text.secondary" sx={{ fontStyle: 'italic', textAlign: 'center', p: 4 }}>
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
