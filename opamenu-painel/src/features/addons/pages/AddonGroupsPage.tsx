import { useState, useMemo } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { 
  Plus, 
  Edit, 
  Trash2, 
  Layers, 
  MoreHorizontal, 
  ArrowUpDown,
  Search,
  Filter
} from "lucide-react";
import { 
  useReactTable, 
  getCoreRowModel, 
  getFilteredRowModel, 
  getPaginationRowModel, 
  getSortedRowModel,
  flexRender,
  type ColumnDef,
  type SortingState,
  type ColumnFiltersState,
} from "@tanstack/react-table";

import { addonsService } from "../addons.service";
import type { AddonGroup, CreateAddonGroupRequest, UpdateAddonGroupRequest } from "../types";
import { AddonGroupForm } from "../components/AddonGroupForm";
import { AddonManager } from "../components/AddonManager";
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
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
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
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { useToast } from "@/hooks/use-toast";
import { getErrorMessage } from "@/lib/utils";
import { usePermission } from "@/hooks/usePermission";
import { PermissionGate } from "@/components/auth/PermissionGate";

export default function AddonGroupsPage() {
  const { can } = usePermission();
  const [isFormOpen, setIsFormOpen] = useState(false);
  const [editingGroup, setEditingGroup] = useState<AddonGroup | null>(null);
  const [deleteId, setDeleteId] = useState<number | null>(null);
  const [manageAddonsGroupId, setManageAddonsGroupId] = useState<number | null>(null);
  
  // Table states
  const [sorting, setSorting] = useState<SortingState>([]);
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([]);
  const [globalFilter, setGlobalFilter] = useState("");
  const [statusFilter, setStatusFilter] = useState<string>("all");

  const queryClient = useQueryClient();
  const { toast } = useToast();

  const { data: groups = [], isLoading } = useQuery({
    queryKey: ["addon-groups"],
    queryFn: addonsService.getGroups,
  });

  const createMutation = useMutation({
    mutationFn: addonsService.createGroup,
    onSuccess: (response) => {
      queryClient.invalidateQueries({ queryKey: ["addon-groups"] });
      toast({ variant: "success", title: "Sucesso", description: response.message || "Grupo criado com sucesso" });
      setIsFormOpen(false);
    },
    onError: (error) => {
      const message = getErrorMessage(error);
      toast({ title: "Erro", description: message, variant: "destructive" });
    },
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: number; data: UpdateAddonGroupRequest }) =>
      addonsService.updateGroup(id, data),
    onSuccess: (response) => {
      queryClient.invalidateQueries({ queryKey: ["addon-groups"] });
      toast({ variant: "success", title: "Sucesso", description: response.message || "Grupo atualizado com sucesso" });
      setIsFormOpen(false);
      setEditingGroup(null);
    },
    onError: (error) => {
      const message = getErrorMessage(error);
      toast({ title: "Erro", description: message, variant: "destructive" });
    },
  });

  const deleteMutation = useMutation({
    mutationFn: addonsService.deleteGroup,
    onSuccess: (response) => {
      queryClient.invalidateQueries({ queryKey: ["addon-groups"] });
      toast({ variant: "success", title: "Sucesso", description: response.message || "Grupo removido com sucesso" });
      setDeleteId(null);
    },
    onError: (error) => {
      const message = getErrorMessage(error);
      toast({ title: "Erro", description: message, variant: "destructive" });
    },
  });

  const handleCreate = (data: CreateAddonGroupRequest | UpdateAddonGroupRequest) => {
    if (editingGroup) {
      updateMutation.mutate({ id: editingGroup.id, data: data as UpdateAddonGroupRequest });
    } else {
      createMutation.mutate(data as CreateAddonGroupRequest);
    }
  };

  const openEditForm = (group: AddonGroup) => {
    setEditingGroup(group);
    setIsFormOpen(true);
  };

  const confirmDelete = () => {
    if (deleteId) {
      deleteMutation.mutate(deleteId);
    }
  };

  const openNewForm = () => {
    setEditingGroup(null);
    setIsFormOpen(true);
  };

  // Filter logic
  const filteredData = useMemo(() => {
    let data = [...groups];
    if (statusFilter !== "all") {
      const isActive = statusFilter === "active";
      data = data.filter(item => item.isActive === isActive);
    }
    return data;
  }, [groups, statusFilter]);

  const columns: ColumnDef<AddonGroup>[] = [
    {
      accessorKey: "name",
      header: ({ column }) => {
        return (
          <Button
            variant="ghost"
            onClick={() => column.toggleSorting(column.getIsSorted() === "asc")}
            className="-ml-4"
          >
            Nome
            <ArrowUpDown className="ml-2 h-4 w-4" />
          </Button>
        )
      },
    },
    {
      accessorKey: "displayOrder",
      header: "Ordem",
      cell: ({ row }) => <div className="text-center w-12">{row.getValue("displayOrder")}</div>,
    },
    {
      accessorKey: "isRequired",
      header: "Obrigatório",
      cell: ({ row }) => (
        <div className="flex justify-center">
          {row.getValue("isRequired") ? (
            <Badge variant="default" className="bg-red-500 hover:bg-red-600">Sim</Badge>
          ) : (
            <Badge variant="secondary">Não</Badge>
          )}
        </div>
      ),
    },
    {
      id: "limits",
      header: "Limites (Mín/Máx)",
      cell: ({ row }) => {
        const min = row.original.minSelections ?? 0;
        const max = row.original.maxSelections ?? "∞";
        return <span>{min} / {max}</span>
      },
    },
    {
      accessorKey: "isActive",
      header: "Status",
      cell: ({ row }) => {
        const isActive = row.getValue("isActive") as boolean;
        return (
          <Badge variant={isActive ? "default" : "destructive"} className={isActive ? "bg-green-600 hover:bg-green-700" : ""}>
            {isActive ? "Ativo" : "Inativo"}
          </Badge>
        )
      },
    },
    {
      id: "actions",
      cell: ({ row }) => {
        const group = row.original;
        const canEdit = can("ADITIONAL_GROUP", "UPDATE");
        const canDelete = can("ADITIONAL_GROUP", "DELETE");

        if (!canEdit && !canDelete) {
          return null;
        }

        return (
          <DropdownMenu>
            <DropdownMenuTrigger asChild>
              <Button variant="ghost" className="h-8 w-8 p-0">
                <span className="sr-only">Abrir menu</span>
                <MoreHorizontal className="h-4 w-4" />
              </Button>
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end">
              <DropdownMenuLabel>Ações</DropdownMenuLabel>
              {canEdit && (
                <>
                  <DropdownMenuItem onClick={() => setManageAddonsGroupId(group.id)}>
                    <Layers className="mr-2 h-4 w-4" />
                    Gerenciar Itens
                  </DropdownMenuItem>
                  <DropdownMenuSeparator />
                  <DropdownMenuItem onClick={() => openEditForm(group)}>
                    <Edit className="mr-2 h-4 w-4" />
                    Editar Grupo
                  </DropdownMenuItem>
                </>
              )}
              {canEdit && canDelete && <DropdownMenuSeparator />}
              {canDelete && (
                <DropdownMenuItem 
                  onClick={() => setDeleteId(group.id)}
                  className="text-red-600 focus:text-red-600"
                >
                  <Trash2 className="mr-2 h-4 w-4" />
                  Excluir
                </DropdownMenuItem>
              )}
            </DropdownMenuContent>
          </DropdownMenu>
        );
      },
    },
  ];

  const table = useReactTable({
    data: filteredData,
    columns,
    getCoreRowModel: getCoreRowModel(),
    getPaginationRowModel: getPaginationRowModel(),
    getSortedRowModel: getSortedRowModel(),
    getFilteredRowModel: getFilteredRowModel(),
    onSortingChange: setSorting,
    onColumnFiltersChange: setColumnFilters,
    onGlobalFilterChange: setGlobalFilter,
    state: {
      sorting,
      columnFilters,
      globalFilter,
    },
  });

  return (
    <div className="space-y-8 animate-in fade-in duration-500">
      <div className="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4">
        <div>
          <h2 className="text-3xl font-bold tracking-tight text-zinc-900 dark:text-zinc-50">Grupos de Adicionais</h2>
          <p className="text-muted-foreground mt-1">
            Gerencie os grupos e opções de adicionais para seus produtos.
          </p>
        </div>
        <PermissionGate module="ADITIONAL_GROUP" operation="CREATE">
          <Button onClick={openNewForm} className="shrink-0 w-full sm:w-auto">
            <Plus className="mr-2 h-4 w-4" />
            Novo Grupo
          </Button>
        </PermissionGate>
      </div>

      <Card className="border-none shadow-md bg-white dark:bg-zinc-800">
        <CardHeader>
          <CardTitle>Listagem de Grupos</CardTitle>
          <div className="flex flex-col lg:flex-row gap-4 pt-4 justify-between items-start lg:items-center">
            <div className="flex flex-col sm:flex-row w-full lg:w-auto gap-2">
              <div className="relative w-full sm:w-72">
                <Search className="absolute left-2 top-2.5 h-4 w-4 text-muted-foreground" />
                <Input
                  placeholder="Buscar por nome..."
                  value={(table.getColumn("name")?.getFilterValue() as string) ?? ""}
                  onChange={(event) =>
                    table.getColumn("name")?.setFilterValue(event.target.value)
                  }
                  className="pl-8"
                />
              </div>
              <div className="w-full sm:w-[180px]">
                <Select
                  value={statusFilter}
                  onValueChange={setStatusFilter}
                >
                  <SelectTrigger className="w-full">
                    <div className="flex items-center gap-2">
                      <Filter className="h-4 w-4 text-muted-foreground" />
                      <SelectValue placeholder="Status" />
                    </div>
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="all">Todos os Status</SelectItem>
                    <SelectItem value="active">Ativos</SelectItem>
                    <SelectItem value="inactive">Inativos</SelectItem>
                  </SelectContent>
                </Select>
              </div>
            </div>
            <div className="text-sm text-muted-foreground whitespace-nowrap">
              Total: {filteredData.length} grupos
            </div>
          </div>
        </CardHeader>

        <CardContent>
          <div>
            <Table>
              <TableHeader>
                {table.getHeaderGroups().map((headerGroup) => (
                  <TableRow key={headerGroup.id}>
                    {headerGroup.headers.map((header) => {
                      return (
                        <TableHead key={header.id}>
                          {header.isPlaceholder
                            ? null
                            : flexRender(
                                header.column.columnDef.header,
                                header.getContext()
                              )}
                        </TableHead>
                      )
                    })}
                  </TableRow>
                ))}
              </TableHeader>
              <TableBody>
                {isLoading ? (
                  <TableRow>
                    <TableCell colSpan={columns.length} className="h-24 text-center">
                      Carregando grupos...
                    </TableCell>
                  </TableRow>
                ) : table.getRowModel().rows?.length ? (
                  table.getRowModel().rows.map((row) => (
                    <TableRow
                      key={row.id}
                      data-state={row.getIsSelected() && "selected"}
                    >
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
                      Nenhum grupo encontrado.
                    </TableCell>
                  </TableRow>
                )}
              </TableBody>
            </Table>
          </div>

          <div className="flex items-center justify-end space-x-2 pt-4">
            <div className="flex-1 text-sm text-muted-foreground">
              Página {table.getState().pagination.pageIndex + 1} de{" "}
              {table.getPageCount()}
            </div>
            <div className="space-x-2">
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
          </div>
        </CardContent>
      </Card>

      <AddonGroupForm
        open={isFormOpen}
        onOpenChange={setIsFormOpen}
        onSubmit={handleCreate}
        initialData={editingGroup || undefined}
        isLoading={createMutation.isPending || updateMutation.isPending}
      />

      <AddonManager
        groupId={manageAddonsGroupId}
        open={!!manageAddonsGroupId}
        onOpenChange={(open: boolean) => !open && setManageAddonsGroupId(null)}
      />

      <AlertDialog open={!!deleteId} onOpenChange={(open: boolean) => !open && setDeleteId(null)}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Você tem certeza?</AlertDialogTitle>
            <AlertDialogDescription>
              Esta ação não pode ser desfeita. Isso excluirá permanentemente o grupo
              e todos os seus adicionais vinculados.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancelar</AlertDialogCancel>
            <AlertDialogAction onClick={confirmDelete} className="bg-red-600 hover:bg-red-700">
              Excluir
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  );
}
