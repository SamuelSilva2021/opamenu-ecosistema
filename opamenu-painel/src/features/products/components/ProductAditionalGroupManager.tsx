import { useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { Plus, Edit, Trash2, Save, X } from "lucide-react";
import { productsService } from "../products.service";
import { aditionalsService } from "../../aditionals/aditionals.service";
import type {
    ProductAditionalGroupResponse,
    AddProductAditionalGroupRequest,
    UpdateProductAditionalGroupRequest,
} from "../types";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Switch } from "@/components/ui/switch";
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
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from "@/components/ui/select";
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
import { useToast } from "@/hooks/use-toast";
import { getErrorMessage } from "@/lib/utils";

interface ProductAditionalGroupManagerProps {
    productId: string | null;
    productName?: string;
    open: boolean;
    onOpenChange: (open: boolean) => void;
}

export function ProductAditionalGroupManager({
    productId,
    productName,
    open,
    onOpenChange,
}: ProductAditionalGroupManagerProps) {
    const [isFormOpen, setIsFormOpen] = useState(false);
    const [editingGroup, setEditingGroup] = useState<ProductAditionalGroupResponse | null>(null);
    const [deleteId, setDeleteId] = useState<string | null>(null);

    // Form State
    const [selectedGroupId, setSelectedGroupId] = useState<string>("");
    const [displayOrder, setDisplayOrder] = useState<number>(0);
    const [isRequired, setIsRequired] = useState<boolean>(false);
    const [minSelections, setMinSelections] = useState<string>("");
    const [maxSelections, setMaxSelections] = useState<string>("");

    const queryClient = useQueryClient();
    const { toast } = useToast();

    // Fetch Product Aditional Groups
    const { data: productAditionalGroups, isLoading: isLoadingGroups } = useQuery({
        queryKey: ["product-aditional-groups", productId],
        queryFn: () => productsService.getProductAditionalGroups(productId!),
        enabled: !!productId && open,
    });

    // Fetch All Aditional Groups (for selection)
    const { data: allAditionalGroups } = useQuery({
        queryKey: ["aditional-groups"],
        queryFn: () => aditionalsService.getGroups(),
        enabled: open && isFormOpen && !editingGroup, // Only fetch when adding new
    });

    const createMutation = useMutation({
        mutationFn: (data: AddProductAditionalGroupRequest) =>
            productsService.addProductAditionalGroup(productId!, data),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ["product-aditional-groups", productId] });
            toast({ variant: "success", title: "Sucesso", description: "Grupo adicionado ao produto" });
            resetForm();
        },
        onError: (error) => {
            toast({ title: "Erro", description: getErrorMessage(error), variant: "destructive" });
        },
    });

    const updateMutation = useMutation({
        mutationFn: ({
            aditionalGroupId,
            data,
        }: {
            aditionalGroupId: string;
            data: UpdateProductAditionalGroupRequest;
        }) => productsService.updateProductAditionalGroup(productId!, aditionalGroupId, data),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ["product-aditional-groups", productId] });
            toast({ variant: "success", title: "Sucesso", description: "Configuração atualizada" });
            resetForm();
        },
        onError: (error) => {
            toast({ title: "Erro", description: getErrorMessage(error), variant: "destructive" });
        },
    });

    const deleteMutation = useMutation({
        mutationFn: (aditionalGroupId: string) =>
            productsService.deleteProductAditionalGroup(productId!, aditionalGroupId),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ["product-aditional-groups", productId] });
            toast({ variant: "success", title: "Sucesso", description: "Grupo removido do produto" });
            setDeleteId(null);
        },
        onError: (error) => {
            toast({ title: "Erro", description: getErrorMessage(error), variant: "destructive" });
        },
    });

    const resetForm = () => {
        setIsFormOpen(false);
        setEditingGroup(null);
        setSelectedGroupId("");
        setDisplayOrder(0);
        setIsRequired(false);
        setMinSelections("");
        setMaxSelections("");
    };

    const handleOpenNew = () => {
        setEditingGroup(null);
        setSelectedGroupId("");
        setDisplayOrder((productAditionalGroups?.length || 0) + 1);
        setIsRequired(false);
        setMinSelections("");
        setMaxSelections("");
        setIsFormOpen(true);
    };

    const handleEdit = (group: ProductAditionalGroupResponse) => {
        setEditingGroup(group);
        setSelectedGroupId(group.aditionalGroupId.toString());
        setDisplayOrder(group.displayOrder);
        setIsRequired(group.isRequired);
        setMinSelections(group.minSelectionsOverride?.toString() || "");
        setMaxSelections(group.maxSelectionsOverride?.toString() || "");
        setIsFormOpen(true);
    };

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();

        const commonData = {
            displayOrder: Number(displayOrder),
            isRequired,
            minSelectionsOverride: minSelections ? Number(minSelections) : undefined,
            maxSelectionsOverride: maxSelections ? Number(maxSelections) : undefined,
        };

        if (editingGroup) {
            updateMutation.mutate({
                aditionalGroupId: editingGroup.aditionalGroupId,
                data: commonData,
            });
        } else {
            if (!selectedGroupId) {
                toast({ title: "Erro", description: "Selecione um grupo de adicionais", variant: "destructive" });
                return;
            }
            createMutation.mutate({
                aditionalGroupId: selectedGroupId,
                ...commonData,
            });
        }
    };

    return (
        <>
            <Sheet open={open} onOpenChange={onOpenChange}>
                <SheetContent className="sm:max-w-xl w-full overflow-y-auto">
                    <SheetHeader className="mb-6">
                        <SheetTitle>Gerenciar Grupos de Adicionais</SheetTitle>
                        <SheetDescription>
                            Produto: {productName || "Carregando..."}
                        </SheetDescription>
                    </SheetHeader>

                    {isFormOpen ? (
                        <div className="space-y-4 border p-4 rounded-md bg-muted/20 mx-1">
                            <div className="flex justify-between items-center mb-2">
                                <h3 className="font-medium">
                                    {editingGroup ? "Editar Configuração" : "Adicionar Grupo"}
                                </h3>
                                <Button variant="ghost" size="sm" onClick={resetForm}>
                                    <X className="h-4 w-4" />
                                </Button>
                            </div>

                            <form onSubmit={handleSubmit} className="space-y-4">
                                {!editingGroup && (
                                    <div className="space-y-2">
                                        <Label htmlFor="group">Grupo de Adicionais</Label>
                                        <Select
                                            value={selectedGroupId}
                                            onValueChange={setSelectedGroupId}
                                        >
                                            <SelectTrigger>
                                                <SelectValue placeholder="Selecione um grupo" />
                                            </SelectTrigger>
                                            <SelectContent>
                                                {allAditionalGroups?.map((group) => (
                                                    <SelectItem
                                                        key={group.id}
                                                        value={group.id.toString()}
                                                        disabled={productAditionalGroups?.some(
                                                            (pag) => pag.aditionalGroupId === group.id
                                                        )}
                                                    >
                                                        {group.name}
                                                    </SelectItem>
                                                ))}
                                            </SelectContent>
                                        </Select>
                                    </div>
                                )}

                                <div className="grid grid-cols-2 gap-4">
                                    <div className="space-y-2">
                                        <Label htmlFor="order">Ordem de Exibição</Label>
                                        <Input
                                            id="order"
                                            type="number"
                                            value={displayOrder}
                                            onChange={(e) => setDisplayOrder(Number(e.target.value))}
                                            required
                                        />
                                    </div>

                                    <div className="flex items-center space-x-2 pt-8">
                                        <Switch
                                            id="required"
                                            checked={isRequired}
                                            onCheckedChange={setIsRequired}
                                        />
                                        <Label htmlFor="required">Obrigatório</Label>
                                    </div>
                                </div>

                                <div className="grid grid-cols-2 gap-4">
                                    <div className="space-y-2">
                                        <Label htmlFor="min">Mínimo (Override)</Label>
                                        <Input
                                            id="min"
                                            type="number"
                                            placeholder="Padrão do grupo"
                                            value={minSelections}
                                            onChange={(e) => setMinSelections(e.target.value)}
                                        />
                                    </div>
                                    <div className="space-y-2">
                                        <Label htmlFor="max">Máximo (Override)</Label>
                                        <Input
                                            id="max"
                                            type="number"
                                            placeholder="Padrão do grupo"
                                            value={maxSelections}
                                            onChange={(e) => setMaxSelections(e.target.value)}
                                        />
                                    </div>
                                </div>

                                <div className="flex justify-end pt-2">
                                    <Button type="submit" disabled={createMutation.isPending || updateMutation.isPending}>
                                        <Save className="mr-2 h-4 w-4" />
                                        Salvar
                                    </Button>
                                </div>
                            </form>
                        </div>
                    ) : (
                        <div className="space-y-6 px-1">
                            <SheetHeader className="mb-8">
                                <h3 className="text-sm font-medium">Grupos Associados ({productAditionalGroups?.length || 0})</h3>
                                <Button size="sm" onClick={handleOpenNew}>
                                    <Plus className="mr-2 h-4 w-4" /> Adicionar Grupo
                                </Button>
                            </SheetHeader>

                            {isLoadingGroups ? (
                                <div className="flex justify-center py-8">Carregando...</div>
                            ) : (
                                <div className="rounded-md border">
                                    <Table>
                                        <TableHeader>
                                            <TableRow>
                                                <TableHead>Grupo</TableHead>
                                                <TableHead className="text-center">Ordem</TableHead>
                                                <TableHead className="text-center">Obrig.</TableHead>
                                                <TableHead className="text-center">Min/Max</TableHead>
                                                <TableHead className="text-right">Ações</TableHead>
                                            </TableRow>
                                        </TableHeader>
                                        <TableBody>
                                            {productAditionalGroups && productAditionalGroups.length > 0 ? (
                                                productAditionalGroups
                                                    .sort((a, b) => a.displayOrder - b.displayOrder)
                                                    .map((pag) => (
                                                        <TableRow key={pag.id}>
                                                            <TableCell className="font-medium">
                                                                {pag.aditionalGroup?.name}
                                                            </TableCell>
                                                            <TableCell className="text-center">
                                                                {pag.displayOrder}
                                                            </TableCell>
                                                            <TableCell className="text-center">
                                                                {pag.isRequired ? "Sim" : "Não"}
                                                            </TableCell>
                                                            <TableCell className="text-center text-xs">
                                                                {pag.minSelectionsOverride ?? "-"} /{" "}
                                                                {pag.maxSelectionsOverride ?? "-"}
                                                            </TableCell>
                                                            <TableCell className="text-right">
                                                                <div className="flex justify-end space-x-2">
                                                                    <Button
                                                                        variant="ghost"
                                                                        size="sm"
                                                                        className="h-8 w-8 p-0"
                                                                        onClick={() => handleEdit(pag)}
                                                                    >
                                                                        <Edit className="h-4 w-4" />
                                                                    </Button>
                                                                    <Button
                                                                        variant="ghost"
                                                                        size="sm"
                                                                        className="h-8 w-8 p-0 text-red-600 hover:text-red-600"
                                                                        onClick={() => setDeleteId(pag.aditionalGroupId)}
                                                                    >
                                                                        <Trash2 className="h-4 w-4" />
                                                                    </Button>
                                                                </div>
                                                            </TableCell>
                                                        </TableRow>
                                                    ))
                                            ) : (
                                                <TableRow>
                                                    <TableCell
                                                        colSpan={5}
                                                        className="h-24 text-center text-muted-foreground"
                                                    >
                                                        Nenhum grupo de adicionais associado.
                                                    </TableCell>
                                                </TableRow>
                                            )}
                                        </TableBody>
                                    </Table>
                                </div>
                            )}
                        </div>
                    )}
                </SheetContent>
            </Sheet>

            <AlertDialog open={!!deleteId} onOpenChange={(open) => !open && setDeleteId(null)}>
                <AlertDialogContent>
                    <AlertDialogHeader>
                        <AlertDialogTitle>Você tem certeza?</AlertDialogTitle>
                        <AlertDialogDescription>
                            Isso removerá este grupo de adicionais do produto. Os adicionais deste grupo não estarão mais disponíveis para este produto.
                        </AlertDialogDescription>
                    </AlertDialogHeader>
                    <AlertDialogFooter>
                        <AlertDialogCancel>Cancelar</AlertDialogCancel>
                        <AlertDialogAction
                            className="bg-red-600 hover:bg-red-700"
                            onClick={() => deleteId && deleteMutation.mutate(deleteId)}
                        >
                            Remover
                        </AlertDialogAction>
                    </AlertDialogFooter>
                </AlertDialogContent>
            </AlertDialog>
        </>
    );
}
