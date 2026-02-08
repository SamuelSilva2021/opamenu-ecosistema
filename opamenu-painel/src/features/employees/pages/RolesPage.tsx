import { useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import {
    Plus,
    Search,
    MoreHorizontal,
    Edit,
    Trash2,
    ShieldCheck
} from "lucide-react";
import { employeesService } from "../employees.service";
import {
    Table,
    TableBody,
    TableCell,
    TableHead,
    TableHeader,
    TableRow,
} from "@/components/ui/table";
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuLabel,
    DropdownMenuSeparator,
    DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Badge } from "@/components/ui/badge";
import { useToast } from "@/hooks/use-toast";
import {
    AlertDialog,
    AlertDialogAction,
    AlertDialogCancel,
    AlertDialogContent,
    AlertDialogDescription,
    AlertDialogFooter,
    AlertDialogHeader,
    AlertDialogTitle,
} from "@/components/ui/alert-dialog";
import RoleForm from "@/features/employees/components/RoleForm";
import type { Role } from "../types";

const RolesPage = () => {
    const [search, setSearch] = useState("");
    const [page] = useState(1);
    const [isFormOpen, setIsFormOpen] = useState(false);
    const [selectedRole, setSelectedRole] = useState<Role | null>(null);
    const [roleToDelete, setRoleToDelete] = useState<Role | null>(null);

    const { toast } = useToast();
    const queryClient = useQueryClient();

    const { data: rolesData, isLoading } = useQuery({
        queryKey: ["roles", page, search],
        queryFn: () => employeesService.getRoles({ page, limit: 10, search }),
    });

    const deleteMutation = useMutation({
        mutationFn: (id: string) => employeesService.deleteRole(id),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ["roles"] });
            toast({
                title: "Sucesso",
                description: "Perfil excluído com sucesso.",
            });
            setRoleToDelete(null);
        },
        onError: (error: any) => {
            toast({
                title: "Erro",
                description: error?.response?.data?.message || "Erro ao excluir perfil.",
                variant: "destructive",
            });
        }
    });

    const handleEdit = (role: Role) => {
        setSelectedRole(role);
        setIsFormOpen(true);
    };

    const handleCreate = () => {
        setSelectedRole(null);
        setIsFormOpen(true);
    };

    return (
        <div className="flex flex-col gap-6 p-6">
            <div className="flex items-center justify-between">
                <div>
                    <h1 className="text-3xl font-bold tracking-tight">Perfis de Acesso</h1>
                    <p className="text-muted-foreground">
                        Defina perfis com permissões específicas para seus colaboradores.
                    </p>
                </div>
                <Button onClick={handleCreate} className="gap-2">
                    <Plus className="h-4 w-4" /> Novo Perfil
                </Button>
            </div>

            <div className="flex items-center gap-2">
                <div className="relative flex-1 max-w-sm">
                    <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
                    <Input
                        placeholder="Buscar perfis..."
                        value={search}
                        onChange={(e) => setSearch(e.target.value)}
                        className="pl-9"
                    />
                </div>
            </div>

            <div className="rounded-md border bg-card">
                <Table>
                    <TableHeader>
                        <TableRow>
                            <TableHead>Perfil</TableHead>
                            <TableHead>Descrição</TableHead>
                            <TableHead>Status</TableHead>
                            <TableHead>Permissões</TableHead>
                            <TableHead className="w-[80px]">Ações</TableHead>
                        </TableRow>
                    </TableHeader>
                    <TableBody>
                        {isLoading ? (
                            <TableRow>
                                <TableCell colSpan={5} className="h-24 text-center">
                                    Carregando...
                                </TableCell>
                            </TableRow>
                        ) : rolesData?.data?.items?.length === 0 ? (
                            <TableRow>
                                <TableCell colSpan={5} className="h-24 text-center">
                                    Nenhum perfil encontrado.
                                </TableCell>
                            </TableRow>
                        ) : (
                            rolesData?.data?.items?.map((role) => (
                                <TableRow key={role.id}>
                                    <TableCell className="font-medium">
                                        <div className="flex items-center gap-2">
                                            <ShieldCheck className="h-4 w-4 text-primary" />
                                            {role.name}
                                            {role.isDefault && (
                                                <Badge variant="secondary" className="text-[10px]">Padrão</Badge>
                                            )}
                                        </div>
                                    </TableCell>
                                    <TableCell>{role.description || "-"}</TableCell>
                                    <TableCell>
                                        <Badge variant={role.isActive ? "default" : "secondary"}>
                                            {role.isActive ? "Ativo" : "Inativo"}
                                        </Badge>
                                    </TableCell>
                                    <TableCell>
                                        <div className="flex flex-wrap gap-1">
                                            {role.permissions?.slice(0, 3).map((p) => (
                                                <Badge key={p.module} variant="outline" className="text-[10px]">
                                                    {p.module}
                                                </Badge>
                                            ))}
                                            {role.permissions?.length > 3 && (
                                                <span className="text-xs text-muted-foreground">
                                                    +{role.permissions.length - 3} mais
                                                </span>
                                            )}
                                        </div>
                                    </TableCell>
                                    <TableCell>
                                        <DropdownMenu>
                                            <DropdownMenuTrigger asChild>
                                                <Button variant="ghost" className="h-8 w-8 p-0">
                                                    <MoreHorizontal className="h-4 w-4" />
                                                </Button>
                                            </DropdownMenuTrigger>
                                            <DropdownMenuContent align="end">
                                                <DropdownMenuLabel>Ações</DropdownMenuLabel>
                                                <DropdownMenuItem onClick={() => handleEdit(role)}>
                                                    <Edit className="mr-2 h-4 w-4" /> Editar
                                                </DropdownMenuItem>
                                                <DropdownMenuSeparator />
                                                <DropdownMenuItem
                                                    className="text-destructive focus:text-destructive"
                                                    onClick={() => setRoleToDelete(role)}
                                                >
                                                    <Trash2 className="mr-2 h-4 w-4" /> Excluir
                                                </DropdownMenuItem>
                                            </DropdownMenuContent>
                                        </DropdownMenu>
                                    </TableCell>
                                </TableRow>
                            ))
                        )}
                    </TableBody>
                </Table>
            </div>

            {isFormOpen && (
                <RoleForm
                    open={isFormOpen}
                    onOpenChange={setIsFormOpen}
                    role={selectedRole}
                />
            )}

            <AlertDialog open={!!roleToDelete} onOpenChange={(open) => !open && setRoleToDelete(null)}>
                <AlertDialogContent>
                    <AlertDialogHeader>
                        <AlertDialogTitle>Você tem certeza?</AlertDialogTitle>
                        <AlertDialogDescription>
                            Esta ação não pode ser desfeita. Isso excluirá permanentemente o perfil
                            <strong> {roleToDelete?.name}</strong> e removerá o acesso de todos os colaboradores vinculados a ele.
                        </AlertDialogDescription>
                    </AlertDialogHeader>
                    <AlertDialogFooter>
                        <AlertDialogCancel>Cancelar</AlertDialogCancel>
                        <AlertDialogAction
                            onClick={() => roleToDelete && deleteMutation.mutate(roleToDelete.id)}
                            className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
                        >
                            Excluir
                        </AlertDialogAction>
                    </AlertDialogFooter>
                </AlertDialogContent>
            </AlertDialog>
        </div>
    );
};

export default RolesPage;
