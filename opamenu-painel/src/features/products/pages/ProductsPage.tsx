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
  ImageIcon,
  Layers
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

import { productsService } from "../products.service";
import { categoriesService } from "@/features/categories/categories.service";
import type { Product, CreateProductRequest, UpdateProductRequest } from "../types";
import { ProductForm } from "../components/ProductForm";
import { ProductAddonGroupManager } from "../components/ProductAddonGroupManager";
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

export default function ProductsPage() {
  const { can } = usePermission();
  const [isFormOpen, setIsFormOpen] = useState(false);
  const [editingProduct, setEditingProduct] = useState<Product | null>(null);
  const [deleteId, setDeleteId] = useState<string | null>(null);
  const [manageAddonGroupsProductId, setManageAddonGroupsProductId] = useState<string | null>(null);
  
  // Table states
  const [sorting, setSorting] = useState<SortingState>([]);
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([]);
  const [globalFilter, setGlobalFilter] = useState("");
  const [categoryFilter, setCategoryFilter] = useState<string>("all");
  const [statusFilter, setStatusFilter] = useState<string>("all");

  const queryClient = useQueryClient();
  const { toast } = useToast();

  const { data: products = [], isLoading } = useQuery({
    queryKey: ["products"],
    queryFn: () => productsService.getProducts(),
  });

  const { data: categories = [] } = useQuery({
    queryKey: ["categories"],
    queryFn: categoriesService.getCategories,
  });

  const createMutation = useMutation({
    mutationFn: productsService.createProduct,
    onSuccess: (response) => {
      queryClient.invalidateQueries({ queryKey: ["products"] });
      const message = response.message || "Produto criado com sucesso";
      toast({ variant: "success", title: "Sucesso", description: message });
      setIsFormOpen(false);
    },
    onError: (error) => {
      console.error("Erro ao criar produto:", error);
      const message = getErrorMessage(error);
      toast({ title: "Erro", description: message, variant: "destructive" });
    },
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: UpdateProductRequest }) =>
      productsService.updateProduct(id, data),
    onSuccess: (response) => {
      queryClient.invalidateQueries({ queryKey: ["products"] });
      const message = response.message || "Produto atualizado com sucesso";
      toast({ variant: "success", title: "Sucesso", description: message });
      setIsFormOpen(false);
      setEditingProduct(null);
    },
    onError: (error) => {
      console.error("Erro ao atualizar produto:", error);
      const message = getErrorMessage(error);
      toast({ title: "Erro", description: message, variant: "destructive" });
    },
  });

  const deleteMutation = useMutation({
    mutationFn: productsService.deleteProduct,
    onSuccess: (response) => {
      queryClient.invalidateQueries({ queryKey: ["products"] });
      const message = response.message || "Produto excluído com sucesso";
      toast({ variant: "success", title: "Sucesso", description: message });
      setDeleteId(null);
    },
    onError: (error) => {
      console.error("Erro ao excluir produto:", error);
      const message = getErrorMessage(error);
      toast({ title: "Erro", description: message, variant: "destructive" });
    },
  });

  const toggleStatusMutation = useMutation({
    mutationFn: productsService.toggleProductStatus,
    onSuccess: (response) => {
      queryClient.invalidateQueries({ queryKey: ["products"] });
      const message = response.message || "Status alterado com sucesso";
      toast({ variant: "success", title: "Sucesso", description: message });
    },
    onError: (error) => {
      console.error("Erro ao alterar status do produto:", error);
      const message = getErrorMessage(error);
      toast({ title: "Erro", description: message, variant: "destructive" });
    },
  });

  const openNewForm = () => {
    setEditingProduct(null);
    setIsFormOpen(true);
  };

  const openEditForm = (product: Product) => {
    setEditingProduct(product);
    setIsFormOpen(true);
  };

  const handleSubmit = (data: CreateProductRequest | UpdateProductRequest) => {
    if (editingProduct) {
      updateMutation.mutate({ id: editingProduct.id, data: data as UpdateProductRequest });
    } else {
      createMutation.mutate(data as CreateProductRequest);
    }
  };

  const handleDelete = () => {
    if (deleteId) {
      deleteMutation.mutate(deleteId);
    }
  };

  // Filter logic
  const filteredData = useMemo(() => {
    let result = products;

    if (categoryFilter !== "all") {
      result = result.filter((p) => p.categoryId.toString() === categoryFilter);
    }

    if (statusFilter !== "all") {
      const isActive = statusFilter === "active";
      result = result.filter((p) => p.isActive === isActive);
    }

    return result;
  }, [products, categoryFilter, statusFilter]);

  const columns: ColumnDef<Product>[] = [
    {
      accessorKey: "imageUrl",
      header: "Imagem",
      cell: ({ row }) => {
        const imageUrl = row.original.imageUrl;
        return (
          <div className="h-10 w-10 rounded-md overflow-hidden bg-muted flex items-center justify-center">
            {imageUrl ? (
              <img src={imageUrl} alt={row.original.name} className="h-full w-full object-cover" />
            ) : (
              <ImageIcon className="h-5 w-5 text-muted-foreground" />
            )}
          </div>
        );
      },
    },
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
      cell: ({ row }) => (
        <div className="flex flex-col">
          <span className="font-medium">{row.getValue("name")}</span>
          <span className="text-xs text-muted-foreground truncate max-w-[200px]">
            {row.original.description}
          </span>
        </div>
      ),
    },
    {
      accessorKey: "categoryName",
      header: ({ column }) => {
        return (
          <Button
            variant="ghost"
            onClick={() => column.toggleSorting(column.getIsSorted() === "asc")}
          >
            Categoria
            <ArrowUpDown className="ml-2 h-4 w-4" />
          </Button>
        );
      },
    },
    {
      accessorKey: "price",
      header: ({ column }) => {
        return (
          <Button
            variant="ghost"
            onClick={() => column.toggleSorting(column.getIsSorted() === "asc")}
          >
            Preço
            <ArrowUpDown className="ml-2 h-4 w-4" />
          </Button>
        );
      },
      cell: ({ row }) => {
        const amount = parseFloat(row.getValue("price"));
        const formatted = new Intl.NumberFormat("pt-BR", {
          style: "currency",
          currency: "BRL",
        }).format(amount);
        return <div className="font-medium">{formatted}</div>;
      },
    },
    {
      accessorKey: "isActive",
      header: "Status",
      cell: ({ row }) => {
        const isActive = row.getValue("isActive") as boolean;
        return (
          <Badge variant={isActive ? "default" : "secondary"} className={isActive ? "bg-emerald-500 hover:bg-emerald-600" : ""}>
            {isActive ? "Ativo" : "Inativo"}
          </Badge>
        );
      },
    },
    {
      id: "actions",
      cell: ({ row }) => {
        const product = row.original;
        const canEdit = can("PRODUCT", "UPDATE");
        const canDelete = can("PRODUCT", "DELETE");

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
                  <DropdownMenuItem onClick={() => openEditForm(product)}>
                    <Edit className="mr-2 h-4 w-4" />
                    Editar
                  </DropdownMenuItem>
                  <DropdownMenuItem onClick={() => setManageAddonGroupsProductId(product.id)}>
                    <Layers className="mr-2 h-4 w-4" />
                    Gerenciar Adicionais
                  </DropdownMenuItem>
                  <DropdownMenuItem onClick={() => toggleStatusMutation.mutate(product.id)}>
                    <ArrowUpDown className="mr-2 h-4 w-4" />
                    {product.isActive ? "Desativar" : "Ativar"}
                  </DropdownMenuItem>
                </>
              )}
              {canEdit && canDelete && <DropdownMenuSeparator />}
              {canDelete && (
                <DropdownMenuItem 
                  onClick={() => setDeleteId(product.id)}
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
    onSortingChange: setSorting,
    onColumnFiltersChange: setColumnFilters,
    getCoreRowModel: getCoreRowModel(),
    getPaginationRowModel: getPaginationRowModel(),
    getSortedRowModel: getSortedRowModel(),
    getFilteredRowModel: getFilteredRowModel(),
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
          <h2 className="text-3xl font-bold tracking-tight text-zinc-900 dark:text-zinc-50">Produtos</h2>
          <p className="text-muted-foreground mt-1">Gerencie os produtos do seu cardápio.</p>
        </div>
        <PermissionGate module="PRODUCT" operation="CREATE">
          <Button onClick={openNewForm} className="shrink-0 w-full sm:w-auto">
            <Plus className="mr-2 h-4 w-4" />
            Novo Produto
          </Button>
        </PermissionGate>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Lista de Produtos</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="flex flex-col lg:flex-row gap-4 mb-6">
            <div className="relative flex-1">
              <Search className="absolute left-2 top-2.5 h-4 w-4 text-muted-foreground" />
              <Input
                placeholder="Buscar produtos..."
                value={globalFilter ?? ""}
                onChange={(event) => setGlobalFilter(event.target.value)}
                className="pl-8 w-full"
              />
            </div>
            <div className="flex flex-col sm:flex-row gap-4">
              <Select value={categoryFilter} onValueChange={setCategoryFilter}>
                <SelectTrigger className="w-full sm:w-[200px]">
                  <div className="flex items-center gap-2">
                    <Filter className="h-4 w-4 text-muted-foreground" />
                    <SelectValue placeholder="Categoria" />
                  </div>
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">Todas as Categorias</SelectItem>
                  {categories.map((category) => (
                    <SelectItem key={category.id} value={category.id.toString()}>
                      {category.name}
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
          </div>

          <div className="rounded-md border overflow-hidden">
             <div className="overflow-x-auto">
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
                  {table.getRowModel().rows?.length ? (
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
                      <TableCell
                        colSpan={columns.length}
                        className="h-24 text-center"
                      >
                        {isLoading ? (
                           <div className="flex items-center justify-center py-4">
                            <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary"></div>
                          </div>
                        ) : (
                          "Nenhum produto encontrado."
                        )}
                      </TableCell>
                    </TableRow>
                  )}
                </TableBody>
              </Table>
            </div>
          </div>

          <div className="flex items-center justify-end space-x-2 py-4">
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
              Próximo
            </Button>
          </div>
        </CardContent>
      </Card>

      <ProductForm
        open={isFormOpen}
        onOpenChange={setIsFormOpen}
        onSubmit={handleSubmit}
        initialData={editingProduct || undefined}
        isLoading={createMutation.isPending || updateMutation.isPending}
      />

      <ProductAddonGroupManager
        productId={manageAddonGroupsProductId}
        productName={products.find(p => p.id === manageAddonGroupsProductId)?.name}
        open={!!manageAddonGroupsProductId}
        onOpenChange={(open) => !open && setManageAddonGroupsProductId(null)}
      />

      <AlertDialog open={!!deleteId} onOpenChange={(open) => !open && setDeleteId(null)}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Você tem certeza?</AlertDialogTitle>
            <AlertDialogDescription>
              Essa ação não pode ser desfeita. Isso excluirá permanentemente o produto.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancelar</AlertDialogCancel>
            <AlertDialogAction
              onClick={handleDelete}
              className="bg-red-600 hover:bg-red-700 focus:ring-red-600"
            >
              {deleteMutation.isPending ? "Excluindo..." : "Excluir"}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  );
}
