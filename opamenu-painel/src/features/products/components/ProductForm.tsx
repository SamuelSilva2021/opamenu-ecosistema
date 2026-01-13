import { useEffect, useState } from "react";
import { useForm, useFieldArray, type Resolver } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import * as z from "zod";
import { useQuery } from "@tanstack/react-query";
import { Loader2, Upload, X } from "lucide-react";

import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogFooter,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
  FormDescription,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { Switch } from "@/components/ui/switch";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Separator } from "@/components/ui/separator";
import { ScrollArea } from "@/components/ui/scroll-area";

import type { Product, CreateProductRequest, UpdateProductRequest } from "../types";
import { categoriesService } from "@/features/categories/categories.service";
import { addonsService } from "@/features/addons/addons.service";
import { filesService } from "@/services/files.service";
import { useToast } from "@/hooks/use-toast";

const productAddonGroupSchema = z.object({
  addonGroupId: z.coerce.number().min(1, "Selecione um grupo"),
  displayOrder: z.coerce.number().default(0),
  isRequired: z.boolean().default(false),
  minSelectionsOverride: z.coerce.number().optional().nullable(),
  maxSelectionsOverride: z.coerce.number().optional().nullable(),
});

const formSchema = z.object({
  name: z.string().min(2, "Nome deve ter pelo menos 2 caracteres"),
  description: z.string().optional(),
  price: z.coerce.number().min(0.01, "O preço deve ser maior que zero"),
  categoryId: z.coerce.number().min(1, "Selecione uma categoria"),
  imageUrl: z.string().optional(),
  isActive: z.boolean().default(true),
  displayOrder: z.coerce.number().default(0),
  addonGroups: z.array(productAddonGroupSchema).optional(),
});

type FormValues = z.infer<typeof formSchema>;

interface ProductFormProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onSubmit: (data: CreateProductRequest | UpdateProductRequest) => void;
  initialData?: Product | null;
  isLoading?: boolean;
}

