import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { useToast } from "@/hooks/use-toast";
import { LoyaltyForm } from "../components/LoyaltyForm";
import { loyaltyService } from "../loyalty.service";
import type { CreateLoyaltyProgramRequest, LoyaltyProgram, UpdateLoyaltyProgramRequest } from "../types";
import { Loader2, Plus, Pencil, Trash2, Award, Calendar } from "lucide-react";
import { usePermission } from "@/hooks/usePermission";
import { PermissionGate } from "@/components/auth/PermissionGate";
import { useState } from "react";
import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
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
import { Card, CardContent, CardDescription, CardHeader, CardTitle, CardFooter } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { ELoyaltyProgramType } from "../types";

export default function LoyaltyPage() {
  const { can } = usePermission();
  const { toast } = useToast();
  const queryClient = useQueryClient();
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [editingProgram, setEditingProgram] = useState<LoyaltyProgram | null>(null);
  const [deleteId, setDeleteId] = useState<string | null>(null);

  const { data: programs, isLoading } = useQuery({
    queryKey: ["loyalty-programs"],
    queryFn: loyaltyService.getPrograms,
  });

  const createMutation = useMutation({
    mutationFn: loyaltyService.createProgram,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["loyalty-programs"] });
      toast({ title: "Sucesso", description: "Programa de fidelidade criado com sucesso." });
      setIsDialogOpen(false);
    },
    onError: () => toast({ title: "Erro", description: "Não foi possível criar o programa.", variant: "destructive" }),
  });

  const updateMutation = useMutation({
    mutationFn: loyaltyService.updateProgram,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["loyalty-programs"] });
      toast({ title: "Sucesso", description: "Programa de fidelidade atualizado com sucesso." });
      setIsDialogOpen(false);
    },
    onError: () => toast({ title: "Erro", description: "Não foi possível atualizar o programa.", variant: "destructive" }),
  });

  const deleteMutation = useMutation({
    mutationFn: loyaltyService.deleteProgram,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["loyalty-programs"] });
      toast({ title: "Sucesso", description: "Programa removido com sucesso." });
    },
    onError: () => toast({ title: "Erro", description: "Não foi possível remover o programa.", variant: "destructive" }),
  });

  const handleSubmit = (data: any) => {
    const payload = {
      ...data,
      pointsValidityDays: data.pointsValidityDays || undefined,
    };

    if (editingProgram) {
      updateMutation.mutate({ ...payload, id: editingProgram.id } as UpdateLoyaltyProgramRequest);
    } else {
      createMutation.mutate(payload as CreateLoyaltyProgramRequest);
    }
  };

  const handleEdit = (program: LoyaltyProgram) => {
    setEditingProgram(program);
    setIsDialogOpen(true);
  };

  const handleAdd = () => {
    setEditingProgram(null);
    setIsDialogOpen(true);
  };

  const handleDelete = (id: string) => {
    setDeleteId(id);
  };

  const confirmDelete = () => {
    if (deleteId) {
      deleteMutation.mutate(deleteId);
      setDeleteId(null);
    }
  };

  if (isLoading) {
    return (
      <div className="flex h-full items-center justify-center">
        <Loader2 className="h-8 w-8 animate-spin" />
      </div>
    );
  }

  const canUpdate = can("LOYALTY", "UPDATE");
  const canDelete = can("LOYALTY", "DELETE");
  const canCreate = can("LOYALTY", "CREATE") || can("LOYALTY", "INSERT");

  return (
    <PermissionGate module="LOYALTY" operation="READ" fallback={
      <div className="flex h-[400px] items-center justify-center">
        <p className="text-muted-foreground">Você não tem permissão para acessar o programa de fidelidade.</p>
      </div>
    }>
      <div className="space-y-6 animate-in fade-in duration-500">
        <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
          <div>
            <h2 className="text-3xl font-bold tracking-tight text-zinc-900 dark:text-zinc-50">Fidelidade</h2>
            <p className="text-muted-foreground mt-1">
              Gerencie seus programas de recompensas e fidelize seus clientes.
            </p>
          </div>
          {canCreate && (
            <Button onClick={handleAdd} className="w-full sm:w-auto">
              <Plus className="mr-2 h-4 w-4" /> Novo Programa
            </Button>
          )}
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {programs?.map((p) => (
            <Card key={p.id} className={`group relative overflow-hidden transition-all hover:shadow-md ${p.isActive ? "border-primary/20" : "opacity-70 grayscale-[0.5]"}`}>
              <CardHeader className="pb-3 relative">
                <div className="flex justify-between items-start mb-2">
                  <Badge variant={p.isActive ? "default" : "secondary"} className={p.isActive ? "bg-emerald-500 hover:bg-emerald-600" : ""}>
                    {p.isActive ? "Ativo" : "Inativo"}
                  </Badge>
                  <div className="flex gap-1 opacity-0 group-hover:opacity-100 transition-opacity">
                    {canUpdate && (
                      <Button variant="ghost" size="icon" className="h-8 w-8 hover:bg-primary/10" onClick={() => handleEdit(p)}>
                        <Pencil className="h-4 w-4" />
                      </Button>
                    )}
                    {canDelete && (
                      <Button variant="ghost" size="icon" className="h-8 w-8 text-destructive hover:bg-destructive/10" onClick={() => handleDelete(p.id)}>
                        <Trash2 className="h-4 w-4" />
                      </Button>
                    )}
                  </div>
                </div>
                <CardTitle className="flex items-center gap-2 text-xl">
                  <div className={`p-2 rounded-lg ${p.isActive ? "bg-primary/10 text-primary" : "bg-muted text-muted-foreground"}`}>
                    <Award className="h-5 w-5" />
                  </div>
                  {p.name}
                </CardTitle>
                <CardDescription className="line-clamp-2 min-h-[40px] mt-2">
                  {p.description || "Sem descrição disponível."}
                </CardDescription>
              </CardHeader>
              <CardContent className="pb-4 text-sm space-y-4">
                <div className="flex items-center gap-3 p-3 rounded-md bg-muted/50 border border-border/50">
                  <Calendar className="h-5 w-5 text-muted-foreground" />
                  <div className="flex flex-col">
                    <span className="font-semibold text-zinc-700 dark:text-zinc-300">Regra de Acúmulo</span>
                    <span className="text-muted-foreground text-xs uppercase tracking-wider font-medium">
                      {p.type === ELoyaltyProgramType.PointsPerValue && "Valor Gasto (Cashback)"}
                      {p.type === ELoyaltyProgramType.OrderCount && "Frequência de Pedidos"}
                      {p.type === ELoyaltyProgramType.ItemCount && "Produtos Específicos"}
                    </span>
                  </div>
                </div>

                <div className="space-y-1">
                  <div className="flex justify-between text-xs font-medium text-muted-foreground mb-1">
                    <span>Meta de Resgate</span>
                    <span>{p.targetCount || 0} pontos</span>
                  </div>
                  <div className="h-2 w-full bg-muted rounded-full overflow-hidden">
                    <div
                      className={`h-full ${p.isActive ? "bg-primary" : "bg-muted-foreground/30"}`}
                      style={{ width: "100%" }}
                    />
                  </div>
                </div>

                {p.filters && p.filters.length > 0 && (
                  <div className="flex items-center gap-2 text-xs text-muted-foreground">
                    <div className="h-1.5 w-1.5 rounded-full bg-primary" />
                    {p.filters.length} {p.filters.length === 1 ? "filtro aplicado" : "filtros aplicados"}
                  </div>
                )}
              </CardContent>
              <CardFooter className="pt-0 border-t bg-muted/20 py-3 flex justify-between items-center">
                <div className="text-xs font-medium flex items-center gap-1.5">
                  {p.isActive ? (
                    <>
                      <div className="h-2 w-2 rounded-full bg-emerald-500 animate-pulse" />
                      <span className="text-emerald-600 dark:text-emerald-400">Em vigor</span>
                    </>
                  ) : (
                    <>
                      <div className="h-2 w-2 rounded-full bg-zinc-400" />
                      <span className="text-zinc-500">Pausado</span>
                    </>
                  )}
                </div>
                {p.pointsValidityDays && (
                  <span className="text-[10px] text-muted-foreground uppercase tracking-tighter font-bold">
                    Expira em {p.pointsValidityDays} dias
                  </span>
                )}
              </CardFooter>
            </Card>
          ))}
          {programs?.length === 0 && (
            <div className="col-span-full py-20 text-center border-2 border-dashed rounded-xl bg-muted/5">
              <div className="mx-auto h-20 w-20 rounded-full bg-muted/30 flex items-center justify-center mb-6">
                <Award className="h-10 w-10 text-muted-foreground/40" />
              </div>
              <h3 className="text-2xl font-bold text-zinc-800 dark:text-zinc-200">Nenhum programa cadastrado</h3>
              <p className="text-muted-foreground max-w-sm mx-auto mt-2">
                Crie seu primeiro programa de fidelidade agora mesmo e começe a recompensar seus clientes mais fiéis.
              </p>
              {canCreate && (
                <Button variant="default" className="mt-8 px-8" onClick={handleAdd}>
                  <Plus className="mr-2 h-4 w-4" /> Criar Meu Primeiro Programa
                </Button>
              )}
            </div>
          )}
        </div>

        <Dialog open={isDialogOpen} onOpenChange={setIsDialogOpen}>
          <DialogContent className="sm:max-w-5xl max-h-[90vh] overflow-y-auto">
            <DialogHeader>
              <DialogTitle>{editingProgram ? "Editar Programa" : "Novo Programa"}</DialogTitle>
              <DialogDescription>
                Configure as regras do seu programa de benefícios.
              </DialogDescription>
            </DialogHeader>
            <LoyaltyForm
              key={editingProgram?.id || "new"}
              initialData={editingProgram}
              onSubmit={handleSubmit}
              isLoading={createMutation.isPending || updateMutation.isPending}
              readOnly={!canUpdate}
            />
          </DialogContent>
        </Dialog>

        <AlertDialog open={!!deleteId} onOpenChange={(open) => !open && setDeleteId(null)}>
          <AlertDialogContent>
            <AlertDialogHeader>
              <AlertDialogTitle>Você tem certeza?</AlertDialogTitle>
              <AlertDialogDescription>
                Esta ação não pode ser desfeita. Isso excluirá permanentemente o programa de fidelidade.
              </AlertDialogDescription>
            </AlertDialogHeader>
            <AlertDialogFooter>
              <AlertDialogCancel>Cancelar</AlertDialogCancel>
              <AlertDialogAction
                onClick={confirmDelete}
                className="bg-red-600 hover:bg-red-700 focus:ring-red-600"
              >
                {deleteMutation.isPending ? "Excluindo..." : "Excluir"}
              </AlertDialogAction>
            </AlertDialogFooter>
          </AlertDialogContent>
        </AlertDialog>
      </div>
    </PermissionGate>
  );
}
