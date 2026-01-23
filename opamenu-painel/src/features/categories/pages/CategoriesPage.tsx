import { useState, useMemo } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { 
  Plus, 
  Edit, 
  Trash2, 
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

import { categoriesService } from "../categories.service";
import type { Category, CreateCategoryRequest, UpdateCategoryRequest } from "../types";
import { CategoryForm } from "../components/CategoryForm";
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

export default function CategoriesPage() {
  const { can } = usePermission();
  const [isFormOpen, setIsFormOpen] = useState(false);
  const [editingCategory, setEditingCategory] = useState<Category | null>(null);
  const [deleteId, setDeleteId] = useState<string | null>(null);
  
  // Table states
  const [sorting, setSorting] = useState<SortingState>([]);
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([]);
  const [globalFilter, setGlobalFilter] = useState("");
  const [statusFilter, setStatusFilter] = useState<string>("all");

  const queryClient = useQueryClient();
  const { toast } = useToast();

  const { data: categories = [], isLoading } = useQuery({
    queryKey: ["categories"],
    queryFn: categoriesService.getCategories,
  });

  const createMutation = useMutation({
    mutationFn: categoriesService.createCategory,
    onSuccess: (response) => {
      queryClient.invalidateQueries({ queryKey: ["categories"] });
      const message = response.message || "Categoria criada com sucesso";
      toast({ variant: "success", title: "Sucesso", description: message });
      setIsFormOpen(false);
    },
    onError: (error) => {
      const message = getErrorMessage(error);
      toast({ title: "Erro", description: message, variant: "destructive" });
    },
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: UpdateCategoryRequest }) =>
      categoriesService.updateCategory(id, data),
    onSuccess: (response) => {
      queryClient.invalidateQueries({ queryKey: ["categories"] });
      const message = response.message || "Categoria atualizada com sucesso";
      toast({ variant: "success", title: "Sucesso", description: message });
      setIsFormOpen(false);
      setEditingCategory(null);
    },
    onError: (error) => {
      const message = getErrorMessage(error);
      toast({ title: "Erro", description: message, variant: "destructive" });
    },
  });

  const deleteMutation = useMutation({
    mutationFn: categoriesService.deleteCategory,
    onSuccess: (response) => {
      queryClient.invalidateQueries({ queryKey: ["categories"] });
      const message = response.message || "Categoria removida com sucesso";
      toast({ variant: "success", title: "Sucesso", description: message });
      setDeleteId(null);
    },
    onError: (error) => {
      const message = getErrorMessage(error);
      toast({ title: "Erro", description: message, variant: "destructive" });
    },
  });

  const handleCreate = (data: CreateCategoryRequest | UpdateCategoryRequest) => {
    if (editingCategory) {
      updateMutation.mutate({ id: editingCategory.id, data: data as UpdateCategoryRequest });
    } else {
      createMutation.mutate(data as CreateCategoryRequest);
    }
  };

  const openEditForm = (category: Category) => {
    setEditingCategory(category);
    setIsFormOpen(true);
  };

  const confirmDelete = () => {
    if (deleteId) {
      deleteMutation.mutate(deleteId);
    }
  };

  const openNewForm = () => {
    setEditingCategory(null);
    setIsFormOpen(true);
  };

  // Filter logic
  const filteredData = useMemo(() => {
    let data = [...categories];
    if (statusFilter !== "all") {
      const isActive = statusFilter === "active";
      data = data.filter(item => item.isActive === isActive);
    }
    return data;
  }, [categories, statusFilter]);

  const columns: ColumnDef<Category>[] = [
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
      accessorKey: "description",
      header: "Descrição",
      cell: ({ row }) => <div className="truncate max-w-[300px]">{row.getValue("description") || "-"}</div>,
    },
    {
      accessorKey: "displayOrder",
      header: "Ordem",
      cell: ({ row }) => <div className="text-center w-12">{row.getValue("displayOrder")}</div>,
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
        const category = row.original;
        const canEdit = can("CATEGORY", "UPDATE");
        const canDelete = can("CATEGORY", "DELETE");

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
                <DropdownMenuItem onClick={() => openEditForm(category)}>
                  <Edit className="mr-2 h-4 w-4" />
                  Editar
                </DropdownMenuItem>
              )}
              {canDelete && (
                <>
                  <DropdownMenuSeparator />
                  <DropdownMenuItem 
                    onClick={() => setDeleteId(category.id)}
                    className="text-red-600 focus:text-red-600"
                  >
                    <Trash2 className="mr-2 h-4 w-4" />
                    Excluir
                  </DropdownMenuItem>
                </>
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
          <h2 className="text-3xl font-bold tracking-tight text-zinc-900 dark:text-zinc-50">Categorias</h2>
          <p className="text-muted-foreground mt-1">
            Gerencie as categorias de produtos do seu cardápio.
          </p>
        </div>
        <PermissionGate module="CATEGORY" operation="CREATE">
          <Button onClick={openNewForm} className="shrink-0 w-full sm:w-auto">
            <Plus className="mr-2 h-4 w-4" />
            Nova Categoria
          </Button>
        </PermissionGate>
      </div>

      <Card className="border-none shadow-md bg-white dark:bg-zinc-800">
        <CardHeader>
          <CardTitle>Listagem de Categorias</CardTitle>
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
              Total: {filteredData.length} categorias
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
                      Carregando categorias...
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
                      Nenhuma categoria encontrada.
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

      <CategoryForm
        open={isFormOpen}
        onOpenChange={setIsFormOpen}
        onSubmit={handleCreate}
        initialData={editingCategory || undefined}
        isLoading={createMutation.isPending || updateMutation.isPending}
      />

      <AlertDialog open={!!deleteId} onOpenChange={(open: boolean) => !open && setDeleteId(null)}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Você tem certeza?</AlertDialogTitle>
            <AlertDialogDescription>
              Esta ação não pode ser desfeita. Isso excluirá permanentemente a categoria.
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
