import { useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { Loader2, Calendar, CreditCard, ShieldCheck, AlertTriangle, ExternalLink } from "lucide-react";

import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Separator } from "@/components/ui/separator";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogTrigger,
} from "@/components/ui/alert-dialog";
import { useToast } from "@/hooks/use-toast";
import { subscriptionService } from "../subscription.service";
import { usePermission } from "@/hooks/usePermission";

export default function PlanPage() {
  const { can } = usePermission();
  const { toast } = useToast();
  const queryClient = useQueryClient();
  const [isCancelDialogOpen, setIsCancelDialogOpen] = useState(false);

  const canUpdate = can("SUBSCRIPTION", "UPDATE");

  const { data: subscription, isLoading, error } = useQuery({
    queryKey: ["subscription-status"],
    queryFn: subscriptionService.getStatus,
  });

  const cancelMutation = useMutation({
    mutationFn: subscriptionService.cancelSubscription,
    onSuccess: () => {
      toast({
        title: "Assinatura cancelada",
        description: "Sua assinatura será cancelada ao final do período atual.",
      });
      setIsCancelDialogOpen(false);
      queryClient.invalidateQueries({ queryKey: ["subscription-status"] });
    },
    onError: (error) => {
      toast({
        variant: "destructive",
        title: "Erro ao cancelar",
        description: "Não foi possível cancelar sua assinatura. Tente novamente.",
      });
      console.error(error);
    },
  });

  const billingPortalMutation = useMutation({
    mutationFn: subscriptionService.getBillingPortalUrl,
    onSuccess: (url) => {
      window.location.href = url;
    },
    onError: (error) => {
      toast({
        variant: "destructive",
        title: "Erro",
        description: "Não foi possível redirecionar para o portal de pagamento.",
      });
      console.error(error);
    },
  });

  const handleCancelSubscription = () => {
    cancelMutation.mutate({});
  };

  const handleManageBilling = () => {
    billingPortalMutation.mutate();
  };

  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-full">
        <Loader2 className="w-8 h-8 animate-spin text-primary" />
      </div>
    );
  }

  if (error || !subscription) {
    return (
      <div className="flex flex-col items-center justify-center h-full gap-4">
        <AlertTriangle className="w-12 h-12 text-destructive" />
        <h2 className="text-lg font-semibold">Erro ao carregar informações do plano</h2>
        <p className="text-muted-foreground">
          Não foi possível buscar os detalhes da sua assinatura. Tente novamente mais tarde.
        </p>
        <Button onClick={() => window.location.reload()}>Tentar novamente</Button>
      </div>
    );
  }

  const formatDate = (dateString: string) => {
    if (!dateString) return "N/A";
    return new Date(dateString).toLocaleDateString("pt-BR", {
      day: "2-digit",
      month: "long",
      year: "numeric",
    });
  };

  const getStatusColor = (status: string) => {
    switch (status.toLowerCase()) {
      case "active":
        return "default"; // green/primary
      case "trialing":
        return "secondary"; // blueish
      case "past_due":
        return "destructive"; // red
      case "canceled":
        return "destructive";
      default:
        return "outline";
    }
  };

  const getStatusLabel = (status: string) => {
    switch (status.toLowerCase()) {
      case "active":
        return "Ativo";
      case "trialing":
        return "Período de Teste";
      case "past_due":
        return "Pagamento Pendente";
      case "canceled":
        return "Cancelado";
      default:
        return status;
    }
  };

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold tracking-tight">Meu Plano</h1>
        <p className="text-muted-foreground">
          Gerencie sua assinatura e visualize detalhes do seu plano atual.
        </p>
      </div>

      <div className="grid gap-6 md:grid-cols-2">
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center justify-between">
              <span>Detalhes da Assinatura</span>
              <Badge variant={getStatusColor(subscription.status) as any}>
                {getStatusLabel(subscription.status)}
              </Badge>
            </CardTitle>
            <CardDescription>Informações sobre seu plano atual</CardDescription>
          </CardHeader>
          <CardContent className="space-y-6">
            <div className="flex items-center justify-between">
              <div className="flex items-center gap-2">
                <ShieldCheck className="w-5 h-5 text-muted-foreground" />
                <span className="font-medium">Plano Atual</span>
              </div>
              <span className="text-lg font-bold">{subscription.planName}</span>
            </div>
            
            <Separator />

            <div className="flex items-center justify-between">
              <div className="flex items-center gap-2">
                <Calendar className="w-5 h-5 text-muted-foreground" />
                <span className="font-medium">Próxima Renovação</span>
              </div>
              <span>{formatDate(subscription.currentPeriodEnd)}</span>
            </div>

            <Separator />

            <div className="flex items-center justify-between">
              <div className="flex items-center gap-2">
                <CreditCard className="w-5 h-5 text-muted-foreground" />
                <span className="font-medium">Dias Restantes</span>
              </div>
              <Badge variant="outline" className="text-base px-3 py-1">
                {subscription.daysRemaining} dias
              </Badge>
            </div>

            {subscription.isTrial && (
              <div className="mt-4 p-4 bg-secondary/20 rounded-lg border border-secondary text-secondary-foreground text-sm">
                Seu período de teste acaba em {subscription.trialEndsAt ? formatDate(subscription.trialEndsAt) : "breve"}.
              </div>
            )}

            {subscription.cancelAtPeriodEnd && (
               <div className="mt-4 p-4 bg-destructive/10 rounded-lg border border-destructive/20 text-destructive text-sm font-medium">
                 Sua assinatura foi cancelada e encerrará em {formatDate(subscription.currentPeriodEnd)}.
               </div>
            )}
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Ações</CardTitle>
            <CardDescription>Gerencie sua assinatura</CardDescription>
          </CardHeader>
          <CardContent className="space-y-4">
            <p className="text-sm text-muted-foreground">
              Gerencie seus métodos de pagamento e faturas diretamente no nosso portal seguro.
            </p>
            
            <Button 
              className="w-full gap-2" 
              onClick={handleManageBilling}
              disabled={billingPortalMutation.isPending || !canUpdate}
            >
              {billingPortalMutation.isPending && <Loader2 className="w-4 h-4 animate-spin" />}
              <ExternalLink className="w-4 h-4" />
              Gerenciar Cobrança e Faturas
            </Button>

            {!subscription.cancelAtPeriodEnd && subscription.status !== 'canceled' && canUpdate && (
              <>
                <div className="relative py-2">
                  <div className="absolute inset-0 flex items-center">
                    <span className="w-full border-t" />
                  </div>
                  <div className="relative flex justify-center text-xs uppercase">
                    <span className="bg-background px-2 text-muted-foreground">
                      Zona de Perigo
                    </span>
                  </div>
                </div>

                <AlertDialog open={isCancelDialogOpen} onOpenChange={setIsCancelDialogOpen}>
                  <AlertDialogTrigger asChild>
                    <Button 
                      variant="ghost" 
                      className="w-full text-destructive hover:text-destructive hover:bg-destructive/10"
                    >
                      Cancelar Assinatura
                    </Button>
                  </AlertDialogTrigger>
                  <AlertDialogContent>
                    <AlertDialogHeader>
                      <AlertDialogTitle>Tem certeza que deseja cancelar?</AlertDialogTitle>
                      <AlertDialogDescription>
                        Ao cancelar, você perderá acesso aos recursos premium ao final do período atual ({formatDate(subscription.currentPeriodEnd)}).
                        Esta ação pode ser desfeita entrando em contato com o suporte antes do término do período.
                      </AlertDialogDescription>
                    </AlertDialogHeader>
                    <AlertDialogFooter>
                      <AlertDialogCancel>Voltar</AlertDialogCancel>
                      <AlertDialogAction 
                        onClick={handleCancelSubscription}
                        className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
                        disabled={cancelMutation.isPending}
                      >
                        {cancelMutation.isPending ? (
                          <Loader2 className="w-4 h-4 animate-spin mr-2" />
                        ) : null}
                        Confirmar Cancelamento
                      </AlertDialogAction>
                    </AlertDialogFooter>
                  </AlertDialogContent>
                </AlertDialog>
              </>
            )}
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
