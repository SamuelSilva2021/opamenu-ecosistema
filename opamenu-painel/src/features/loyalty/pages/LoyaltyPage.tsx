import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { useToast } from "@/hooks/use-toast";
import { LoyaltyForm } from "../components/LoyaltyForm";
import { loyaltyService } from "../loyalty.service";
import type { CreateLoyaltyProgramRequest, UpdateLoyaltyProgramRequest } from "../types";
import { Loader2 } from "lucide-react";
import { usePermission } from "@/hooks/usePermission";
import { PermissionGate } from "@/components/auth/PermissionGate";

export default function LoyaltyPage() {
  const { can } = usePermission();
  const { toast } = useToast();
  const queryClient = useQueryClient();

  const { data: program, isLoading } = useQuery({
    queryKey: ["loyalty-program"],
    queryFn: loyaltyService.getProgram,
  });

  const createMutation = useMutation({
    mutationFn: loyaltyService.createProgram,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["loyalty-program"] });
      toast({
        title: "Sucesso",
        description: "Programa de fidelidade criado com sucesso.",
      });
    },
    onError: () => {
      toast({
        title: "Erro",
        description: "Não foi possível criar o programa.",
        variant: "destructive",
      });
    },
  });

  const updateMutation = useMutation({
    mutationFn: loyaltyService.updateProgram,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["loyalty-program"] });
      toast({
        title: "Sucesso",
        description: "Programa de fidelidade atualizado com sucesso.",
      });
    },
    onError: () => {
      toast({
        title: "Erro",
        description: "Não foi possível atualizar o programa.",
        variant: "destructive",
      });
    },
  });

  const handleSubmit = (data: any) => {
    // Tratamento de validade vazia -> null ou undefined para envio
    const payload = {
      ...data,
      pointsValidityDays: data.pointsValidityDays || undefined,
    };

    if (program) {
      updateMutation.mutate({ ...payload, id: program.id } as UpdateLoyaltyProgramRequest);
    } else {
      createMutation.mutate(payload as CreateLoyaltyProgramRequest);
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

  return (
    <PermissionGate module="LOYALTY" operation="READ" fallback={
      <div className="flex h-[400px] items-center justify-center">
        <p className="text-muted-foreground">Você não tem permissão para acessar o programa de fidelidade.</p>
      </div>
    }>
      <div className="space-y-6">
        <div>
          <h2 className="text-3xl font-bold tracking-tight">Fidelidade</h2>
          <p className="text-muted-foreground">
            Gerencie o programa de recompensas para seus clientes.
          </p>
        </div>

        <LoyaltyForm
          initialData={program}
          onSubmit={handleSubmit}
          isLoading={createMutation.isPending || updateMutation.isPending}
          readOnly={!canUpdate}
        />
      </div>
    </PermissionGate>
  );
}
