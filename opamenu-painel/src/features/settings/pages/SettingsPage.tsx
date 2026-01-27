import { useState, useEffect } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { Loader2, Store, MapPin, Clock, CreditCard, Share2, Upload, Copy, Check, Facebook, MessageCircle, Gift, Landmark, QrCode } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Label } from "@/components/ui/label";
import { Separator } from "@/components/ui/separator";
import { Switch } from "@/components/ui/switch";
import { useToast } from "@/hooks/use-toast";
import { cn } from "@/lib/utils";
import { usePermission } from "@/hooks/usePermission";

import { settingsService } from "../settings.service";
import { filesService } from "@/services/files.service";
import type { OpeningHours, UpdateTenantBusinessRequestDto } from "../types";
import LoyaltyPage from "@/features/loyalty/pages/LoyaltyPage";
import { BankDetailsTab } from "../components/BankDetailsTab";
import { PixConfigForm } from "../components/PixConfigForm";

// Schema definition
const formSchema = z.object({
  name: z.string().min(2, "Nome deve ter pelo menos 2 caracteres"),
  logoUrl: z.string().optional(),
  description: z.string().optional(),
  phone: z.string().optional(),
  email: z.string().email("Email inválido").optional().or(z.literal("")),
  
  addressStreet: z.string().optional(),
  addressNumber: z.string().optional(),
  addressComplement: z.string().optional(),
  addressNeighborhood: z.string().optional(),
  addressCity: z.string().optional(),
  addressState: z.string().optional(),
  addressZipcode: z.string().optional(),
  
  instagramUrl: z.string().optional(),
  facebookUrl: z.string().optional(),
  whatsappNumber: z.string().optional(),
});

type FormValues = z.infer<typeof formSchema>;

