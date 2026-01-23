import { useState, useMemo } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { 
  Plus, 
  Edit, 
  Trash2, 
  MoreHorizontal, 
  ArrowUpDown,
  Search
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
import { format } from "date-fns";

import { couponsService } from "../coupons.service";
import { DiscountType, type Coupon, type CreateCouponRequest, type UpdateCouponRequest } from "../types";
import { CouponForm } from "../components/CouponForm";
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
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { useToast } from "@/hooks/use-toast";
import { getErrorMessage } from "@/lib/utils";
import { usePermission } from "@/hooks/usePermission";
import { PermissionGate } from "@/components/auth/PermissionGate";

export default function CouponsPage() {
  const { can } = usePermission();
  const [isFormOpen, setIsFormOpen] = useState(false);
  const [editingCoupon, setEditingCoupon] = useState<Coupon | null>(null);
  const [deleteId, setDeleteId] = useState<string | null>(null);
  
  // Table states
  const [sorting, setSorting] = useState<SortingState>([]);
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([]);
  const [globalFilter, setGlobalFilter] = useState("");

  const queryClient = useQueryClient();
  const { toast } = useToast();

  const { data: coupons = [], isLoading } = useQuery({
    queryKey: ["coupons"],
    queryFn: couponsService.getCoupons,
  });

  const createMutation = useMutation({
    mutationFn: couponsService.createCoupon,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["coupons"] });
      toast({ variant: "default", title: "Sucesso", description: "Cupom criado com sucesso" });
      setIsFormOpen(false);
    },
    onError: (error) => {
      const message = getErrorMessage(error);
      toast({ title: "Erro", description: message, variant: "destructive" });
    },
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: UpdateCouponRequest }) => 
      couponsService.updateCoupon(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["coupons"] });
      toast({ variant: "default", title: "Sucesso", description: "Cupom atualizado com sucesso" });
      setIsFormOpen(false);
      setEditingCoupon(null);
    },
    onError: (error) => {
      const message = getErrorMessage(error);
      toast({ title: "Erro", description: message, variant: "destructive" });
    },
  });

  const deleteMutation = useMutation({
    mutationFn: couponsService.deleteCoupon,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["coupons"] });
      toast({ variant: "default", title: "Sucesso", description: "Cupom excluído com sucesso" });
      setDeleteId(null);
    },
    onError: (error) => {
      const message = getErrorMessage(error);
      toast({ title: "Erro", description: message, variant: "destructive" });
    },
  });

  const handleCreate = (data: CreateCouponRequest | UpdateCouponRequest) => {
    // Cast to specific types based on context (editing or creating)
    if (editingCoupon) {
      updateMutation.mutate({ 
        id: editingCoupon.id, 
        data: data as UpdateCouponRequest 
      });
    } else {
      createMutation.mutate(data as CreateCouponRequest);
    }
  };

  const handleDelete = () => {
    if (deleteId) {
      deleteMutation.mutate(deleteId);
    }
  };

  const openEditForm = (coupon: Coupon) => {
    setEditingCoupon(coupon);
    setIsFormOpen(true);
  };

  const columns: ColumnDef<Coupon>[] = useMemo(() => [
    {
      accessorKey: "code",
      header: ({ column }) => {
        return (
          <Button
            variant="ghost"
            onClick={() => column.toggleSorting(column.getIsSorted() === "asc")}
          >
            Código
            <ArrowUpDown className="ml-2 h-4 w-4" />
          </Button>
        )
      },
      cell: ({ row }) => <div className="font-medium px-4">{row.getValue("code")}</div>,
    },
    {
      accessorKey: "discountValue",
      header: "Desconto",
      cell: ({ row }) => {
        const type = row.original.discountType;
        const value = parseFloat(row.getValue("discountValue"));
        return (
          <div>
            {type === DiscountType.Percentage ? `${value}%` : `R$ ${value.toFixed(2)}`}
          </div>
        )
      },
    },
    {
      accessorKey: "usage",
      header: "Uso",
      cell: ({ row }) => {
        const limit = row.original.usageLimit;
        const count = row.original.usageCount;
        return (
          <div>
            {count} {limit ? `/ ${limit}` : "(Ilimitado)"}
          </div>
        )
      },
    },
    {
      accessorKey: "expirationDate",
      header: "Validade",
      cell: ({ row }) => {
        const date = row.getValue("expirationDate") as string | undefined;
        if (!date) return <span className="text-muted-foreground">Sem validade</span>;
        return <span>{format(new Date(date), "dd/MM/yyyy HH:mm")}</span>;
      },
    },
    {
      accessorKey: "isActive",
      header: "Status",
      cell: ({ row }) => {
        const isActive = row.getValue("isActive") as boolean;
        return (
          <Badge variant={isActive ? "default" : "secondary"}>
            {isActive ? "Ativo" : "Inativo"}
          </Badge>
        )
      },
    },
    {
      id: "actions",
      cell: ({ row }) => {
        const coupon = row.original;
        const canEdit = can("COUPON", "UPDATE");
        const canDelete = can("COUPON", "DELETE");

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
              <DropdownMenuItem onClick={() => navigator.clipboard.writeText(coupon.code)}>
                Copiar código
              </DropdownMenuItem>
              {(canEdit || canDelete) && <DropdownMenuSeparator />}
              {canEdit && (
                <DropdownMenuItem onClick={() => openEditForm(coupon)}>
                  <Edit className="mr-2 h-4 w-4" /> Editar
                </DropdownMenuItem>
              )}
              {canDelete && (
                <DropdownMenuItem 
                  onClick={() => setDeleteId(coupon.id)}
                  className="text-red-600 focus:text-red-600"
                >
                  <Trash2 className="mr-2 h-4 w-4" /> Excluir
                </DropdownMenuItem>
              )}
            </DropdownMenuContent>
          </DropdownMenu>
        )
      },
    },
  ], []);

  const table = useReactTable({
    data: coupons,
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
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Cupons</h1>
          <p className="text-muted-foreground">
            Gerencie os cupons de desconto da sua loja.
          </p>
        </div>
        <PermissionGate module="COUPON" operation="CREATE">
          <Button onClick={() => { setEditingCoupon(null); setIsFormOpen(true); }}>
            <Plus className="mr-2 h-4 w-4" /> Novo Cupom
          </Button>
        </PermissionGate>
      </div>

      <Card>
        <CardHeader className="pb-3">
          <CardTitle className="text-lg font-medium">Lista de Cupons</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="flex items-center py-4">
            <div className="relative w-full max-w-sm">
              <Search className="absolute left-2 top-2.5 h-4 w-4 text-muted-foreground" />
              <Input
                placeholder="Buscar por código..."
                value={(table.getColumn("code")?.getFilterValue() as string) ?? ""}
                onChange={(event) =>
                  table.getColumn("code")?.setFilterValue(event.target.value)
                }
                className="pl-8"
              />
            </div>
          </div>
          
          <div className="rounded-md border">
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
                      {isLoading ? "Carregando..." : "Nenhum cupom encontrado."}
                    </TableCell>
                  </TableRow>
                )}
              </TableBody>
            </Table>
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

      <CouponForm
        open={isFormOpen}
        onOpenChange={setIsFormOpen}
        onSubmit={handleCreate}
        initialData={editingCoupon}
        isLoading={createMutation.isPending || updateMutation.isPending}
      />

      <AlertDialog open={!!deleteId} onOpenChange={() => setDeleteId(null)}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Você tem certeza?</AlertDialogTitle>
            <AlertDialogDescription>
              Esta ação não pode ser desfeita. Isso excluirá permanentemente o cupom.
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
