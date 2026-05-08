import { useNavigate } from "react-router-dom";
import { Check, Loader2, Rocket, Zap, Star, ShieldCheck, Clock, TrendingUp, Smartphone, Menu as MenuIcon, X, ArrowRight } from "lucide-react";
import { useQuery, useMutation } from "@tanstack/react-query";
import { useState, useEffect } from "react";

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
import { OpaMenuLogo } from "@/components/common/OpaMenuLogo";
import { cn } from "@/lib/utils";

export default function PlansPage() {
  const navigate = useNavigate();
  const { toast } = useToast();
  const { setAccessToken, setUser, isAuthenticated } = useAuthStore();
  const [isMenuOpen, setIsMenuOpen] = useState(false);
  const [scrolled, setScrolled] = useState(false);

  useEffect(() => {
    const handleScroll = () => setScrolled(window.scrollY > 50);
    window.addEventListener("scroll", handleScroll);
    return () => window.removeEventListener("scroll", handleScroll);
  }, []);
  
  // Buscar planos
  const { data: plans = [], isLoading } = useQuery({
    queryKey: ["plans"],
    queryFn: authService.getPlans,
  });

  // Ativar Plano
  const { mutate: activatePlan, isPending: isActivating } = useMutation({
    mutationFn: authService.activatePlan,
    onSuccess: async (response) => {
        if (response.succeeded) {
            toast({
                title: "Plano ativado com sucesso!",
                description: "Bem-vindo ao Opamenu.",
            });
            const currentToken = useAuthStore.getState().accessToken;
            const currentRefreshToken = useAuthStore.getState().refreshToken;
            if (currentToken && currentRefreshToken) {
                setAccessToken(currentToken, currentRefreshToken, false);
                try {
                  const permissions = await authService.getPermissions();
                  if (permissions.succeeded) {
                    setUser(permissions.data);
                  }
                } catch (error) {
                  console.error("Erro ao buscar permissões atualizadas", error);
                }
            }
            navigate("/dashboard");
        } else {
            toast({
                variant: "destructive",
                title: "Erro",
                description: response.message || "Não foi possível ativar o plano.",
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
      <div className="flex h-screen items-center justify-center bg-zinc-950">
        <Loader2 className="h-10 w-10 animate-spin text-[#F37021]" />
      </div>
    );
  }

  const benefits = [
    {
      icon: <TrendingUp className="h-6 w-6" />,
      title: "Aumento de Faturamento",
      description: "Nossa interface inteligente sugere adicionais e combos, aumentando o ticket médio em até 30%."
    },
    {
      icon: <Zap className="h-6 w-6" />,
      title: "Atendimento Ultra Rápido",
      description: "Reduza o tempo de espera eliminando etapas manuais. O pedido vai direto da mesa para a cozinha."
    },
    {
      icon: <Star className="h-6 w-6" />,
      title: "Experiência do Cliente",
      description: "Ofereça modernidade com um cardápio digital intuitivo, fotos premium e facilidade no pagamento."
    },
    {
      icon: <Smartphone className="h-6 w-6" />,
      title: "Gestão na Palma da Mão",
      description: "Acompanhe suas vendas, estoque e equipe em tempo real, de qualquer lugar do mundo."
    }
  ];

  const features = [
    "Cardápio Digital via QR Code",
    "Gestão de Pedidos em Tempo Real",
    "Controle de Estoque e Categorias",
    "Relatórios Detalhados de Vendas",
    "Suporte Especializado 24/7"
  ];

  return (
    <div className="min-h-screen bg-white text-zinc-900 selection:bg-[#F37021] selection:text-white overflow-x-hidden">
      {/* Navigation */}
      <nav className={cn(
        "fixed top-0 w-full z-50 transition-all duration-300 px-6 py-4 flex items-center justify-between",
        scrolled ? "bg-white/80 backdrop-blur-md shadow-sm border-b" : "bg-transparent"
      )}>
        <OpaMenuLogo size="small" />
        
        <div className="hidden md:flex items-center gap-8 font-medium">
          <a href="#beneficios" className="hover:text-[#F37021] transition-colors">Benefícios</a>
          <a href="#como-funciona" className="hover:text-[#F37021] transition-colors">Como Funciona</a>
          <a href="#planos" className="hover:text-[#F37021] transition-colors">Planos</a>
          {isAuthenticated ? (
            <Button onClick={() => navigate("/dashboard")} className="bg-[#F37021] hover:bg-[#D65D18]">Dashboard</Button>
          ) : (
            <Button variant="outline" onClick={() => navigate("/login")} className="border-[#F37021] text-[#F37021] hover:bg-[#F37021]/10">Entrar</Button>
          )}
        </div>

        <button className="md:hidden" onClick={() => setIsMenuOpen(!isMenuOpen)}>
          {isMenuOpen ? <X /> : <MenuIcon />}
        </button>
      </nav>

      {/* Mobile Menu */}
      {isMenuOpen && (
        <div className="fixed inset-0 z-40 bg-white flex flex-col items-center justify-center gap-8 text-2xl font-bold animate-in fade-in duration-300">
          <a href="#beneficios" onClick={() => setIsMenuOpen(false)}>Benefícios</a>
          <a href="#como-funciona" onClick={() => setIsMenuOpen(false)}>Como Funciona</a>
          <a href="#planos" onClick={() => setIsMenuOpen(false)}>Planos</a>
          <Button onClick={() => navigate("/login")} className="text-xl px-12 py-6 bg-[#F37021]">Entrar</Button>
        </div>
      )}

      {/* Hero Section */}
      <section className="relative pt-32 pb-20 md:pt-48 md:pb-32 px-6">
        <div className="container mx-auto grid lg:grid-cols-2 gap-12 items-center">
          <div className="space-y-8 animate-in slide-in-from-left duration-700">
            <div className="inline-flex items-center gap-2 px-4 py-2 rounded-full bg-[#F37021]/10 text-[#F37021] text-sm font-bold uppercase tracking-wider">
              <Rocket className="h-4 w-4" />
              O futuro do seu restaurante chegou
            </div>
            <h1 className="text-5xl md:text-7xl font-extrabold leading-[1.1] tracking-tight">
              Transforme cliques em <span className="text-[#F37021]">pedidos lucrativos.</span>
            </h1>
            <p className="text-xl text-gray-600 max-w-xl leading-relaxed">
              O Opamenu não é apenas um cardápio digital. É o motor de crescimento que seu negócio precisa para escalar, automatizar e encantar cada cliente.
            </p>
            <div className="flex flex-col sm:flex-row gap-4">
              <Button size="lg" className="bg-[#F37021] hover:bg-[#D65D18] text-lg px-8 py-7 shadow-xl shadow-[#F37021]/20 group" onClick={() => document.getElementById('planos')?.scrollIntoView({ behavior: 'smooth' })}>
                Começar Agora <ArrowRight className="ml-2 h-5 w-5 group-hover:translate-x-1 transition-transform" />
              </Button>
              <Button size="lg" variant="outline" className="text-lg px-8 py-7 border-zinc-200 hover:bg-zinc-50" onClick={() => document.getElementById('como-funciona')?.scrollIntoView({ behavior: 'smooth' })}>
                Saiba Mais
              </Button>
            </div>
          </div>
          <div className="relative animate-in zoom-in duration-1000">
            <div className="absolute -inset-4 bg-gradient-to-tr from-[#F37021]/20 to-transparent rounded-3xl blur-3xl" />
            <img 
              src="/hero-landing.png" 
              alt="Opamenu Dashboard" 
              className="relative rounded-2xl shadow-2xl border border-zinc-100 object-cover aspect-[4/3]"
            />
            {/* Floating Badges */}
            <div className="absolute -bottom-6 -left-6 bg-white p-4 rounded-xl shadow-xl border border-zinc-50 hidden md:block animate-bounce duration-[3s]">
              <div className="flex items-center gap-3">
                <div className="p-2 rounded-full bg-green-100 text-green-600"><TrendingUp className="h-5 w-5" /></div>
                <div>
                  <p className="text-xs text-gray-500 font-medium">Ticket Médio</p>
                  <p className="text-lg font-bold text-green-600">+28%</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* Stats Section */}
      <section className="bg-zinc-950 py-16 text-white overflow-hidden relative">
        <div className="container mx-auto px-6 grid md:grid-cols-3 gap-12 text-center relative z-10">
          <div>
            <p className="text-4xl md:text-5xl font-black text-[#F37021] mb-2">500+</p>
            <p className="text-zinc-400 font-medium uppercase tracking-widest text-sm">Restaurantes Parceiros</p>
          </div>
          <div>
            <p className="text-4xl md:text-5xl font-black text-[#F37021] mb-2">2M+</p>
            <p className="text-zinc-400 font-medium uppercase tracking-widest text-sm">Pedidos Processados</p>
          </div>
          <div>
            <p className="text-4xl md:text-5xl font-black text-[#F37021] mb-2">99.9%</p>
            <p className="text-zinc-400 font-medium uppercase tracking-widest text-sm">Disponibilidade</p>
          </div>
        </div>
        <div className="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-full h-full opacity-10 pointer-events-none">
           <div className="absolute inset-0 bg-[radial-gradient(circle_at_center,_var(--tw-gradient-stops))] from-[#F37021] via-transparent to-transparent blur-3xl"></div>
        </div>
      </section>

      {/* Benefits Section */}
      <section id="beneficios" className="py-24 md:py-32 bg-zinc-50/50">
        <div className="container mx-auto px-6">
          <div className="text-center max-w-3xl mx-auto mb-20 space-y-4">
            <h2 className="text-[#F37021] font-bold uppercase tracking-widest text-sm">Por que escolher o Opamenu?</h2>
            <p className="text-4xl md:text-5xl font-extrabold tracking-tight">Vantagens que o seu negócio sente no <span className="italic">bolso</span>.</p>
          </div>
          <div className="grid md:grid-cols-2 lg:grid-cols-4 gap-8">
            {benefits.map((benefit, i) => (
              <div key={i} className="bg-white p-8 rounded-2xl shadow-sm border border-zinc-100 hover:shadow-xl hover:-translate-y-1 transition-all duration-300">
                <div className="w-14 h-14 bg-[#F37021]/10 text-[#F37021] rounded-xl flex items-center justify-center mb-6">
                  {benefit.icon}
                </div>
                <h3 className="text-xl font-bold mb-3">{benefit.title}</h3>
                <p className="text-gray-500 leading-relaxed">{benefit.description}</p>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* Features Showcase */}
      <section id="como-funciona" className="py-24 bg-white">
        <div className="container mx-auto px-6 grid lg:grid-cols-2 gap-20 items-center">
          <div className="order-2 lg:order-1 relative">
            <div className="bg-gradient-to-br from-zinc-100 to-white rounded-3xl p-8 shadow-inner border border-zinc-200">
               <ul className="space-y-6">
                 {features.map((feature, i) => (
                   <li key={i} className="flex items-center gap-4 bg-white p-5 rounded-xl shadow-sm border border-zinc-50 transform transition-transform hover:scale-105" style={{ transitionDelay: `${i * 100}ms` }}>
                      <div className="bg-green-500 rounded-full p-1"><Check className="h-4 w-4 text-white" /></div>
                      <span className="text-lg font-semibold">{feature}</span>
                   </li>
                 ))}
               </ul>
            </div>
          </div>
          <div className="order-1 lg:order-2 space-y-8">
            <h2 className="text-4xl md:text-5xl font-extrabold tracking-tight">Tudo o que você precisa em uma <span className="text-[#F37021]">única plataforma.</span></h2>
            <div className="grid grid-cols-2 gap-6">
               <div className="space-y-2">
                 <div className="font-bold text-2xl">01.</div>
                 <div className="font-semibold text-lg text-[#F37021]">Configure</div>
                 <p className="text-sm text-gray-500">Cadastre seus produtos e categorias em minutos.</p>
               </div>
               <div className="space-y-2">
                 <div className="font-bold text-2xl">02.</div>
                 <div className="font-semibold text-lg text-[#F37021]">Compartilhe</div>
                 <p className="text-sm text-gray-500">Imprima QR Codes ou envie o link pelo WhatsApp.</p>
               </div>
            </div>
          </div>
        </div>
      </section>

      {/* Pricing Section */}
      <section id="planos" className="py-24 md:py-32 bg-zinc-950 text-white relative overflow-hidden">
        <div className="container mx-auto px-6 relative z-10">
          <div className="text-center max-w-3xl mx-auto mb-20 space-y-4">
            <h2 className="text-[#F37021] font-bold uppercase tracking-widest text-sm">Preços e Planos</h2>
            <p className="text-4xl md:text-5xl font-extrabold tracking-tight">O investimento que se <span className="text-[#F37021]">paga sozinho.</span></p>
            <p className="text-zinc-400 text-lg">Escolha o plano que melhor se adapta ao tamanho do seu sonho.</p>
          </div>

          <div className="flex flex-wrap justify-center gap-8">
            {plans.map((plan) => (
              <Card key={plan.id} className={cn(
                "flex flex-col w-full max-w-sm border-0 transition-all duration-300 hover:scale-[1.02]",
                plan.price > 0 ? "bg-white text-zinc-900 shadow-2xl" : "bg-zinc-900 text-white border border-zinc-800"
              )}>
                <CardHeader className="text-center">
                  <div className="mx-auto mb-4 w-12 h-12 bg-[#F37021]/10 rounded-full flex items-center justify-center">
                    {plan.price === 0 ? <Zap className="h-6 w-6 text-[#F37021]" /> : <Star className="h-6 w-6 text-[#F37021]" />}
                  </div>
                  <CardTitle className="text-2xl font-bold">{plan.name}</CardTitle>
                  <CardDescription className={plan.price > 0 ? "text-gray-500" : "text-zinc-400"}>
                    {plan.description}
                  </CardDescription>
                </CardHeader>
                <CardContent className="flex-1 px-8">
                  <div className="mb-8 text-center">
                    <div className="text-5xl font-black">
                      <span className="text-2xl align-top mr-1 font-normal">R$</span>
                      {plan.price.toFixed(2)}
                    </div>
                    <p className={cn("text-sm mt-2", plan.price > 0 ? "text-gray-400" : "text-zinc-500")}>
                      Cobrado {plan.billingCycle === 'monthly' ? 'mensalmente' : 'anualmente'}
                    </p>
                  </div>
                  <ul className="space-y-4 mb-8">
                    {[
                      "Acesso total aos módulos",
                      "Suporte prioritário",
                      "Painel de controle intuitivo",
                      plan.price > 0 ? "Sem limites de pedidos" : "Ideal para começar"
                    ].map((feature, i) => (
                      <li key={i} className="flex items-center gap-3">
                        <Check className="h-5 w-5 text-green-500 shrink-0" />
                        <span className="text-sm font-medium">{feature}</span>
                      </li>
                    ))}
                  </ul>
                </CardContent>
                <CardFooter className="px-8 pb-8">
                  <Button 
                    className={cn(
                      "w-full py-6 text-lg font-bold shadow-lg transition-all",
                      plan.price > 0 
                        ? "bg-[#F37021] hover:bg-[#D65D18] shadow-[#F37021]/20" 
                        : "bg-zinc-800 hover:bg-zinc-700 text-white"
                    )}
                    onClick={() => {
                      if (plan.price > 0) {
                        toast({
                          title: "Redirecionando...",
                          description: "Em breve: Integração com checkout seguro.",
                        });
                        navigate(`/checkout/${plan.id}`);
                      } else {
                        activatePlan(plan.id);
                      }
                    }}
                    disabled={isActivating}
                  >
                    {isActivating && plan.price === 0 ? <Loader2 className="mr-2 h-4 w-4 animate-spin" /> : null}
                    {plan.price > 0 ? 'Assinar Agora' : 'Começar Grátis'}
                  </Button>
                </CardFooter>
              </Card>
            ))}
          </div>
        </div>
        
        {/* Background elements for dark section */}
        <div className="absolute bottom-0 right-0 w-[500px] h-[500px] bg-[#F37021]/5 rounded-full blur-[120px] pointer-events-none" />
        <div className="absolute top-0 left-0 w-[300px] h-[300px] bg-blue-500/5 rounded-full blur-[100px] pointer-events-none" />
      </section>

      {/* CTA Section */}
      <section className="py-24 bg-[#F37021]">
        <div className="container mx-auto px-6 text-center text-white space-y-8">
           <h2 className="text-4xl md:text-6xl font-black">Pronto para dobrar seus pedidos?</h2>
           <p className="text-xl opacity-90 max-w-2xl mx-auto">Junte-se a centenas de estabelecimentos que já estão colhendo os frutos da digitalização com o Opamenu.</p>
           <Button size="lg" className="bg-white text-[#F37021] hover:bg-zinc-100 text-xl px-12 py-8 font-bold shadow-2xl" onClick={() => navigate("/register")}>
              Criar minha conta agora
           </Button>
           <p className="text-sm opacity-75">Sem cartão de crédito. Comece em 2 minutos.</p>
        </div>
      </section>

      {/* Footer */}
      <footer className="py-12 bg-white border-t border-zinc-100">
        <div className="container mx-auto px-6 flex flex-col md:flex-row justify-between items-center gap-8">
          <OpaMenuLogo size="small" />
          <div className="flex gap-8 text-gray-500 text-sm font-medium">
            <a href="#" className="hover:text-[#F37021]">Termos</a>
            <a href="#" className="hover:text-[#F37021]">Privacidade</a>
            <a href="#" className="hover:text-[#F37021]">Suporte</a>
          </div>
          <p className="text-gray-400 text-sm">© 2026 Opamenu. Todos os direitos reservados.</p>
        </div>
      </footer>
    </div>
  );
}

