import { useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { 
  Plus, 
  Edit, 
  Trash2, 
  MoreHorizontal, 
  ArrowUpDown,
  Search,
  Truck
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

import { deliveryAreaService } from "../delivery-areas.service";
import type { DeliveryArea, CreateDeliveryAreaRequest } from "../types";
import { DeliveryAreaForm } from "../components/DeliveryAreaForm";
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

export default function DeliveryAreasPage() {
  const { can } = usePermission();
  const [isFormOpen, setIsFormOpen] = useState(false);
  const [editingArea, setEditingArea] = useState<DeliveryArea | null>(null);
  const [deleteId, setDeleteId] = useState<string | null>(null);
  
  // Table states
  const [sorting, setSorting] = useState<SortingState>([]);
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([]);
  const [globalFilter, setGlobalFilter] = useState("");

  const queryClient = useQueryClient();
  const { toast } = useToast();

  const { data: areas = [], isLoading } = useQuery({
    queryKey: ["delivery-areas"],
    queryFn: deliveryAreaService.getDeliveryAreas,
  });

  const createMutation = useMutation({
    mutationFn: deliveryAreaService.createDeliveryArea,
    onSuccess: (response) => {
      queryClient.invalidateQueries({ queryKey: ["delivery-areas"] });
      toast({ variant: "success", title: "Sucesso", description: response.message || "Taxa de entrega criada com sucesso" });
      setIsFormOpen(false);
    },
    onError: (error) => {
      toast({ title: "Erro", description: getErrorMessage(error), variant: "destructive" });
    },
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: CreateDeliveryAreaRequest }) =>
      deliveryAreaService.updateDeliveryArea(id, data),
    onSuccess: (response) => {
      queryClient.invalidateQueries({ queryKey: ["delivery-areas"] });
      toast({ variant: "success", title: "Sucesso", description: response.message || "Taxa de entrega atualizada com sucesso" });
      setIsFormOpen(false);
      setEditingArea(null);
    },
    onError: (error) => {
      toast({ title: "Erro", description: getErrorMessage(error), variant: "destructive" });
    },
  });

  const deleteMutation = useMutation({
    mutationFn: deliveryAreaService.deleteDeliveryArea,
    onSuccess: (response) => {
      queryClient.invalidateQueries({ queryKey: ["delivery-areas"] });
      toast({ variant: "success", title: "Sucesso", description: response.message || "Taxa de entrega removida com sucesso" });
      setDeleteId(null);
    },
    onError: (error) => {
      toast({ title: "Erro", description: getErrorMessage(error), variant: "destructive" });
    },
  });

  const handleFormSubmit = (data: CreateDeliveryAreaRequest) => {
    if (editingArea) {
      updateMutation.mutate({ id: editingArea.id, data });
    } else {
      createMutation.mutate(data);
    }
  };

  const openEditForm = (area: DeliveryArea) => {
    setEditingArea(area);
    setIsFormOpen(true);
  };

  const confirmDelete = () => {
    if (deleteId) {
      deleteMutation.mutate(deleteId);
    }
  };

  const columns: ColumnDef<DeliveryArea>[] = [
    {
      accessorKey: "city",
      header: ({ column }) => (
        <Button variant="ghost" onClick={() => column.toggleSorting(column.getIsSorted() === "asc")} className="-ml-4">
          Cidade <ArrowUpDown className="ml-2 h-4 w-4" />
        </Button>
      ),
    },
    {
      accessorKey: "neighborhood",
      header: "Bairro",
      cell: ({ row }) => row.getValue("neighborhood") || "Toda a cidade",
    },
    {
      accessorKey: "fee",
      header: "Taxa",
      cell: ({ row }) => (
        <span className="font-bold text-primary">
          {new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(row.getValue("fee"))}
        </span>
      ),
    },
    {
      id: "actions",
      cell: ({ row }) => {
        const area = row.original;
        const canEdit = can("DELIVERY_AREA", "UPDATE");
        const canDelete = can("DELIVERY_AREA", "DELETE");

        return (
          <DropdownMenu>
            <DropdownMenuTrigger asChild>
              <Button variant="ghost" className="h-8 w-8 p-0">
                <MoreHorizontal className="h-4 w-4" />
              </Button>
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end">
              <DropdownMenuLabel>Ações</DropdownMenuLabel>
              {canEdit && (
                <DropdownMenuItem onClick={() => openEditForm(area)}>
                  <Edit className="mr-2 h-4 w-4" /> Editar
                </DropdownMenuItem>
              )}
              {canDelete && (
                <>
                  <DropdownMenuSeparator />
                  <DropdownMenuItem onClick={() => setDeleteId(area.id)} className="text-red-600">
                    <Trash2 className="mr-2 h-4 w-4" /> Excluir
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
    data: areas,
    columns,
    getCoreRowModel: getCoreRowModel(),
    getPaginationRowModel: getPaginationRowModel(),
    getSortedRowModel: getSortedRowModel(),
    getFilteredRowModel: getFilteredRowModel(),
    onSortingChange: setSorting,
    onColumnFiltersChange: setColumnFilters,
    onGlobalFilterChange: setGlobalFilter,
    state: { sorting, columnFilters, globalFilter },
  });

  return (
    <div className="space-y-8 animate-in fade-in duration-500">
      <div className="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4">
        <div>
          <h2 className="text-3xl font-bold tracking-tight text-zinc-900 dark:text-zinc-50 flex items-center gap-2">
            <Truck className="h-8 w-8 text-primary" />
            Taxas de Entrega
          </h2>
          <p className="text-muted-foreground mt-1">Configure taxas personalizadas por cidade e bairro.</p>
        </div>
        <PermissionGate module="DELIVERY_AREA" operation="CREATE">
          <Button onClick={() => { setEditingArea(null); setIsFormOpen(true); }} className="shrink-0 w-full sm:w-auto">
            <Plus className="mr-2 h-4 w-4" /> Nova Taxa
          </Button>
        </PermissionGate>
      </div>

      <Card className="border-none shadow-md">
        <CardHeader>
          <div className="flex items-center justify-between">
            <CardTitle>Listagem de Áreas</CardTitle>
            <div className="relative w-72">
              <Search className="absolute left-2 top-2.5 h-4 w-4 text-muted-foreground" />
              <Input
                placeholder="Buscar por cidade..."
                value={(table.getColumn("city")?.getFilterValue() as string) ?? ""}
                onChange={(event) => table.getColumn("city")?.setFilterValue(event.target.value)}
                className="pl-8"
              />
            </div>
          </div>
        </CardHeader>
        <CardContent>
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
                <TableRow><TableCell colSpan={columns.length} className="h-24 text-center">Carregando...</TableCell></TableRow>
              ) : table.getRowModel().rows?.length ? (
                table.getRowModel().rows.map((row) => (
                  <TableRow key={row.id}>{row.getVisibleCells().map((cell) => (
                    <TableCell key={cell.id}>{flexRender(cell.column.columnDef.cell, cell.getContext())}</TableCell>
                  ))}</TableRow>
                ))
              ) : (
                <TableRow><TableCell colSpan={columns.length} className="h-24 text-center">Nenhuma área configurada.</TableCell></TableRow>
              )}
            </TableBody>
          </Table>
          <div className="flex items-center justify-end space-x-2 pt-4">
            <Button variant="outline" size="sm" onClick={() => table.previousPage()} disabled={!table.getCanPreviousPage()}>Anterior</Button>
            <Button variant="outline" size="sm" onClick={() => table.nextPage()} disabled={!table.getCanNextPage()}>Próxima</Button>
          </div>
        </CardContent>
      </Card>

      <DeliveryAreaForm
        open={isFormOpen}
        onOpenChange={setIsFormOpen}
        onSubmit={handleFormSubmit}
        initialData={editingArea}
        isLoading={createMutation.isPending || updateMutation.isPending}
      />

      <AlertDialog open={!!deleteId} onOpenChange={(open) => !open && setDeleteId(null)}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Excluir taxa de entrega?</AlertDialogTitle>
            <AlertDialogDescription>Esta ação não pode ser desfeita.</AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancelar</AlertDialogCancel>
            <AlertDialogAction onClick={confirmDelete} className="bg-red-600 hover:bg-red-700">Excluir</AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  );
}
