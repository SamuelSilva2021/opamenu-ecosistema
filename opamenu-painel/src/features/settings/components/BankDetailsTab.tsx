import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { Plus, Pencil, Trash2, Check } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Label } from "@/components/ui/label";
import { Switch } from "@/components/ui/switch";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { useToast } from "@/hooks/use-toast";
import { bankDetailsService } from "../bank-details.service";
import type { BankDetailsDto } from "../types";

const formSchema = z.object({
  bankName: z.string().max(100).optional(),
  agency: z.string().max(20).optional(),
  accountNumber: z.string().max(30).optional(),
  accountType: z.string().optional(),
  pixKey: z.string().max(100).optional(),
  isPixKeySelected: z.boolean(),
});

type FormValues = z.infer<typeof formSchema>;

export function BankDetailsTab() {
  const { toast } = useToast();
  const queryClient = useQueryClient();
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);

  const { data: bankDetails, isLoading } = useQuery({
    queryKey: ["bank-details"],
    queryFn: bankDetailsService.getAll,
  });

  const form = useForm<FormValues>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      bankName: undefined,
      agency: undefined,
      accountNumber: undefined,
      accountType: undefined,
      pixKey: undefined,
      isPixKeySelected: false,
    },
  });

  const createMutation = useMutation({
    mutationFn: bankDetailsService.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["bank-details"] });
      toast({ title: "Sucesso", description: "Dados bancários adicionados." });
      setIsDialogOpen(false);
      form.reset();
    },
    onError: () => toast({ title: "Erro", description: "Falha ao adicionar dados bancários.", variant: "destructive" }),
  });

  const updateMutation = useMutation({
    mutationFn: bankDetailsService.update,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["bank-details"] });
      toast({ title: "Sucesso", description: "Dados bancários atualizados." });
      setIsDialogOpen(false);
      setEditingId(null);
      form.reset();
    },
    onError: () => toast({ title: "Erro", description: "Falha ao atualizar dados bancários.", variant: "destructive" }),
  });

  const deleteMutation = useMutation({
    mutationFn: bankDetailsService.delete,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["bank-details"] });
      toast({ title: "Sucesso", description: "Dados bancários removidos." });
    },
    onError: () => toast({ title: "Erro", description: "Falha ao remover dados bancários.", variant: "destructive" }),
  });

  const onSubmit = (data: FormValues) => {
    const payload = {
      ...data,
      accountType: data.accountType ? Number(data.accountType) : undefined
    };
    
    if (editingId) {
      updateMutation.mutate({ ...payload, id: editingId });
    } else {
      createMutation.mutate(payload);
    }
  };

  const handleEdit = (item: BankDetailsDto) => {
    setEditingId(item.id);
    form.reset({
      bankName: item.bankName,
      agency: item.agency,
      accountNumber: item.accountNumber,
      accountType: item.accountType?.toString(),
      pixKey: item.pixKey,
      isPixKeySelected: item.isPixKeySelected,
    });
    setIsDialogOpen(true);
  };

  const handleAddNew = () => {
    setEditingId(null);
    form.reset({
      bankName: undefined,
      agency: undefined,
      accountNumber: undefined,
      accountType: undefined,
      pixKey: undefined,
      isPixKeySelected: false,
    });
    setIsDialogOpen(true);
  };

  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between">
        <div>
          <CardTitle>Dados Bancários</CardTitle>
          <CardDescription>Gerencie suas contas bancárias e chaves PIX.</CardDescription>
        </div>
        <Button onClick={handleAddNew}>
          <Plus className="mr-2 h-4 w-4" /> Adicionar
        </Button>
      </CardHeader>
      <CardContent>
        {isLoading ? (
          <div>Carregando...</div>
        ) : (
          <div className="rounded-md border">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Banco</TableHead>
                  <TableHead>Agência</TableHead>
                  <TableHead>Conta</TableHead>
                  <TableHead>Chave PIX</TableHead>
                  <TableHead>PIX Selecionado</TableHead>
                  <TableHead className="text-right">Ações</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {bankDetails?.length === 0 ? (
                    <TableRow>
                        <TableCell colSpan={6} className="text-center py-4">
                            Nenhum dado bancário cadastrado.
                        </TableCell>
                    </TableRow>
                ) : (
                    bankDetails?.map((item) => (
                    <TableRow key={item.id}>
                        <TableCell className="font-medium">{item.bankName || "-"}</TableCell>
                        <TableCell>{item.agency || "-"}</TableCell>
                        <TableCell>{item.accountNumber || "-"}</TableCell>
                        <TableCell>{item.pixKey || "-"}</TableCell>
                        <TableCell>
                        {item.isPixKeySelected ? (
                            <div className="flex items-center text-green-600">
                                <Check className="mr-1 h-4 w-4" /> Sim
                            </div>
                        ) : (
                            <span className="text-muted-foreground">Não</span>
                        )}
                        </TableCell>
                        <TableCell className="text-right">
                        <Button variant="ghost" size="icon" onClick={() => handleEdit(item)}>
                            <Pencil className="h-4 w-4" />
                        </Button>
                        <Button variant="ghost" size="icon" className="text-destructive" onClick={() => deleteMutation.mutate(item.id)}>
                            <Trash2 className="h-4 w-4" />
                        </Button>
                        </TableCell>
                    </TableRow>
                    ))
                )}
              </TableBody>
            </Table>
          </div>
        )}

        <Dialog open={isDialogOpen} onOpenChange={setIsDialogOpen}>
          <DialogContent>
            <DialogHeader>
              <DialogTitle>{editingId ? "Editar" : "Adicionar"} Dados Bancários</DialogTitle>
              <DialogDescription>
                Preencha as informações abaixo.
              </DialogDescription>
            </DialogHeader>
            <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="bankName">Nome do Banco</Label>
                <Input id="bankName" {...form.register("bankName")} placeholder="Ex: Nubank" />
              </div>
              <div className="grid grid-cols-3 gap-4">
                <div className="space-y-2">
                    <Label htmlFor="agency">Agência</Label>
                    <Input id="agency" {...form.register("agency")} placeholder="0001" />
                </div>
                <div className="space-y-2">
                    <Label htmlFor="accountNumber">Conta</Label>
                    <Input id="accountNumber" {...form.register("accountNumber")} placeholder="12345-6" />
                </div>
                <div className="space-y-2">
                    <Label htmlFor="accountType">Tipo</Label>
                    <Input id="accountType" {...form.register("accountType")} placeholder="Ex: 1" type="number" />
                </div>
              </div>
              <div className="space-y-2">
                <Label htmlFor="pixKey">Chave PIX</Label>
                <Input id="pixKey" {...form.register("pixKey")} placeholder="CPF, Email, Aleatória..." />
              </div>
              <div className="flex items-center space-x-2">
                <Switch
                    id="isPixKeySelected"
                    checked={form.watch("isPixKeySelected")}
                    onCheckedChange={(checked) => form.setValue("isPixKeySelected", checked)}
                />
                <Label htmlFor="isPixKeySelected">Usar como chave PIX principal da loja</Label>
              </div>
              <DialogFooter>
                <Button type="button" variant="outline" onClick={() => setIsDialogOpen(false)}>Cancelar</Button>
                <Button type="submit">{editingId ? "Salvar" : "Adicionar"}</Button>
              </DialogFooter>
            </form>
          </DialogContent>
        </Dialog>
      </CardContent>
    </Card>
  );
}
