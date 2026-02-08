import { useEffect } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import * as z from "zod";
import { useMutation, useQueryClient, useQuery } from "@tanstack/react-query";
import { Shield } from "lucide-react";
import { employeesService } from "../employees.service";
import type { Role, Module } from "../types";
import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogFooter,
    DialogHeader,
    DialogTitle,
} from "@/components/ui/dialog";
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
import { Button } from "@/components/ui/button";
import { Textarea } from "@/components/ui/textarea";
import { Switch } from "@/components/ui/switch";
import { useToast } from "@/hooks/use-toast";
import { Badge } from "@/components/ui/badge";
import { cn } from "@/lib/utils";
import { Checkbox } from "@/components/ui/checkbox";

const roleSchema = z.object({
    name: z.string().min(2, "O nome deve ter pelo menos 2 caracteres"),
    description: z.string().optional(),
    isActive: z.boolean(),
    permissions: z.array(z.object({
        module: z.string(),
        actions: z.array(z.string())
    }))
});

type RoleFormValues = z.infer<typeof roleSchema>;

interface RoleFormProps {
    open: boolean;
    onOpenChange: (open: boolean) => void;
    role: Role | null;
    readOnly?: boolean;
}

const RoleForm = ({ open, onOpenChange, role, readOnly = false }: RoleFormProps) => {
    const { toast } = useToast();
    const queryClient = useQueryClient();

    const { data: modulesResponse, isLoading: isLoadingModules } = useQuery({
        queryKey: ["available-modules"],
        queryFn: () => employeesService.getAvailableModules(),
    });

    const modulesData = Array.isArray(modulesResponse)
        ? modulesResponse
        : Array.isArray((modulesResponse as any)?.data)
            ? (modulesResponse as any).data
            : (modulesResponse as any)?.items || [];


    const form = useForm<RoleFormValues>({
        resolver: zodResolver(roleSchema),
        defaultValues: {
            name: "",
            description: "",
            isActive: true,
            permissions: [],
        },
    });

    useEffect(() => {
        if (role) {
            form.reset({
                name: role.name,
                description: role.description || "",
                isActive: role.isActive,
                permissions: role.permissions || [],
            });
        } else {
            form.reset({
                name: "",
                description: "",
                isActive: true,
                permissions: [],
            });
        }
    }, [role, form]);

    const mutation = useMutation({
        mutationFn: (data: RoleFormValues) => {
            if (role) {
                return employeesService.updateRole(role.id, data);
            }
            return employeesService.createRole(data);
        },
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ["roles"] });
            toast({
                title: "Sucesso",
                description: `Perfil ${role ? "atualizado" : "criado"} com sucesso.`,
            });
            onOpenChange(false);
        },
        onError: (error: any) => {
            toast({
                title: "Erro",
                description: error?.response?.data?.message || "Erro ao salvar perfil.",
                variant: "destructive",
            });
        },
    });

    const onSubmit = (data: RoleFormValues) => {
        if (readOnly) return;
        mutation.mutate(data);
    };

    const togglePermission = (moduleKey: string, action: string) => {
        if (readOnly) return;
        const currentPermissions = form.getValues("permissions") || [];
        const modulePermission = currentPermissions.find(p => p.module === moduleKey);

        if (modulePermission) {
            const hasAction = modulePermission.actions.includes(action);
            const newActions = hasAction
                ? modulePermission.actions.filter(a => a !== action)
                : [...modulePermission.actions, action];

            if (newActions.length === 0) {
                form.setValue("permissions", currentPermissions.filter(p => p.module !== moduleKey));
            } else {
                form.setValue("permissions", currentPermissions.map(p =>
                    p.module === moduleKey ? { ...p, actions: newActions } : p
                ), { shouldDirty: true });
            }
        } else {
            form.setValue("permissions", [...currentPermissions, { module: moduleKey, actions: [action] }], { shouldDirty: true });
        }
    };

    const isActionSelected = (moduleKey: string, action: string) => {
        const permissions = form.watch("permissions") || [];
        return permissions.find(p => p.module === moduleKey)?.actions.includes(action);
    };

    const toggleAllActions = (moduleKey: string, availableActions: string[]) => {
        if (readOnly) return;
        const permissions = form.getValues("permissions") || [];
        const modulePermission = permissions.find(p => p.module === moduleKey);

        if (modulePermission?.actions.length === availableActions.length) {
            form.setValue("permissions", permissions.filter(p => p.module !== moduleKey), { shouldDirty: true });
        } else {
            if (modulePermission) {
                form.setValue("permissions", permissions.map(p =>
                    p.module === moduleKey ? { ...p, actions: [...availableActions] } : p
                ), { shouldDirty: true });
            } else {
                form.setValue("permissions", [...permissions, { module: moduleKey, actions: [...availableActions] }], { shouldDirty: true });
            }
        }
    };

    return (
        <Dialog open={open} onOpenChange={onOpenChange}>
            <DialogContent className="max-w-6xl h-[90vh] p-0 bg-background border flex flex-col overflow-hidden">
                <DialogHeader className="p-6 pb-2 shrink-0">
                    <DialogTitle className="flex items-center gap-2">
                        <Shield className="h-5 w-5 text-primary" />
                        {readOnly ? "Visualizar Perfil" : role ? "Editar Perfil" : "Novo Perfil"}
                    </DialogTitle>
                    <DialogDescription>
                        {readOnly
                            ? "Visualize os detalhes e permissões deste perfil de acesso."
                            : "Defina o nome e as permissões de acesso para este perfil."
                        }
                    </DialogDescription>
                </DialogHeader>

                <Form {...form}>
                    <form onSubmit={form.handleSubmit(onSubmit)} className="flex-1 flex flex-col overflow-hidden min-h-0">
                        <div className="flex-1 overflow-y-auto px-6 min-h-0">
                            <div className="space-y-6 py-4">
                                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                                    <FormField
                                        control={form.control}
                                        name="name"
                                        render={({ field }) => (
                                            <FormItem>
                                                <FormLabel>Nome do Perfil</FormLabel>
                                                <FormControl>
                                                    <Input
                                                        placeholder="Ex: Gerente, Atendente..."
                                                        disabled={readOnly}
                                                        {...field}
                                                    />
                                                </FormControl>
                                                <FormMessage />
                                            </FormItem>
                                        )}
                                    />
                                    <FormField
                                        control={form.control}
                                        name="isActive"
                                        render={({ field }) => (
                                            <FormItem className="flex flex-row items-center justify-between rounded-lg border p-3 shadow-sm mt-8">
                                                <div className="space-y-0.5">
                                                    <FormLabel>Status</FormLabel>
                                                    <FormDescription>
                                                        Perfis inativos não podem ser usados.
                                                    </FormDescription>
                                                </div>
                                                <FormControl>
                                                    <Switch
                                                        checked={field.value}
                                                        onCheckedChange={field.onChange}
                                                        disabled={readOnly}
                                                    />
                                                </FormControl>
                                            </FormItem>
                                        )}
                                    />
                                </div>

                                <FormField
                                    control={form.control}
                                    name="description"
                                    render={({ field }) => (
                                        <FormItem>
                                            <FormLabel>Descrição</FormLabel>
                                            <FormControl>
                                                <Textarea
                                                    placeholder="Descreva as responsabilidades deste perfil..."
                                                    className="resize-none h-24"
                                                    disabled={readOnly}
                                                    {...field}
                                                />
                                            </FormControl>
                                            <FormMessage />
                                        </FormItem>
                                    )}
                                />

                                <div className="space-y-4">
                                    <div className="flex items-center justify-between">
                                        <h3 className="text-lg font-semibold flex items-center gap-2">
                                            <Shield className="h-5 w-5 text-primary" /> Permissões por Módulo
                                        </h3>
                                        {!readOnly && (
                                            <Badge variant="outline" className="text-[10px] font-normal">
                                                Selecione as ações permitidas
                                            </Badge>
                                        )}
                                    </div>

                                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                                        {isLoadingModules ? (
                                            <>
                                                {[1, 2, 3, 4].map((i) => (
                                                    <div key={i} className="rounded-xl border p-4 bg-zinc-50/50 dark:bg-zinc-900/50 animate-pulse h-[140px]" />
                                                ))}
                                            </>
                                        ) : modulesData && modulesData.length > 0 ? (
                                            modulesData.map((module: Module) => {

                                                const count = form.watch("permissions")?.find(p => p.module === module.key)?.actions.length || 0;
                                                const isAllSelected = count === module.availableActions.length;

                                                return (
                                                    <div key={module.key} className={cn(
                                                        "rounded-xl border p-4 transition-all duration-200",
                                                        count > 0 ? "border-primary/20 bg-primary/5 shadow-sm" : "bg-zinc-50/50 dark:bg-zinc-900/50"
                                                    )}>
                                                        <div className="flex items-center justify-between mb-3">
                                                            <div className="space-y-1">
                                                                <div className="flex items-center gap-2">
                                                                    <h4 className="font-semibold text-sm leading-none">{module.name}</h4>
                                                                    {count > 0 && (
                                                                        <Badge className="h-4 px-1 text-[9px] font-bold">
                                                                            {count}
                                                                        </Badge>
                                                                    )}
                                                                </div>
                                                                <p className="text-[11px] text-muted-foreground leading-tight line-clamp-1">{module.description}</p>
                                                            </div>
                                                            {!readOnly && (
                                                                <Button
                                                                    type="button"
                                                                    variant="ghost"
                                                                    size="sm"
                                                                    className={cn(
                                                                        "text-[10px] h-7 px-2",
                                                                        isAllSelected ? "text-primary" : "text-muted-foreground"
                                                                    )}
                                                                    onClick={() => toggleAllActions(module.key, module.availableActions)}
                                                                >
                                                                    {isAllSelected ? "Desmarcar" : "Marcar Todos"}
                                                                </Button>
                                                            )}
                                                        </div>
                                                        <div className="flex flex-wrap gap-2">
                                                            {module.availableActions.map((action) => (
                                                                <div key={action} className={cn(
                                                                    "flex items-center space-x-2 rounded-md border px-2 py-1.5 transition-colors",
                                                                    isActionSelected(module.key, action)
                                                                        ? "border-primary/30 bg-primary/10"
                                                                        : "bg-background border-zinc-200"
                                                                )}>
                                                                    <Checkbox
                                                                        id={`${module.key}-${action}`}
                                                                        checked={isActionSelected(module.key, action)}
                                                                        onCheckedChange={() => togglePermission(module.key, action)}
                                                                        disabled={readOnly}
                                                                    />
                                                                    <label
                                                                        htmlFor={`${module.key}-${action}`}
                                                                        className={cn(
                                                                            "text-[11px] font-medium leading-none cursor-pointer select-none",
                                                                            readOnly && "cursor-default"
                                                                        )}
                                                                    >
                                                                        {translateAction(action)}
                                                                    </label>
                                                                </div>
                                                            ))}
                                                        </div>
                                                    </div>
                                                );
                                            })
                                        ) : (
                                            <div className="col-span-full py-12 flex flex-col items-center justify-center border rounded-xl border-dashed bg-zinc-50/30">
                                                <Shield className="h-8 w-8 text-muted-foreground/30 mb-2" />
                                                <p className="text-sm text-muted-foreground">Nenhum módulo de permissão disponível.</p>
                                            </div>
                                        )}
                                    </div>
                                </div>
                            </div>
                        </div>

                        <DialogFooter className="p-6 border-t bg-background shrink-0">
                            <Button type="button" variant="outline" onClick={() => onOpenChange(false)}>
                                {readOnly ? "Fechar" : "Cancelar"}
                            </Button>
                            {!readOnly && (
                                <Button type="submit" disabled={mutation.isPending}>
                                    {mutation.isPending ? "Salvando..." : role ? "Atualizar Perfil" : "Criar Perfil"}
                                </Button>
                            )}
                        </DialogFooter>
                    </form>
                </Form>
            </DialogContent>
        </Dialog>
    );
};

const translateAction = (action: string) => {
    const translations: Record<string, string> = {
        "CREATE": "Criar",
        "READ": "Visualizar",
        "UPDATE": "Editar",
        "DELETE": "Excluir",
        "ALL": "Tudo"
    };
    return translations[action] || action;
};

export default RoleForm;
