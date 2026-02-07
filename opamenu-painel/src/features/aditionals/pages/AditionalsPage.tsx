import { useState, useMemo } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import {
  Plus,
  Edit,
  Trash2,
  MoreHorizontal,
  ArrowUpDown,
  Search,
  Filter,
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

import { aditionalsService } from "../aditionals.service";
import type { Aditional, CreateAditionalRequest, UpdateAditionalRequest } from "../types";
import { AditionalForm } from "../components/AditionalForm";
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

export default function AditionalsPage() {
  const { can } = usePermission();
  const [isFormOpen, setIsFormOpen] = useState(false);
  const [editingAditional, setEditingAditional] = useState<Aditional | null>(null);
  const [deleteId, setDeleteId] = useState<string | null>(null);

  // Table states
  const [sorting, setSorting] = useState<SortingState>([]);
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([]);
  const [globalFilter, setGlobalFilter] = useState("");
  const [statusFilter, setStatusFilter] = useState<string>("all");
  const [groupFilter, setGroupFilter] = useState<string>("all");

  const queryClient = useQueryClient();
  const { toast } = useToast();

  const { data: aditionals = [], isLoading: isLoadingAditionals } = useQuery({
    queryKey: ["aditionals"],
    queryFn: aditionalsService.getAditionals,
  });

  const { data: groups = [] } = useQuery({
    queryKey: ["aditional-groups"],
    queryFn: aditionalsService.getGroups,
  });

  const createMutation = useMutation({
    mutationFn: aditionalsService.createAditional,
    onSuccess: (response) => {
      queryClient.invalidateQueries({ queryKey: ["aditionals"] });
      queryClient.invalidateQueries({ queryKey: ["aditional-groups"] }); // Groups might show counts
      toast({ variant: "success", title: "Sucesso", description: response.message || "Adicional criado com sucesso" });
      setIsFormOpen(false);
    },
    onError: (error) => {
      const message = getErrorMessage(error);
      toast({ title: "Erro", description: message, variant: "destructive" });
    },
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: UpdateAditionalRequest }) =>
      aditionalsService.updateAditional(id, data),
    onSuccess: (response) => {
      queryClient.invalidateQueries({ queryKey: ["aditionals"] });
      queryClient.invalidateQueries({ queryKey: ["aditional-groups"] });
      toast({ variant: "success", title: "Sucesso", description: response.message || "Adicional atualizado com sucesso" });
      setIsFormOpen(false);
      setEditingAditional(null);
    },
    onError: (error) => {
      const message = getErrorMessage(error);
      toast({ title: "Erro", description: message, variant: "destructive" });
    },
  });

  const deleteMutation = useMutation({
    mutationFn: aditionalsService.deleteAditional,
    onSuccess: (response) => {
      queryClient.invalidateQueries({ queryKey: ["aditionals"] });
      queryClient.invalidateQueries({ queryKey: ["aditional-groups"] });
      toast({ variant: "success", title: "Sucesso", description: response.message || "Adicional removido com sucesso" });
      setDeleteId(null);
    },
    onError: (error) => {
      const message = getErrorMessage(error);
      toast({ title: "Erro", description: message, variant: "destructive" });
    },
  });

  const handleCreate = (data: CreateAditionalRequest | UpdateAditionalRequest) => {
    if (editingAditional) {
      updateMutation.mutate({ id: editingAditional.id, data: data as UpdateAditionalRequest });
    } else {
      createMutation.mutate(data as CreateAditionalRequest);
    }
  };

  const openEditForm = (aditional: Aditional) => {
    setEditingAditional(aditional);
    setIsFormOpen(true);
  };

  const confirmDelete = () => {
    if (deleteId) {
      deleteMutation.mutate(deleteId);
    }
  };

  const openNewForm = () => {
    setEditingAditional(null);
    setIsFormOpen(true);
  };

  // Filter logic
  const filteredData = useMemo(() => {
    let data = [...aditionals];

    if (statusFilter !== "all") {
      const isActive = statusFilter === "active";
      data = data.filter(item => item.isActive === isActive);
    }

    if (groupFilter !== "all") {
      data = data.filter(item => item.aditionalGroupId === groupFilter);
    }

    return data;
  }, [aditionals, statusFilter, groupFilter]);

  const columns: ColumnDef<Aditional>[] = [
    {
      accessorKey: "name",
      header: ({ column }) => {
        return (
          <Button
            variant="ghost"
            onClick={() => column.toggleSorting(column.getIsSorted() === "asc")}
          >
            Nome
            <ArrowUpDown className="ml-2 h-4 w-4" />
          </Button>
        );
      },
    },
    {
      accessorKey: "price",
      header: "Preço",
      cell: ({ row }) => {
        const price = parseFloat(row.getValue("price"));
        const formatted = new Intl.NumberFormat("pt-BR", {
          style: "currency",
          currency: "BRL",
        }).format(price);
        return <div>{formatted}</div>;
      },
    },
    {
      accessorKey: "aditionalGroupId",
      header: "Grupo",
      cell: ({ row }) => {
        const groupId = row.getValue("aditionalGroupId") as string;
        const group = groups.find(g => g.id === groupId);
        return <div>{group?.name || "N/A"}</div>;
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
        const aditional = row.original;
        const canEdit = can("ADITIONAL", "UPDATE");
        const canDelete = can("ADITIONAL", "DELETE");

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
                <DropdownMenuItem onClick={() => openEditForm(aditional)}>
                  <Edit className="mr-2 h-4 w-4" />
                  Editar
                </DropdownMenuItem>
              )}
              {canDelete && (
                <DropdownMenuItem
                  onClick={() => setDeleteId(aditional.id)}
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
          <h2 className="text-3xl font-bold tracking-tight text-zinc-900 dark:text-zinc-50">Adicionais</h2>
          <p className="text-muted-foreground mt-1">
            Gerencie todos os itens adicionais disponíveis.
          </p>
        </div>
        <PermissionGate module="ADITIONAL" operation="CREATE">
          <Button onClick={openNewForm} className="shrink-0 w-full sm:w-auto">
            <Plus className="mr-2 h-4 w-4" />
            Novo Adicional
          </Button>
        </PermissionGate>
      </div>

      <Card className="border-none shadow-md bg-white dark:bg-zinc-800">
        <CardHeader>
          <CardTitle>Listagem de Adicionais</CardTitle>
          <div className="flex flex-col lg:flex-row gap-4 pt-4 justify-between items-start lg:items-center">
            <div className="flex flex-col sm:flex-row w-full lg:w-auto gap-2">
              <div className="relative w-full sm:w-72">
                <Search className="absolute left-2 top-2.5 h-4 w-4 text-muted-foreground" />
                <Input
                  placeholder="Buscar adicionais..."
                  value={globalFilter ?? ""}
                  onChange={(event) => setGlobalFilter(event.target.value)}
                  className="pl-8 w-full"
                />
              </div>
              <Select value={groupFilter} onValueChange={setGroupFilter}>
                <SelectTrigger className="w-full sm:w-[200px]">
                  <div className="flex items-center gap-2">
                    <Filter className="h-4 w-4 text-muted-foreground" />
                    <SelectValue placeholder="Filtrar por grupo" />
                  </div>
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">Todos os grupos</SelectItem>
                  {groups.map((group) => (
                    <SelectItem key={group.id} value={String(group.id)}>
                      {group.name}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>

              <Select value={statusFilter} onValueChange={setStatusFilter}>
                <SelectTrigger className="w-full sm:w-[180px]">
                  <div className="flex items-center gap-2">
                    <Filter className="h-4 w-4 text-muted-foreground" />
                    <SelectValue placeholder="Status" />
                  </div>
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">Todos</SelectItem>
                  <SelectItem value="active">Ativos</SelectItem>
                  <SelectItem value="inactive">Inativos</SelectItem>
                </SelectContent>
              </Select>
            </div>
            <div className="text-sm text-muted-foreground whitespace-nowrap">
              Total: {filteredData.length} adicionais
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
                      );
                    })}
                  </TableRow>
                ))}
              </TableHeader>
              <TableBody>
                {isLoadingAditionals ? (
                  <TableRow>
                    <TableCell colSpan={columns.length} className="h-24 text-center">
                      Carregando...
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
                          {flexRender(
                            cell.column.columnDef.cell,
                            cell.getContext()
                          )}
                        </TableCell>
                      ))}
                    </TableRow>
                  ))
                ) : (
                  <TableRow>
                    <TableCell colSpan={columns.length} className="h-24 text-center">
                      Nenhum adicional encontrado.
                    </TableCell>
                  </TableRow>
                )}
              </TableBody>
            </Table>
          </div>

          <div className="flex items-center justify-end space-x-2 pt-4">
            <div className="flex-1 text-sm text-muted-foreground">
              {table.getFilteredRowModel().rows.length} registro(s)
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

      <AditionalForm
        open={isFormOpen}
        onOpenChange={setIsFormOpen}
        onSubmit={handleCreate}
        initialData={editingAditional || undefined}
        groups={groups}
        isLoading={createMutation.isPending || updateMutation.isPending}
      />

      <AlertDialog open={!!deleteId} onOpenChange={(open: boolean) => !open && setDeleteId(null)}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Você tem certeza?</AlertDialogTitle>
            <AlertDialogDescription>
              Esta ação não pode ser desfeita. Isso excluirá permanentemente o adicional.
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
