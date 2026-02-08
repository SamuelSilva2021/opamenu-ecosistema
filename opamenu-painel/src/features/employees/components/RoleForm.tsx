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
import { ScrollArea } from "@/components/ui/scroll-area";
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
}

const RoleForm = ({ open, onOpenChange, role }: RoleFormProps) => {
    const { toast } = useToast();
    const queryClient = useQueryClient();

    const { data: modulesData } = useQuery({
        queryKey: ["available-modules"],
        queryFn: () => employeesService.getAvailableModules(),
    });

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
        mutation.mutate(data);
    };

    const togglePermission = (moduleKey: string, action: string) => {
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
            <DialogContent className="max-w-4xl max-h-[90vh] flex flex-col p-0 bg-background border">
                <DialogHeader className="p-6 pb-2">
                    <DialogTitle>{role ? "Editar Perfil" : "Novo Perfil"}</DialogTitle>
                    <DialogDescription>
                        Defina o nome e as permissões de acesso para este perfil.
                    </DialogDescription>
                </DialogHeader>

                <Form {...form}>
                    <form onSubmit={form.handleSubmit(onSubmit)} className="flex-1 flex flex-col overflow-hidden">
                        <ScrollArea className="flex-1 px-6">
                            <div className="space-y-6 py-4">
                                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                                    <FormField
                                        control={form.control}
                                        name="name"
                                        render={({ field }) => (
                                            <FormItem>
                                                <FormLabel>Nome do Perfil</FormLabel>
                                                <FormControl>
                                                    <Input placeholder="Ex: Gerente, Atendente..." {...field} />
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
                                                    {...field}
                                                />
                                            </FormControl>
                                            <FormMessage />
                                        </FormItem>
                                    )}
                                />

                                <div className="space-y-4">
                                    <h3 className="text-lg font-semibold flex items-center gap-2">
                                        <Shield className="h-5 w-5 text-primary" /> Permissões por Módulo
                                    </h3>

                                    <div className="grid grid-cols-1 gap-4">
                                        {modulesData?.data?.map((module: Module) => {
                                            function permissionsCount(key: string) {
                                                return form.watch("permissions")?.find(p => p.module === key)?.actions.length || 0;
                                            }

                                            return (
                                                <div key={module.key} className="rounded-lg border bg-zinc-50/50 dark:bg-zinc-900/50 p-4">
                                                    <div className="flex items-center justify-between mb-4">
                                                        <div>
                                                            <h4 className="font-medium text-sm">{module.name}</h4>
                                                            <p className="text-xs text-muted-foreground">{module.description}</p>
                                                        </div>
                                                        <Button
                                                            type="button"
                                                            variant="ghost"
                                                            size="sm"
                                                            className="text-[10px] h-7"
                                                            onClick={() => toggleAllActions(module.key, module.availableActions)}
                                                        >
                                                            {permissionsCount(module.key) === module.availableActions.length ? "Desmarcar Todos" : "Marcar Todos"}
                                                        </Button>
                                                    </div>
                                                    <div className="flex flex-wrap gap-4">
                                                        {module.availableActions.map((action) => (
                                                            <div key={action} className="flex items-center space-x-2">
                                                                <Checkbox
                                                                    id={`${module.key}-${action}`}
                                                                    checked={isActionSelected(module.key, action)}
                                                                    onCheckedChange={() => togglePermission(module.key, action)}
                                                                />
                                                                <label
                                                                    htmlFor={`${module.key}-${action}`}
                                                                    className="text-xs font-medium leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70 cursor-pointer"
                                                                >
                                                                    {translateAction(action)}
                                                                </label>
                                                            </div>
                                                        ))}
                                                    </div>
                                                </div>
                                            );
                                        })}
                                    </div>
                                </div>
                            </div>
                        </ScrollArea>

                        <DialogFooter className="p-6 pt-2 border-t mt-auto">
                            <Button type="button" variant="outline" onClick={() => onOpenChange(false)}>
                                Cancelar
                            </Button>
                            <Button type="submit" disabled={mutation.isPending}>
                                {mutation.isPending ? "Salvando..." : role ? "Atualizar Perfil" : "Criar Perfil"}
                            </Button>
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
