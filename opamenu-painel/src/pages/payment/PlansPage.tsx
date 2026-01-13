import { useNavigate } from "react-router-dom";
import { Check, Loader2 } from "lucide-react";
import { useQuery, useMutation } from "@tanstack/react-query";

import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { authService } from "@/features/auth/auth.service";
import { useAuthStore } from "@/store/auth.store";
import { useToast } from "@/hooks/use-toast";

export default function PlansPage() {
  const navigate = useNavigate();
  const { toast } = useToast();
  const { setAccessToken } = useAuthStore();
  
  // Buscar planos
  const { data: plans = [], isLoading } = useQuery({
    queryKey: ["plans"],
    queryFn: authService.getPlans,
  });

  // Ativar Plano
  const { mutate: activatePlan, isPending: isActivating } = useMutation({
    mutationFn: authService.activatePlan,
    onSuccess: (response) => {
        if (response.succeeded) {
            toast({
                title: "Plano ativado com sucesso!",
                description: "Bem-vindo ao OpaMenu.",
            });
            // Atualizar status no store (remove requiresPayment)
            const currentToken = useAuthStore.getState().accessToken;
            if (currentToken) {
                setAccessToken(currentToken, false);
            }
            navigate("/dashboard");
        } else {
            toast({
                variant: "destructive",
                title: "Erro",
                description: response.message || "Não foi possível ativar o trial.",
            });
        }
    },
    onError: () => {
        toast({
            variant: "destructive",
            title: "Erro",
            description: "Ocorreu um erro ao processar sua solicitação.",
        });
    }
  });

  if (isLoading) {
    return (
      <div className="flex h-screen items-center justify-center">
        <Loader2 className="h-8 w-8 animate-spin text-primary" />
      </div>
    );
  }

  return (
    <div className="container py-10">
      <div className="mx-auto max-w-4xl text-center">
        <h1 className="text-3xl font-bold tracking-tight sm:text-4xl">
          Escolha o plano ideal para seu negócio
        </h1>
      </div>

      <div className="mt-10 flex flex-wrap justify-center gap-8">
        {plans.map((plan) => (
          <Card key={plan.id} className="flex flex-col w-full max-w-sm">
            <CardHeader>
              <CardTitle>{plan.name}</CardTitle>
              <CardDescription>{plan.description}</CardDescription>
            </CardHeader>
            <CardContent className="flex-1">
              <div className="mb-4 text-3xl font-bold">
                R$ {plan.price.toFixed(2)}
                <span className="text-sm font-normal text-muted-foreground">
                  /{plan.billingCycle === 'monthly' ? 'mês' : 'ano'}
                </span>
              </div>
              <ul className="space-y-2 text-sm text-muted-foreground">
                {plan.features?.map((feature) => (
                  <li key={feature} className="flex items-center">
                    <Check className="mr-2 h-4 w-4 text-primary" />
                    {feature}
                  </li>
                ))}
                <li className="flex items-center">
                    <Check className="mr-2 h-4 w-4 text-primary" />
                    {plan.description}
                </li>
              </ul>
            </CardContent>
            <CardFooter>
              <Button 
                className="w-full" 
                onClick={() => activatePlan(plan.id)}
                disabled={isActivating}
              >
                {isActivating ? <Loader2 className="mr-2 h-4 w-4 animate-spin" /> : null}
                {plan.price > 0 ? 'Assinar' : 'Ativar Teste Grátis'}
              </Button>
            </CardFooter>
          </Card>
        ))}
      </div>
    </div>
  );
}