export function ProductForm({
  open,
  onOpenChange,
  onSubmit,
  initialData,
  isLoading,
}: ProductFormProps) {
  const { toast } = useToast();
  const [isUploading, setIsUploading] = useState(false);

  const { data: categories = [] } = useQuery({
    queryKey: ["categories", "active"],
    queryFn: categoriesService.getActiveCategories,
    enabled: open,
  });

  const { data: addonGroups = [] } = useQuery({
    queryKey: ["addon-groups"],
    queryFn: addonsService.getGroups,
    enabled: open,
  });

  const form = useForm<FormValues>({
    resolver: zodResolver(formSchema) as Resolver<FormValues>,
    defaultValues: {
      name: "",
      description: "",
      price: 0,
      categoryId: 0,
      imageUrl: "",
      isActive: true,
      displayOrder: 0,
      addonGroups: [],
    },
  });

  const { fields } = useFieldArray({
    control: form.control,
    name: "addonGroups",
  });

  useEffect(() => {
    if (open) {
      if (initialData) {
        form.reset({
          name: initialData.name,
          description: initialData.description || "",
          price: initialData.price,
          categoryId: initialData.categoryId,
          imageUrl: initialData.imageUrl || "",
          isActive: initialData.isActive,
          displayOrder: initialData.displayOrder,
          addonGroups: initialData.addonGroups?.map((g) => ({
            addonGroupId: g.addonGroupId,
            displayOrder: g.displayOrder,
            isRequired: g.isRequired,
            minSelectionsOverride: g.minSelectionsOverride,
            maxSelectionsOverride: g.maxSelectionsOverride,
          })) || [],
        });
      } else {
        form.reset({
          name: "",
          description: "",
          price: 0,
          categoryId: 0,
          imageUrl: "",
          isActive: true,
          displayOrder: 0,
          addonGroups: [],
        });
      }
    }
  }, [initialData, form, open]);

  const handleImageUpload = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;

    try {
      setIsUploading(true);
      const result = await filesService.uploadFile(file, "products");
      if (result.isSuccess && result.fileUrl) {
        form.setValue("imageUrl", result.fileUrl);
        toast({ title: "Sucesso", description: "Imagem enviada com sucesso", variant: "success" });
      } else {
        toast({ title: "Erro", description: "Falha ao enviar imagem", variant: "destructive" });
      }
    } catch (error) {
      toast({ title: "Erro", description: "Erro ao enviar imagem", variant: "destructive" });
    } finally {
      setIsUploading(false);
    }
  };

  const handleSubmit = (values: FormValues) => {
    // Filter out groups with invalid ID (0)
    const validGroups = values.addonGroups?.filter(g => g.addonGroupId > 0);
    
    onSubmit({
      ...values,
      addonGroups: validGroups?.map(g => ({
        addonGroupId: g.addonGroupId,
        displayOrder: g.displayOrder,
        isRequired: g.isRequired,
        minSelectionsOverride: g.minSelectionsOverride ?? undefined,
        maxSelectionsOverride: g.maxSelectionsOverride ?? undefined,
      })),
    });
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-6xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle>
            {initialData ? "Editar Produto" : "Novo Produto"}
          </DialogTitle>
        </DialogHeader>

        <Form {...form}>
          <form onSubmit={form.handleSubmit(handleSubmit)} className="space-y-6">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              {/* Left Column: Basic Info */}
              <div className="space-y-4">
                <FormField
                  control={form.control}
                  name="name"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Nome do Produto</FormLabel>
                      <FormControl>
                        <Input placeholder="Ex: X-Bacon" {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <FormField
                  control={form.control}
                  name="description"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Descrição</FormLabel>
                      <FormControl>
                        <Textarea 
                          placeholder="Detalhes do produto..." 
                          className="resize-none h-24" 
                          {...field} 
                        />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <div className="grid grid-cols-2 gap-4">
                  <FormField
                    control={form.control}
                    name="price"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Preço (R$)</FormLabel>
                        <FormControl>
                          <Input 
                            type="number" 
                            step="0.01" 
                            min="0"
                            placeholder="0.00" 
                            {...field} 
                          />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />

                  <FormField
                    control={form.control}
                    name="displayOrder"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Ordem de Exibição</FormLabel>
                        <FormControl>
                          <Input 
                            type="number" 
                            min="0"
                            {...field} 
                          />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </div>

                <FormField
                  control={form.control}
                  name="categoryId"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Categoria</FormLabel>
                      <Select 
                        onValueChange={(val) => field.onChange(Number(val))} 
                        value={field.value?.toString()}
                      >
                        <FormControl>
                          <SelectTrigger>
                            <SelectValue placeholder="Selecione uma categoria" />
                          </SelectTrigger>
                        </FormControl>
                        <SelectContent>
                          {categories.map((category) => (
                            <SelectItem key={category.id} value={category.id.toString()}>
                              {category.name}
                            </SelectItem>
                          ))}
                        </SelectContent>
                      </Select>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <FormField
                  control={form.control}
                  name="isActive"
                  render={({ field }) => (
                    <FormItem className="flex flex-row items-center justify-between rounded-lg border p-4">
                      <div className="space-y-0.5">
                        <FormLabel className="text-base">Ativo</FormLabel>
                        <FormDescription>
                          Produto visível no cardápio
                        </FormDescription>
                      </div>
                      <FormControl>
                        <Switch
                          checked={field.value}
                          onCheckedChange={field.onChange}
                        />
                      </FormControl>
                    </FormItem>
                  )}
                />
              </div>

              {/* Right Column: Image & Addons */}
              <div className="space-y-6">
                {/* Image Upload */}
                <div className="space-y-4">
                  <FormLabel>Imagem do Produto</FormLabel>
                  <div className="flex flex-col items-center gap-4 p-4 border-2 border-dashed rounded-lg">
                    {form.watch("imageUrl") ? (
                      <div className="relative w-full aspect-video rounded-lg overflow-hidden bg-muted">
                        <img 
                          src={form.watch("imageUrl")} 
                          alt="Preview" 
                          className="w-full h-full object-cover"
                        />
                        <Button
                          type="button"
                          variant="destructive"
                          size="icon"
                          className="absolute top-2 right-2 h-8 w-8"
                          onClick={() => form.setValue("imageUrl", "")}
                        >
                          <X className="h-4 w-4" />
                        </Button>
                      </div>
                    ) : (
                      <div className="flex flex-col items-center justify-center py-8 text-muted-foreground">
                        <Upload className="h-12 w-12 mb-2" />
                        <span className="text-sm">Clique para fazer upload</span>
                      </div>
                    )}
                    <div className="flex items-center gap-2 w-full">
                      <Input 
                        type="file" 
                        accept="image/*"
                        className="hidden" 
                        id="image-upload"
                        onChange={handleImageUpload}
                        disabled={isUploading}
                      />
                      <Button
                        type="button"
                        variant="outline"
                        className="w-full"
                        disabled={isUploading}
                        onClick={() => document.getElementById("image-upload")?.click()}
                      >
                        {isUploading ? (
                          <>
                            <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                            Enviando...
                          </>
                        ) : (
                          "Selecionar Imagem"
                        )}
                      </Button>
                    </div>
                  </div>
                </div>

                <Separator />

                {/* Addon Groups */}
                <div className="space-y-4">
                  <div className="flex items-center justify-between">
                    <FormLabel className="text-base">Grupos de Adicionais</FormLabel>
                  </div>
                  
                  <ScrollArea className="h-[400px] pr-4">
                    <div className="space-y-4">
                      {fields.map((field) => {
                         const currentGroup = addonGroups.find(g => g.id === field.addonGroupId);
                         return (
                        <div key={field.id} className="bg-muted/50 p-4 rounded-lg space-y-4 border">
                          <div className="flex items-start justify-between gap-4">
                            <div className="flex-1">
                              <FormLabel className="text-xs font-bold text-muted-foreground uppercase tracking-wider">Grupo</FormLabel>
                              <div className="text-sm font-medium mt-1">
                                {currentGroup?.name || "Grupo não encontrado"}
                              </div>
                            </div>
                          </div>

                          <div className="grid grid-cols-2 gap-4">
                            <div className="flex flex-col gap-1">
                                <span className="text-xs font-medium text-muted-foreground">Obrigatório</span>
                                <span className="text-sm">{field.isRequired ? "Sim" : "Não"}</span>
                            </div>
                             <div className="flex flex-col gap-1">
                                <span className="text-xs font-medium text-muted-foreground">Ordem</span>
                                <span className="text-sm">{field.displayOrder}</span>
                            </div>
                          </div>

                          <div className="grid grid-cols-2 gap-4">
                            <div className="flex flex-col gap-1">
                                <span className="text-xs font-medium text-muted-foreground">Mín. Seleções</span>
                                <span className="text-sm">{field.minSelectionsOverride ?? "Padrão"}</span>
                            </div>
                            <div className="flex flex-col gap-1">
                                <span className="text-xs font-medium text-muted-foreground">Máx. Seleções</span>
                                <span className="text-sm">{field.maxSelectionsOverride ?? "Padrão"}</span>
                            </div>
                          </div>
                        </div>
                      )})}
                      {fields.length === 0 && (
                        <div className="text-center py-8 text-muted-foreground text-sm border-2 border-dashed rounded-lg">
                          Nenhum grupo vinculado
                        </div>
                      )}
                    </div>
                  </ScrollArea>
                </div>
              </div>
            </div>

            <DialogFooter>
              <Button type="button" variant="outline" onClick={() => onOpenChange(false)}>
                Cancelar
              </Button>
              <Button type="submit" disabled={isLoading || isUploading}>
                {isLoading && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
                {initialData ? "Salvar Alterações" : "Criar Produto"}
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}
