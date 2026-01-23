import { useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { Plus, Edit, Trash2 } from "lucide-react";
import { addonsService } from "../addons.service";
import type { Addon, CreateAddonRequest, UpdateAddonRequest } from "../types";
import { AddonForm } from "./AddonForm";
import { Button } from "@/components/ui/button";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import {
  Sheet,
  SheetContent,
  SheetDescription,
  SheetHeader,
  SheetTitle,
} from "@/components/ui/sheet";
import { Badge } from "@/components/ui/badge";
import { useToast } from "@/hooks/use-toast";
import { getErrorMessage } from "@/lib/utils";
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

interface AddonManagerProps {
  groupId: string | null;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export function AddonManager({ groupId, open, onOpenChange }: AddonManagerProps) {
  const [isFormOpen, setIsFormOpen] = useState(false);
  const [editingAddon, setEditingAddon] = useState<Addon | null>(null);
  const [deleteId, setDeleteId] = useState<string | null>(null);
  
  const queryClient = useQueryClient();
  const { toast } = useToast();

  const { data: group, isLoading } = useQuery({
    queryKey: ["addon-group", groupId],
    queryFn: () => addonsService.getGroupById(groupId!),
    enabled: !!groupId && open,
  });

  const createMutation = useMutation({
    mutationFn: addonsService.createAddon,
    onSuccess: (response) => {
      queryClient.invalidateQueries({ queryKey: ["addon-group", groupId] });
      // Also invalidate main list as it might show counts or previews
      queryClient.invalidateQueries({ queryKey: ["addon-groups"] }); 
      toast({ variant: "success", title: "Sucesso", description: response.message || "Item adicionado com sucesso" });
      setIsFormOpen(false);
    },
    onError: (error) => {
      const message = getErrorMessage(error);
      toast({ title: "Erro", description: message, variant: "destructive" });
    },
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: UpdateAddonRequest }) =>
      addonsService.updateAddon(id, data),
    onSuccess: (response) => {
      queryClient.invalidateQueries({ queryKey: ["addon-group", groupId] });
      queryClient.invalidateQueries({ queryKey: ["addon-groups"] });
      toast({ variant: "success", title: "Sucesso", description: response.message || "Item atualizado com sucesso" });
      setIsFormOpen(false);
      setEditingAddon(null);
    },
    onError: (error) => {
      const message = getErrorMessage(error);
      toast({ title: "Erro", description: message, variant: "destructive" });
    },
  });

  const deleteMutation = useMutation({
    mutationFn: addonsService.deleteAddon,
    onSuccess: (response) => {
      queryClient.invalidateQueries({ queryKey: ["addon-group", groupId] });
      queryClient.invalidateQueries({ queryKey: ["addon-groups"] });
      toast({ variant: "success", title: "Sucesso", description: response.message || "Item removido com sucesso" });
      setDeleteId(null);
    },
    onError: (error) => {
      const message = getErrorMessage(error);
      toast({ title: "Erro", description: message, variant: "destructive" });
    },
  });

  const toggleStatusMutation = useMutation({
      mutationFn: addonsService.toggleAddonStatus,
      onSuccess: (response) => {
          queryClient.invalidateQueries({ queryKey: ["addon-group", groupId] });
          toast({ variant: "success", title: "Sucesso", description: response.message || "Status alterado" });
      },
      onError: (error) => {
          const message = getErrorMessage(error);
          toast({ title: "Erro", description: message, variant: "destructive" });
      }
  });

  const handleCreate = (data: CreateAddonRequest | UpdateAddonRequest) => {
    if (editingAddon) {
      updateMutation.mutate({ id: editingAddon.id, data: data as UpdateAddonRequest });
    } else {
      createMutation.mutate(data as CreateAddonRequest);
    }
  };

  const handleEdit = (addon: Addon) => {
    setEditingAddon(addon);
    setIsFormOpen(true);
  };

  const handleDelete = () => {
    if (deleteId) {
      deleteMutation.mutate(deleteId);
    }
  };

  const openNewForm = () => {
    setEditingAddon(null);
    setIsFormOpen(true);
  };

  return (
    <Sheet open={open} onOpenChange={onOpenChange}>
      <SheetContent className="sm:max-w-xl w-full overflow-y-auto">
        <SheetHeader className="mb-6">
          <SheetTitle>Gerenciar Itens</SheetTitle>
          <SheetDescription>
            {group ? `Itens do grupo: ${group.name}` : "Carregando..."}
          </SheetDescription>
        </SheetHeader>

        {isLoading ? (
            <div className="flex justify-center py-8">Carregando itens...</div>
        ) : (
            <div className="space-y-6">
                <SheetHeader className="mb-6">
                    <h3 className="text-sm font-medium">Lista de Itens ({group?.addons?.length || 0})</h3>
                    <Button size="sm" onClick={openNewForm}>
                        <Plus className="mr-2 h-4 w-4" /> Novo Item
                    </Button>
                </SheetHeader>

                <div className="rounded-md border">
                    <Table>
                        <TableHeader>
                            <TableRow>
                                <TableHead>Nome</TableHead>
                                <TableHead>Preço</TableHead>
                                <TableHead>Status</TableHead>
                                <TableHead className="text-right">Ações</TableHead>
                            </TableRow>
                        </TableHeader>
                        <TableBody>
                            {group?.addons && group.addons.length > 0 ? (
                                group.addons.map((addon) => (
                                    <TableRow key={addon.id}>
                                        <TableCell className="font-medium">
                                            <div className="flex flex-col">
                                                <span>{addon.name}</span>
                                                {addon.description && (
                                                    <span className="text-xs text-muted-foreground truncate max-w-[150px]">{addon.description}</span>
                                                )}
                                            </div>
                                        </TableCell>
                                        <TableCell>
                                            {addon.price > 0 ? (
                                                <span className="text-green-600 font-medium">
                                                    + {addon.price.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })}
                                                </span>
                                            ) : (
                                                <Badge variant="secondary" className="text-[10px]">Grátis</Badge>
                                            )}
                                        </TableCell>
                                        <TableCell>
                                             <Button 
                                                variant="ghost" 
                                                size="sm" 
                                                className={`h-6 px-2 ${addon.isActive ? 'text-green-600' : 'text-muted-foreground'}`}
                                                onClick={() => toggleStatusMutation.mutate(addon.id)}
                                                disabled={toggleStatusMutation.isPending}
                                             >
                                                {addon.isActive ? "Ativo" : "Inativo"}
                                             </Button>
                                        </TableCell>
                                        <TableCell className="text-right">
                                            <div className="flex justify-end gap-1">
                                                <Button
                                                    variant="ghost"
                                                    size="icon"
                                                    className="h-8 w-8"
                                                    onClick={() => handleEdit(addon)}
                                                >
                                                    <Edit className="h-4 w-4" />
                                                </Button>
                                                <Button
                                                    variant="ghost"
                                                    size="icon"
                                                    className="h-8 w-8 text-destructive hover:text-destructive"
                                                    onClick={() => setDeleteId(addon.id)}
                                                >
                                                    <Trash2 className="h-4 w-4" />
                                                </Button>
                                            </div>
                                        </TableCell>
                                    </TableRow>
                                ))
                            ) : (
                                <TableRow>
                                    <TableCell colSpan={4} className="text-center h-24 text-muted-foreground">
                                        Nenhum item neste grupo.
                                    </TableCell>
                                </TableRow>
                            )}
                        </TableBody>
                    </Table>
                </div>
            </div>
        )}

        {/* Forms and Dialogs */}
        <AddonForm 
            open={isFormOpen}
            onOpenChange={setIsFormOpen}
            onSubmit={handleCreate}
            initialData={editingAddon}
            groupId={groupId!}
            isLoading={createMutation.isPending || updateMutation.isPending}
        />

        <AlertDialog open={!!deleteId} onOpenChange={() => setDeleteId(null)}>
            <AlertDialogContent>
            <AlertDialogHeader>
                <AlertDialogTitle>Excluir item?</AlertDialogTitle>
                <AlertDialogDescription>
                Esta ação não pode ser desfeita.
                </AlertDialogDescription>
            </AlertDialogHeader>
            <AlertDialogFooter>
                <AlertDialogCancel>Cancelar</AlertDialogCancel>
                <AlertDialogAction
                className="bg-destructive hover:bg-destructive/90"
                onClick={handleDelete}
                >
                Excluir
                </AlertDialogAction>
            </AlertDialogFooter>
            </AlertDialogContent>
        </AlertDialog>

      </SheetContent>
    </Sheet>
  );
}
