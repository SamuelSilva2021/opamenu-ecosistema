import { useEffect, useState } from "react";
import { useForm, useFieldArray } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import * as z from "zod";
import { useQuery } from "@tanstack/react-query";
import { Loader2, Upload, X } from "lucide-react";
import imageCompression from 'browser-image-compression';

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
import { CurrencyInput } from "@/components/ui/currency-input";

import type { Product, CreateProductRequest, UpdateProductRequest } from "../types";
import { productsService } from "../products.service";
import { categoriesService } from "@/features/categories/categories.service";
import { aditionalsService } from "@/features/aditionals/aditionals.service";
import { filesService } from "@/services/files.service";
import { useToast } from "@/hooks/use-toast";

const productAditionalGroupSchema = z.object({
  aditionalGroupId: z.string().min(1, "Selecione um grupo"),
  displayOrder: z.coerce.number().default(0),
  isRequired: z.boolean().default(false),
  minSelectionsOverride: z.coerce.number().optional(),
  maxSelectionsOverride: z.coerce.number().optional(),
});

const formSchema = z.object({
  name: z.string().min(2, "Nome deve ter pelo menos 2 caracteres"),
  description: z.string().optional(),
  price: z.coerce.number().min(0.01, "O preço deve ser maior que zero"),
  categoryId: z.string().min(1, "Selecione uma categoria"),
  imageUrl: z.string().optional(),
  isActive: z.boolean().default(true),
  displayOrder: z.coerce.number().default(0),
  aditionalGroups: z.array(productAditionalGroupSchema).optional(),
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

  const { data: aditionalGroups = [] } = useQuery({
    queryKey: ["aditional-groups"],
    queryFn: aditionalsService.getGroups,
    enabled: open,
  });

  // Fetch product aditional groups separately to ensure we have the details
  const { data: productAditionalGroupsData } = useQuery({
    queryKey: ["product-aditional-groups", initialData?.id],
    queryFn: () => productsService.getProductAditionalGroups(initialData!.id),
    enabled: !!initialData?.id && open,
  });

  const form = useForm<FormValues>({
    resolver: zodResolver(formSchema) as any,
    defaultValues: {
      name: "",
      description: "",
      price: 0,
      categoryId: "",
      imageUrl: "",
      isActive: true,
      displayOrder: 0,
      aditionalGroups: [],
    },
  });

  const { fields } = useFieldArray({
    control: form.control,
    name: "aditionalGroups",
  });

  useEffect(() => {
    if (open && initialData) {
      const groupsToUse = productAditionalGroupsData || initialData.aditionalGroups || [];

      form.reset({
        name: initialData.name,
        description: initialData.description || "",
        price: initialData.price,
        categoryId: initialData.categoryId,
        imageUrl: initialData.imageUrl || "",
        isActive: initialData.isActive,
        displayOrder: initialData.displayOrder,
        aditionalGroups: groupsToUse.map((g) => ({
          aditionalGroupId: g.aditionalGroupId,
          displayOrder: g.displayOrder,
          isRequired: g.isRequired,
          minSelectionsOverride: g.minSelectionsOverride,
          maxSelectionsOverride: g.maxSelectionsOverride,
        })),
      });
    } else if (open && !initialData) {
      form.reset({
        name: "",
        description: "",
        price: 0,
        categoryId: "",
        imageUrl: "",
        isActive: true,
        displayOrder: 0,
        aditionalGroups: [],
      });
    }
  }, [initialData, productAditionalGroupsData, form, open]);

  const handleImageUpload = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;

    try {
      setIsUploading(true);

      let fileToUpload = file;

      // Otimizar imagem antes do envio
      if (file.type.startsWith('image/')) {
        try {
          console.log(`Original file size: ${(file.size / 1024 / 1024).toFixed(2)} MB`);

          const options = {
            maxSizeMB: 1, // Reduzido para 1MB para garantir margem de segurança
            maxWidthOrHeight: 1920,
            useWebWorker: false,
            initialQuality: 0.7,
          };

          const compressedBlob = await imageCompression(file, options);
          console.log(`Compressed file size: ${(compressedBlob.size / 1024 / 1024).toFixed(2)} MB`);

          fileToUpload = new File([compressedBlob], file.name, {
            type: file.type,
            lastModified: new Date().getTime()
          });

        } catch (compressionError) {
          console.error("Erro na compressão de imagem:", compressionError);
        }
      }

      const MAX_SIZE_BYTES = 5 * 1024 * 1024; // 5MB exatos
      if (fileToUpload.size > MAX_SIZE_BYTES) {
        toast({
          title: "Arquivo muito grande",
          description: `Mesmo após otimização, a imagem tem ${(fileToUpload.size / 1024 / 1024).toFixed(2)}MB. O limite é 5MB. Por favor, escolha outra imagem.`,
          variant: "destructive"
        });
        setIsUploading(false);
        e.target.value = "";
        return;
      }

      const result = await filesService.uploadFile(fileToUpload, "products");
      if (result.isSuccess && result.fileUrl) {
        form.setValue("imageUrl", result.fileUrl);
        toast({ title: "Sucesso", description: "Imagem enviada com sucesso", variant: "success" });
      } else {
        toast({ title: "Erro", description: "Falha ao enviar imagem", variant: "destructive" });
      }
    } catch (error) {
      console.error(error);
      toast({ title: "Erro", description: "Erro ao enviar imagem", variant: "destructive" });
    } finally {
      setIsUploading(false);
      e.target.value = "";
    }
  };

  const handleSubmit = (values: FormValues) => {
    const validGroups = values.aditionalGroups?.filter(g => g.aditionalGroupId !== "");

    onSubmit({
      ...values,
      aditionalGroups: validGroups?.map(g => ({
        aditionalGroupId: g.aditionalGroupId,
        displayOrder: g.displayOrder,
        isRequired: g.isRequired,
        minSelectionsOverride: g.minSelectionsOverride ?? undefined,
        maxSelectionsOverride: g.maxSelectionsOverride ?? undefined,
      })),
    });
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-6xl h-[90vh] flex flex-col p-0 gap-0">
        <DialogHeader className="px-6 py-4 border-b">
          <DialogTitle>
            {initialData ? "Editar Produto" : "Novo Produto"}
          </DialogTitle>
        </DialogHeader>

        <Form {...form}>
          <form onSubmit={form.handleSubmit(handleSubmit)} className="flex flex-col flex-1 min-h-0">
            <div className="flex-1 overflow-y-auto px-6 py-6 space-y-6">
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
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
                          <FormLabel>Preço</FormLabel>
                          <FormControl>
                            <CurrencyInput
                              placeholder="R$ 0,00"
                              value={field.value}
                              onChange={field.onChange}
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

                  <div className="grid grid-cols-[1fr_auto] gap-4 items-end">
                    <FormField
                      control={form.control}
                      name="categoryId"
                      render={({ field }) => (
                        <FormItem>
                          <FormLabel>Categoria</FormLabel>
                          <Select
                            onValueChange={field.onChange}
                            value={field.value}
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
                        <FormItem className="flex flex-col gap-2 rounded-lg border p-3 h-full justify-center">
                          <div className="flex items-center gap-2">
                            <FormLabel className="text-base cursor-pointer" htmlFor="is-active-switch">Ativo</FormLabel>
                            <FormControl>
                              <Switch
                                id="is-active-switch"
                                checked={field.value}
                                onCheckedChange={field.onChange}
                              />
                            </FormControl>
                          </div>
                        </FormItem>
                      )}
                    />
                  </div>
                </div>

                <div className="space-y-6">
                  <div className="space-y-4">
                    <FormLabel>Imagem do Produto</FormLabel>
                    <div className="flex flex-col items-center gap-4 p-4 border-2 border-dashed rounded-lg h-[340px] justify-center">
                      {form.watch("imageUrl") ? (
                        <div className="relative w-full h-full rounded-lg overflow-hidden bg-muted flex items-center justify-center">
                          <img
                            src={form.watch("imageUrl")}
                            alt="Preview"
                            className="w-full h-full object-contain"
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
                        <div className="flex flex-col items-center justify-center flex-1 text-muted-foreground">
                          <Upload className="h-12 w-12 mb-2" />
                          <span className="text-sm">Clique para fazer upload</span>
                        </div>
                      )}
                      <div className="flex items-center gap-2 w-full mt-auto">
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
                </div>
              </div>

              <Separator className="my-6" />

              {/* Aditional Groups */}
              <div className="space-y-4">
                <div className="flex items-center justify-between">
                  <FormLabel className="text-base">Grupos de Adicionais</FormLabel>
                </div>

                <ScrollArea className="h-[300px] pr-4">
                  <div className="space-y-4">
                    {fields.map((field) => {
                      const currentGroup = aditionalGroups.find(g => g.id === field.aditionalGroupId);
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

                          <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
                            <div className="flex flex-col gap-1">
                              <span className="text-xs font-medium text-muted-foreground">Obrigatório</span>
                              <span className="text-sm">{field.isRequired ? "Sim" : "Não"}</span>
                            </div>
                            <div className="flex flex-col gap-1">
                              <span className="text-xs font-medium text-muted-foreground">Ordem</span>
                              <span className="text-sm">{field.displayOrder}</span>
                            </div>
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
                      )
                    })}
                    {fields.length === 0 && (
                      <div className="text-center py-8 text-muted-foreground text-sm border-2 border-dashed rounded-lg">
                        Nenhum grupo vinculado
                      </div>
                    )}
                  </div>
                </ScrollArea>
              </div>

            </div>
            <DialogFooter className="px-6 py-4 border-t mt-auto bg-background">
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
