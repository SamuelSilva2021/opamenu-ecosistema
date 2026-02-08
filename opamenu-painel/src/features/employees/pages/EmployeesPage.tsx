import { useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import {
    Plus,
    Edit,
    Trash2,
    MoreHorizontal,
    Search,
    Shield,
    UserCheck,
    UserMinus,
} from "lucide-react";
import {
    useReactTable,
    getCoreRowModel,
    getPaginationRowModel,
    getSortedRowModel,
    flexRender,
    type ColumnDef,
    type SortingState,
} from "@tanstack/react-table";

import { employeesService } from "../employees.service";
import type { Employee, UpdateEmployeeRequest } from "../types";
import { EmployeeForm } from "../components/EmployeeForm";
import { EmployeeRoleModal } from "../components/EmployeeRoleModal";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import {
    Table,
    TableBody,
    TableCell,
    TableHead,
    TableHeader,
    TableRow,
} from "@/components/ui/table";
import { Badge } from "@/components/ui/badge";
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuLabel,
    DropdownMenuSeparator,
    DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
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
import { Card, CardContent, CardHeader } from "@/components/ui/card";
import { useToast } from "@/hooks/use-toast";
import { getErrorMessage } from "@/lib/utils";

export default function EmployeesPage() {
    const [isFormOpen, setIsFormOpen] = useState(false);
    const [isRoleModalOpen, setIsRoleModalOpen] = useState(false);
    const [editingEmployee, setEditingEmployee] = useState<Employee | null>(null);
    const [employeeForRole, setEmployeeForRole] = useState<Employee | null>(null);
    const [deleteId, setDeleteId] = useState<string | null>(null);

    // Table states
    const [sorting, setSorting] = useState<SortingState>([]);
    const [globalFilter, setGlobalFilter] = useState("");
    const [pagination, setPagination] = useState({
        pageIndex: 0,
        pageSize: 10,
    });

    const queryClient = useQueryClient();
    const { toast } = useToast();

    const { data: employeesData, isLoading } = useQuery({
        queryKey: ["employees", pagination.pageIndex, pagination.pageSize, globalFilter],
        queryFn: () => employeesService.getEmployees({
            page: pagination.pageIndex + 1,
            limit: pagination.pageSize,
            search: globalFilter
        }),
    });

    const { data: rolesResponse } = useQuery({
        queryKey: ["roles-list"],
        queryFn: () => employeesService.getRoles({ limit: 100 }),
    });

    const roles = rolesResponse?.data?.items || [];

    const createMutation = useMutation({
        mutationFn: employeesService.createEmployee,
        onSuccess: (response) => {
            queryClient.invalidateQueries({ queryKey: ["employees"] });
            toast({ variant: "success", title: "Sucesso", description: response.message || "Colaborador criado com sucesso" });
            setIsFormOpen(false);
        },
        onError: (error) => {
            toast({ title: "Erro", description: getErrorMessage(error), variant: "destructive" });
        },
    });

    const updateMutation = useMutation({
        mutationFn: ({ id, data }: { id: string; data: UpdateEmployeeRequest }) =>
            employeesService.updateEmployee(id, data),
        onSuccess: (response) => {
            queryClient.invalidateQueries({ queryKey: ["employees"] });
            toast({ variant: "success", title: "Sucesso", description: response.message || "Colaborador atualizado com sucesso" });
            setIsFormOpen(false);
            setEditingEmployee(null);
        },
        onError: (error) => {
            toast({ title: "Erro", description: getErrorMessage(error), variant: "destructive" });
        },
    });

    const deleteMutation = useMutation({
        mutationFn: employeesService.deleteEmployee,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ["employees"] });
            toast({ variant: "success", title: "Sucesso", description: "Colaborador removido com sucesso" });
            setDeleteId(null);
        },
        onError: (error) => {
            toast({ title: "Erro", description: getErrorMessage(error), variant: "destructive" });
        },
    });

    const toggleStatusMutation = useMutation({
        mutationFn: employeesService.toggleEmployeeStatus,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ["employees"] });
            toast({ variant: "success", title: "Sucesso", description: "Status alterado com sucesso" });
        },
        onError: (error) => {
            toast({ title: "Erro", description: getErrorMessage(error), variant: "destructive" });
        },
    });

    const handleCreateOrUpdate = (data: any) => {
        if (editingEmployee) {
            updateMutation.mutate({ id: editingEmployee.id, data });
        } else {
            createMutation.mutate(data);
        }
    };

    const handleRoleUpdate = (roleId: string) => {
        if (employeeForRole) {
            updateMutation.mutate({
                id: employeeForRole.id,
                data: {
                    firstName: employeeForRole.firstName,
                    lastName: employeeForRole.lastName,
                    email: employeeForRole.email,
                    roleId,
                    status: employeeForRole.status,
                },
            });
            setIsRoleModalOpen(false);
        }
    };

    const columns: ColumnDef<Employee>[] = [
        {
            accessorKey: "fullName",
            header: "Nome",
            cell: ({ row }) => (
                <div className="flex flex-col">
                    <span className="font-medium">{row.original.fullName}</span>
                    <span className="text-xs text-muted-foreground">@{row.original.username}</span>
                </div>
            ),
        },
        {
            accessorKey: "email",
            header: "E-mail",
        },
        {
            accessorKey: "roleName",
            header: "Perfil",
            cell: ({ row }) => (
                <div className="flex items-center gap-1">
                    <Shield className="h-3.5 w-3.5 text-primary/70" />
                    <span>{row.original.roleName || "Nenhum"}</span>
                </div>
            ),
        },
        {
            accessorKey: "status",
            header: "Status",
            cell: ({ row }) => {
                const status = row.original.status;
                const isActive = status === "Ativo";
                return (
                    <Badge variant={isActive ? "default" : "secondary"}>
                        {status}
                    </Badge>
                );
            },
        },
        {
            id: "actions",
            cell: ({ row }) => {
                const employee = row.original;
                return (
                    <DropdownMenu>
                        <DropdownMenuTrigger asChild>
                            <Button variant="ghost" className="h-8 w-8 p-0">
                                <MoreHorizontal className="h-4 w-4" />
                            </Button>
                        </DropdownMenuTrigger>
                        <DropdownMenuContent align="end">
                            <DropdownMenuLabel>Ações</DropdownMenuLabel>
                            <DropdownMenuItem onClick={() => {
                                setEditingEmployee(employee);
                                setIsFormOpen(true);
                            }}>
                                <Edit className="mr-2 h-4 w-4" />
                                Editar
                            </DropdownMenuItem>
                            <DropdownMenuItem onClick={() => {
                                setEmployeeForRole(employee);
                                setIsRoleModalOpen(true);
                            }}>
                                <Shield className="mr-2 h-4 w-4" />
                                Alterar Perfil
                            </DropdownMenuItem>
                            <DropdownMenuItem onClick={() => toggleStatusMutation.mutate(employee.id)}>
                                {employee.status === "Ativo" ? (
                                    <>
                                        <UserMinus className="mr-2 h-4 w-4 text-orange-500" />
                                        Desativar
                                    </>
                                ) : (
                                    <>
                                        <UserCheck className="mr-2 h-4 w-4 text-green-500" />
                                        Ativar
                                    </>
                                )}
                            </DropdownMenuItem>
                            <DropdownMenuSeparator />
                            <DropdownMenuItem
                                onClick={() => setDeleteId(employee.id)}
                                className="text-red-600 focus:text-red-600"
                            >
                                <Trash2 className="mr-2 h-4 w-4" />
                                Excluir
                            </DropdownMenuItem>
                        </DropdownMenuContent>
                    </DropdownMenu>
                );
            },
        },
    ];

    const table = useReactTable({
        data: employeesData?.data?.items || [],
        columns,
        pageCount: Math.ceil((employeesData?.data?.total || 0) / pagination.pageSize),
        state: {
            sorting,
            pagination,
        },
        onSortingChange: setSorting,
        onPaginationChange: setPagination,
        getCoreRowModel: getCoreRowModel(),
        getPaginationRowModel: getPaginationRowModel(),
        getSortedRowModel: getSortedRowModel(),
        manualPagination: true,
    });

    return (
        <div className="space-y-6 animate-in fade-in duration-500">
            <div className="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4">
                <div>
                    <h2 className="text-3xl font-bold tracking-tight">Colaboradores</h2>
                    <p className="text-muted-foreground mt-1">
                        Gerencie sua equipe e atribua permissões personalizadas.
                    </p>
                </div>
                <Button onClick={() => {
                    setEditingEmployee(null);
                    setIsFormOpen(true);
                }}>
                    <Plus className="mr-2 h-4 w-4" />
                    Novo Colaborador
                </Button>
            </div>

            <Card>
                <CardHeader>
                    <div className="flex flex-col sm:flex-row gap-4 justify-between items-start sm:items-center">
                        <div className="relative w-full sm:w-80">
                            <Search className="absolute left-2 top-2.5 h-4 w-4 text-muted-foreground" />
                            <Input
                                placeholder="Buscar por nome ou e-mail..."
                                value={globalFilter}
                                onChange={(e) => setGlobalFilter(e.target.value)}
                                className="pl-8"
                            />
                        </div>
                        <div className="text-sm text-muted-foreground">
                            Total: {employeesData?.data?.total || 0} colaboradores
                        </div>
                    </div>
                </CardHeader>
                <CardContent>
                    <div className="rounded-md border">
                        <Table>
                            <TableHeader>
                                {table.getHeaderGroups().map((headerGroup) => (
                                    <TableRow key={headerGroup.id}>
                                        {headerGroup.headers.map((header) => (
                                            <TableHead key={header.id}>
                                                {header.isPlaceholder ? null : flexRender(header.column.columnDef.header, header.getContext())}
                                            </TableHead>
                                        ))}
                                    </TableRow>
                                ))}
                            </TableHeader>
                            <TableBody>
                                {isLoading ? (
                                    <TableRow>
                                        <TableCell colSpan={columns.length} className="h-24 text-center">
                                            Carregando colaboradores...
                                        </TableCell>
                                    </TableRow>
                                ) : (table.getRowModel().rows?.length ? (
                                    table.getRowModel().rows.map((row) => (
                                        <TableRow key={row.id}>
                                            {row.getVisibleCells().map((cell) => (
                                                <TableCell key={cell.id}>
                                                    {flexRender(cell.column.columnDef.cell, cell.getContext())}
                                                </TableCell>
                                            ))}
                                        </TableRow>
                                    ))
                                ) : (
                                    <TableRow>
                                        <TableCell colSpan={columns.length} className="h-24 text-center">
                                            Nenhum colaborador encontrado.
                                        </TableCell>
                                    </TableRow>
                                ))}
                            </TableBody>
                        </Table>
                    </div>

                    <div className="flex items-center justify-end space-x-2 pt-4">
                        <Button
                            variant="outline"
                            size="sm"
                            onClick={() => table.previousPage()}
                            disabled={!table.getCanPreviousPage()}
                        >
                            Anterior
                        </Button>
                        <Button
                            variant="outline"
                            size="sm"
                            onClick={() => table.nextPage()}
                            disabled={!table.getCanNextPage()}
                        >
                            Próxima
                        </Button>
                    </div>
                </CardContent>
            </Card>

            <EmployeeForm
                open={isFormOpen}
                onOpenChange={setIsFormOpen}
                onSubmit={handleCreateOrUpdate}
                initialData={editingEmployee}
                roles={roles}
                isLoading={createMutation.isPending || updateMutation.isPending}
            />

            <EmployeeRoleModal
                open={isRoleModalOpen}
                onOpenChange={setIsRoleModalOpen}
                employee={employeeForRole}
                onSuccess={handleRoleUpdate}
                isLoading={updateMutation.isPending}
            />

            <AlertDialog open={!!deleteId} onOpenChange={(open) => !open && setDeleteId(null)}>
                <AlertDialogContent>
                    <AlertDialogHeader>
                        <AlertDialogTitle>Excluir Colaborador?</AlertDialogTitle>
                        <AlertDialogDescription>
                            Esta ação removerá permanentemente o acesso deste usuário ao sistema.
                        </AlertDialogDescription>
                    </AlertDialogHeader>
                    <AlertDialogFooter>
                        <AlertDialogCancel>Cancelar</AlertDialogCancel>
                        <AlertDialogAction onClick={() => deleteId && deleteMutation.mutate(deleteId)} className="bg-red-600 hover:bg-red-700">
                            Excluir
                        </AlertDialogAction>
                    </AlertDialogFooter>
                </AlertDialogContent>
            </AlertDialog>
        </div>
    );
}