export default function SettingsPage() {
  const { can } = usePermission();
  const { toast } = useToast();
  const queryClient = useQueryClient();
  const [activeTab, setActiveTab] = useState("general");
  const [isUploadingLogo, setIsUploadingLogo] = useState(false);
  const [isCepLoading, setIsCepLoading] = useState(false);
  const [copied, setCopied] = useState(false);

  const canUpdate = can("SETTINGS", "UPDATE");

  // Fetch settings
  const { data: settings, isLoading } = useQuery({
    queryKey: ["settings"],
    queryFn: settingsService.getSettings,
    staleTime: 1000 * 60 * 5, // 5 minutes
    refetchOnWindowFocus: false,
  });

  // Local state for complex fields
  const [openingHours, setOpeningHours] = useState<OpeningHours>({});
  const [paymentMethods, setPaymentMethods] = useState<string[]>([]);

  // Form setup
  const form = useForm<FormValues>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      name: "",
      logoUrl: "",
      description: "",
      phone: "",
      email: "",
      addressStreet: "",
      addressNumber: "",
      addressComplement: "",
      addressNeighborhood: "",
      addressCity: "",
      addressState: "",
      addressZipcode: "",
      instagramUrl: "",
      facebookUrl: "",
      whatsappNumber: "",
    },
  });

  // Reset form when data loads
  useEffect(() => {
    if (settings) {
      form.reset({
        name: settings.name || "",
        logoUrl: settings.logoUrl || "",
        description: settings.description || "",
        phone: settings.phone || "",
        email: settings.email || "",
        addressStreet: settings.addressStreet || "",
        addressNumber: settings.addressNumber || "",
        addressComplement: settings.addressComplement || "",
        addressNeighborhood: settings.addressNeighborhood || "",
        addressCity: settings.addressCity || "",
        addressState: settings.addressState || "",
        addressZipcode: settings.addressZipcode || "",
        instagramUrl: settings.instagramUrl || "",
        facebookUrl: settings.facebookUrl || "",
        whatsappNumber: settings.whatsappNumber || "",
      });
      
      if (settings.openingHours) setOpeningHours(settings.openingHours);
      if (settings.paymentMethods) {
          let methodsData = settings.paymentMethods;
          if (typeof methodsData === 'string') {
             try { methodsData = JSON.parse(methodsData); } catch {}
          }
          
          if (Array.isArray(methodsData)) {
              setPaymentMethods(methodsData);
          } else if (typeof methodsData === 'object' && methodsData !== null) {
              // @ts-ignore
              if (Array.isArray(methodsData.methods)) setPaymentMethods(methodsData.methods);
          }
      }
    }
  }, [settings, form]);

  // Mutation
  const mutation = useMutation({
    mutationFn: settingsService.updateSettings,
    onSuccess: () => {
      toast({
        title: "Sucesso",
        description: "Configurações atualizadas com sucesso.",
      });
      queryClient.invalidateQueries({ queryKey: ["settings"] });
    },
    onError: () => {
      toast({
        title: "Erro",
        description: "Falha ao atualizar configurações.",
        variant: "destructive",
      });
    },
  });

  const handleCepBlur = async (e: React.FocusEvent<HTMLInputElement>) => {
    const cep = e.target.value.replace(/\D/g, "");
    
    if (cep.length === 8) {
      setIsCepLoading(true);
      try {
        const response = await fetch(`https://viacep.com.br/ws/${cep}/json/`);
        const data = await response.json();

        if (!data.erro) {
          form.setValue("addressStreet", data.logradouro);
          form.setValue("addressNeighborhood", data.bairro);
          form.setValue("addressCity", data.localidade);
          form.setValue("addressState", data.uf);
          form.setFocus("addressNumber");
        } else {
          toast({
            title: "CEP não encontrado",
            description: "Verifique o CEP informado.",
            variant: "destructive",
          });
        }
      } catch (error) {
        toast({
          title: "Erro na busca",
          description: "Não foi possível buscar o endereço.",
          variant: "destructive",
        });
      } finally {
        setIsCepLoading(false);
      }
    }
  };

  const onSubmit = (data: FormValues) => {
    const paymentInfo = {
      methods: paymentMethods
    };

    const payload: UpdateTenantBusinessRequestDto = {
      ...data,
      openingHours: openingHours,
      paymentMethods: paymentInfo as any,
    };
    mutation.mutate(payload);
  };

  const handleLogoUpload = async (e: React.ChangeEvent<HTMLInputElement>) => {
      const file = e.target.files?.[0];
      if (!file) return;

      setIsUploadingLogo(true);
      try {
          const result = await filesService.uploadFile(file, "tenants/logos");
          if (result.fileUrl) {
              form.setValue("logoUrl", result.fileUrl);
              toast({ title: "Logo atualizada", description: "Imagem enviada com sucesso." });
          }
      } catch (error) {
          toast({ title: "Erro no upload", description: "Não foi possível enviar a imagem.", variant: "destructive" });
      } finally {
          setIsUploadingLogo(false);
      }
  };

  const handleCopyStoreUrl = () => {
    const baseUrl = import.meta.env.VITE_MENU_APP_URL;
    const url = `${baseUrl}${settings?.slug || ""}`;
    navigator.clipboard.writeText(url);
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  };

  const handleShareWhatsApp = () => {
    const baseUrl = import.meta.env.VITE_MENU_APP_URL;
    const url = `${baseUrl}${settings?.slug || ""}`;
    const text = `Olá! Confira o cardápio digital do ${settings?.name || "restaurante"}:\n${url}`;
    window.open(`https://wa.me/?text=${encodeURIComponent(text)}`, '_blank');
  };

  const handleShareFacebook = () => {
    const baseUrl = import.meta.env.VITE_MENU_APP_URL;
    const url = `${baseUrl}${settings?.slug || ""}`;
    window.open(`https://www.facebook.com/sharer/sharer.php?u=${encodeURIComponent(url)}`, '_blank');
  };

  const tabs = [
    { id: "general", label: "Geral", icon: Store },
    { id: "address", label: "Endereço", icon: MapPin },
    { id: "hours", label: "Horários", icon: Clock },
    { id: "payments", label: "Pagamentos", icon: CreditCard },
    { id: "pix-integration", label: "Integração PIX", icon: QrCode },
    { id: "bank-details", label: "Dados Bancários", icon: Landmark },
    { id: "social", label: "Redes Sociais", icon: Share2 },
    { id: "loyalty", label: "Fidelidade", icon: Gift },
  ];

  if (isLoading) {
    return <div className="flex h-full items-center justify-center"><Loader2 className="h-8 w-8 animate-spin" /></div>;
  }

  return (
    <div className="space-y-6 animate-in fade-in duration-500">
      <div>
        <h3 className="text-lg font-medium">Configurações</h3>
        <p className="text-sm text-muted-foreground">
          Gerencie as informações da sua loja e preferências.
        </p>
      </div>
      <Separator />
      <div className="flex flex-col space-y-8 lg:flex-row lg:space-x-12 lg:space-y-0">
        <aside className="-mx-4 lg:w-1/5">
          <nav className="flex space-x-2 lg:flex-col lg:space-x-0 lg:space-y-1">
            {tabs.map((tab) => (
              <Button
                key={tab.id}
                variant={activeTab === tab.id ? "secondary" : "ghost"}
                className={cn("justify-start", activeTab === tab.id && "bg-muted hover:bg-muted")}
                onClick={() => setActiveTab(tab.id)}
              >
                <tab.icon className="mr-2 h-4 w-4" />
                {tab.label}
              </Button>
            ))}
          </nav>
        </aside>
        <div className="flex-1 lg:max-w-2xl">
          {activeTab === "loyalty" ? (
            <LoyaltyPage />
          ) : activeTab === "bank-details" ? (
            <BankDetailsTab />
          ) : activeTab === "pix-integration" ? (
            <PixConfigForm />
          ) : (
          <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
            <fieldset disabled={!canUpdate} className="space-y-6 group-disabled:opacity-50">
            {activeTab === "general" && (
              <Card>
                <CardHeader>
                  <CardTitle>Informações da Loja</CardTitle>
                  <CardDescription>Detalhes visíveis para seus clientes.</CardDescription>
                </CardHeader>
                <CardContent className="space-y-4">
                  <div className="space-y-2">
                    <Label>Logotipo</Label>
                    <div className="flex items-center gap-4">
                        <div className="relative h-20 w-20 overflow-hidden rounded-full border bg-muted">
                            {form.watch("logoUrl") ? (
                                <img src={form.watch("logoUrl")} alt="Logo" className="h-full w-full object-cover" />
                            ) : (
                                <div className="flex h-full w-full items-center justify-center text-muted-foreground">
                                    <Store className="h-8 w-8" />
                                </div>
                            )}
                        </div>
                        <div className="flex flex-col gap-2">
                             <Button type="button" variant="outline" size="sm" className="relative" disabled={isUploadingLogo}>
                                {isUploadingLogo ? <Loader2 className="mr-2 h-4 w-4 animate-spin" /> : <Upload className="mr-2 h-4 w-4" />}
                                Alterar Logo
                                <input 
                                    type="file" 
                                    className="absolute inset-0 cursor-pointer opacity-0" 
                                    accept="image/*"
                                    onChange={handleLogoUpload}
                                />
                            </Button>
                            <p className="text-xs text-muted-foreground">JPG, PNG ou WebP. Máx 5MB.</p>
                        </div>
                    </div>
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="name">Nome (Visualização)</Label>
                    <Input id="name" {...form.register("name")} />
                    <p className="text-[0.8rem] text-muted-foreground">Nome público da sua loja.</p>
                    {form.formState.errors.name && (
                      <p className="text-sm text-destructive">{form.formState.errors.name.message}</p>
                    )}
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="description">Descrição</Label>
                    <Textarea 
                      id="description" 
                      placeholder="Fale um pouco sobre sua loja..." 
                      {...form.register("description")} 
                    />
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="phone">Telefone</Label>
                    <Input id="phone" {...form.register("phone")} />
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="email">Email de Contato</Label>
                    <Input id="email" {...form.register("email")} />
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="storeUrl">Endereço loja virtual</Label>
                    <div className="flex items-center gap-2">
                      <Input 
                        id="storeUrl" 
                        readOnly 
                        value={`${import.meta.env.VITE_MENU_APP_URL || "https://app.opamenu.com.br/"}${settings?.slug || ""}`} 
                        className="bg-muted text-muted-foreground"
                      />
                      <Button 
                        type="button" 
                        variant="outline" 
                        size="icon" 
                        onClick={handleCopyStoreUrl}
                        title="Copiar endereço"
                      >
                        {copied ? <Check className="h-4 w-4 text-green-500" /> : <Copy className="h-4 w-4" />}
                      </Button>
                      <Button 
                        type="button" 
                        variant="outline" 
                        size="icon" 
                        onClick={handleShareWhatsApp}
                        title="Compartilhar no WhatsApp"
                        className="text-green-600 hover:text-green-700 hover:bg-green-50"
                      >
                        <MessageCircle className="h-4 w-4" />
                      </Button>
                      <Button 
                        type="button" 
                        variant="outline" 
                        size="icon" 
                        onClick={handleShareFacebook}
                        title="Compartilhar no Facebook"
                        className="text-blue-600 hover:text-blue-700 hover:bg-blue-50"
                      >
                        <Facebook className="h-4 w-4" />
                      </Button>
                    </div>
                  </div>
                </CardContent>
              </Card>
            )}

            {activeTab === "address" && (
              <Card>
                 <CardHeader>
                  <CardTitle>Endereço</CardTitle>
                  <CardDescription>Localização física do estabelecimento.</CardDescription>
                </CardHeader>
                <CardContent className="space-y-4">
                  <div className="grid grid-cols-2 gap-4">
                    <div className="space-y-2 col-span-2">
                        <Label htmlFor="zipcode">CEP</Label>
                        <div className="relative w-[150px]">
                          <Input 
                            id="zipcode" 
                            {...form.register("addressZipcode", {
                              onBlur: handleCepBlur
                            })} 
                            placeholder="00000-000"
                            maxLength={9}
                            onChange={(e) => {
                              // Mascara simples de CEP
                              let value = e.target.value.replace(/\D/g, "");
                              if (value.length > 8) value = value.slice(0, 8);
                              if (value.length > 5) {
                                value = value.replace(/^(\d{5})(\d)/, "$1-$2");
                              }
                              e.target.value = value;
                              form.setValue("addressZipcode", value, { shouldValidate: true });
                            }}
                          />
                          {isCepLoading && (
                            <div className="absolute right-2 top-2.5">
                              <Loader2 className="h-4 w-4 animate-spin text-muted-foreground" />
                            </div>
                          )}
                        </div>
                    </div>
                    <div className="space-y-2 col-span-2">
                        <Label htmlFor="street">Rua</Label>
                        <Input id="street" {...form.register("addressStreet")} />
                    </div>
                    <div className="space-y-2">
                        <Label htmlFor="number">Número</Label>
                        <Input id="number" {...form.register("addressNumber")} />
                    </div>
                    <div className="space-y-2">
                        <Label htmlFor="complement">Complemento</Label>
                        <Input id="complement" {...form.register("addressComplement")} />
                    </div>
                    <div className="space-y-2">
                        <Label htmlFor="neighborhood">Bairro</Label>
                        <Input id="neighborhood" {...form.register("addressNeighborhood")} />
                    </div>
                    <div className="space-y-2">
                        <Label htmlFor="city">Cidade</Label>
                        <Input id="city" {...form.register("addressCity")} />
                    </div>
                    <div className="space-y-2">
                        <Label htmlFor="state">Estado</Label>
                        <Input id="state" {...form.register("addressState")} />
                    </div>
                  </div>
                </CardContent>
              </Card>
            )}

            {activeTab === "social" && (
              <Card>
                <CardHeader>
                  <CardTitle>Redes Sociais</CardTitle>
                  <CardDescription>Conecte-se com seus clientes.</CardDescription>
                </CardHeader>
                <CardContent className="space-y-4">
                   <div className="space-y-2">
                    <Label htmlFor="whatsapp">WhatsApp (Número com DDD)</Label>
                    <Input id="whatsapp" {...form.register("whatsappNumber")} placeholder="11999999999" />
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="instagram">Instagram (URL)</Label>
                    <Input id="instagram" {...form.register("instagramUrl")} placeholder="https://instagram.com/..." />
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="facebook">Facebook (URL)</Label>
                    <Input id="facebook" {...form.register("facebookUrl")} placeholder="https://facebook.com/..." />
                  </div>
                </CardContent>
              </Card>
            )}

            {activeTab === "payments" && (
               <Card>
                <CardHeader>
                  <CardTitle>Métodos de Pagamento</CardTitle>
                  <CardDescription>Selecione os métodos aceitos na loja.</CardDescription>
                </CardHeader>
                <CardContent className="space-y-4">
                    {["Crédito", "Débito", "PIX", "Dinheiro"].map((method) => (
                        <div key={method} className="space-y-2">
                            <div className="flex items-center space-x-2">
                                <Switch 
                                    id={`pay-${method}`}
                                    checked={paymentMethods.includes(method)}
                                    onCheckedChange={(checked) => {
                                        if (checked) {
                                            setPaymentMethods([...paymentMethods, method]);
                                        } else {
                                            setPaymentMethods(paymentMethods.filter(m => m !== method));
                                        }
                                    }}
                                />
                                <Label htmlFor={`pay-${method}`}>{method}</Label>
                            </div>
                            {method === "PIX" && paymentMethods.includes("PIX") && (
                                <div className="ml-8 animate-in slide-in-from-top-2 duration-300">
                                    <p className="text-sm text-muted-foreground mt-1">
                                        Para utilizar o PIX, configure sua chave na aba <Button variant="link" className="p-0 h-auto font-bold" onClick={() => setActiveTab("pix-integration")}>Integração PIX</Button>.
                                    </p>
                                </div>
                            )}
                        </div>
                    ))}
                </CardContent>
              </Card>
            )}

            {activeTab === "hours" && (
               <Card>
                <CardHeader>
                  <CardTitle>Horário de Atendimento</CardTitle>
                  <CardDescription>Configure os horários de abertura e fechamento.</CardDescription>
                </CardHeader>
                <CardContent className="space-y-6">
                    {["monday", "tuesday", "wednesday", "thursday", "friday", "saturday", "sunday"].map((day) => {
                         const dayKey = day as keyof OpeningHours;
                         const dayConfig = openingHours[dayKey] || { open: "08:00", close: "18:00", isOpen: false };
                         
                         const translateDay = (d: string) => {
                             const map: Record<string, string> = {
                                 monday: "Segunda-feira", tuesday: "Terça-feira", wednesday: "Quarta-feira",
                                 thursday: "Quinta-feira", friday: "Sexta-feira", saturday: "Sábado", sunday: "Domingo"
                             };
                             return map[d] || d;
                         };

                         return (
                            <div key={day} className="flex items-center justify-between">
                                <div className="flex items-center space-x-4">
                                    <Switch 
                                        checked={dayConfig.isOpen}
                                        onCheckedChange={(checked) => {
                                            setOpeningHours({
                                                ...openingHours,
                                                [dayKey]: { ...dayConfig, isOpen: checked }
                                            });
                                        }}
                                        disabled={!canUpdate}
                                    />
                                    <Label className="w-24">{translateDay(day)}</Label>
                                </div>
                                {dayConfig.isOpen && (
                                    <div className="flex items-center space-x-2">
                                        <Input 
                                            type="time" 
                                            value={dayConfig.open} 
                                            onChange={(e) => setOpeningHours({
                                                ...openingHours,
                                                [dayKey]: { ...dayConfig, open: e.target.value }
                                            })}
                                            className="w-24"
                                            disabled={!canUpdate}
                                        />
                                        <span>às</span>
                                        <Input 
                                            type="time" 
                                            value={dayConfig.close} 
                                            onChange={(e) => setOpeningHours({
                                                ...openingHours,
                                                [dayKey]: { ...dayConfig, close: e.target.value }
                                            })}
                                            className="w-24"
                                            disabled={!canUpdate}
                                        />
                                    </div>
                                )}
                                {!dayConfig.isOpen && <span className="text-sm text-muted-foreground">Fechado</span>}
                            </div>
                         );
                    })}
                </CardContent>
              </Card>
            )}
            </fieldset>

            {canUpdate && (
            <div className="flex justify-end">
              <Button type="submit" disabled={mutation.isPending}>
                {mutation.isPending && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
                Salvar Alterações
              </Button>
            </div>
            )}
          </form>
          )}
        </div>
      </div>
    </div>
  );
}
