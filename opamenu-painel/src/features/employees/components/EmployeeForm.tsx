import { useEffect } from "react";
import { useForm, type Resolver } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import * as z from "zod";
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
import { Switch } from "@/components/ui/switch";
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from "@/components/ui/select";
import type { Employee, CreateEmployeeRequest, UpdateEmployeeRequest, Role } from "../types";

const formSchema = z.object({
    username: z.string().min(3, "Username deve ter pelo menos 3 caracteres"),
    email: z.string().email("E-mail inválido"),
    firstName: z.string().min(2, "Nome deve ter pelo menos 2 caracteres"),
    lastName: z.string().min(2, "Sobrenome deve ter pelo menos 2 caracteres"),
    password: z.string().min(6, "Senha deve ter pelo menos 6 caracteres").optional().or(z.literal("")),
    confirmPassword: z.string().optional().or(z.literal("")),
    isActive: z.boolean().default(true),
    roleId: z.string().optional(),
}).refine((data) => {
    if (!data.password && !data.confirmPassword) return true;
    return data.password === data.confirmPassword;
}, {
    message: "As senhas não coincidem",
    path: ["confirmPassword"],
});

interface EmployeeFormProps {
    open: boolean;
    onOpenChange: (open: boolean) => void;
    onSubmit: (data: CreateEmployeeRequest | UpdateEmployeeRequest) => void;
    initialData?: Employee | null;
    roles?: Role[];
    isLoading?: boolean;
}

export function EmployeeForm({
    open,
    onOpenChange,
    onSubmit,
    initialData,
    roles = [],
    isLoading,
}: EmployeeFormProps) {
    const form = useForm<z.infer<typeof formSchema>>({
        resolver: zodResolver(formSchema) as Resolver<z.infer<typeof formSchema>>,
        defaultValues: {
            username: "",
            email: "",
            firstName: "",
            lastName: "",
            password: "",
            confirmPassword: "",
            isActive: true,
            roleId: "",
        },
    });

    useEffect(() => {
        if (open) {
            if (initialData) {
                form.reset({
                    username: initialData.username,
                    email: initialData.email,
                    firstName: initialData.firstName,
                    lastName: initialData.lastName,
                    password: "",
                    confirmPassword: "",
                    isActive: initialData.status === "Ativo",
                    roleId: initialData.roleId || "",
                });
            } else {
                form.reset({
                    username: "",
                    email: "",
                    firstName: "",
                    lastName: "",
                    password: "",
                    confirmPassword: "",
                    isActive: true,
                    roleId: "",
                });
            }
        }
    }, [initialData, form, open]);

    const handleSubmit = (values: z.infer<typeof formSchema>) => {
        const { confirmPassword, isActive, ...data } = values;

        // Na edição, não enviamos a senha se estiver vazia
        if (initialData && !data.password) {
            delete data.password;
        }

        onSubmit({
            ...data,
            status: isActive ? "Ativo" : "Inativo",
        } as CreateEmployeeRequest | UpdateEmployeeRequest);
    };

    return (
        <Dialog open={open} onOpenChange={onOpenChange}>
            <DialogContent className="sm:max-w-[500px]">
                <DialogHeader>
                    <DialogTitle>
                        {initialData ? "Editar Colaborador" : "Novo Colaborador"}
                    </DialogTitle>
                </DialogHeader>

                <Form {...form}>
                    <form onSubmit={form.handleSubmit(handleSubmit)} className="space-y-4">
                        <div className="grid grid-cols-2 gap-4">
                            <FormField
                                control={form.control}
                                name="firstName"
                                render={({ field }) => (
                                    <FormItem>
                                        <FormLabel>Nome</FormLabel>
                                        <FormControl>
                                            <Input placeholder="Ex: João" {...field} />
                                        </FormControl>
                                        <FormMessage />
                                    </FormItem>
                                )}
                            />
                            <FormField
                                control={form.control}
                                name="lastName"
                                render={({ field }) => (
                                    <FormItem>
                                        <FormLabel>Sobrenome</FormLabel>
                                        <FormControl>
                                            <Input placeholder="Ex: Silva" {...field} />
                                        </FormControl>
                                        <FormMessage />
                                    </FormItem>
                                )}
                            />
                        </div>

                        <FormField
                            control={form.control}
                            name="username"
                            render={({ field }) => (
                                <FormItem>
                                    <FormLabel>Usuário (Login)</FormLabel>
                                    <FormControl>
                                        <Input placeholder="ex: joao.silva" disabled={!!initialData} {...field} />
                                    </FormControl>
                                    <FormDescription>Nome único para acesso ao sistema</FormDescription>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />

                        <FormField
                            control={form.control}
                            name="email"
                            render={({ field }) => (
                                <FormItem>
                                    <FormLabel>E-mail</FormLabel>
                                    <FormControl>
                                        <Input type="email" placeholder="ex: joao@empresa.com" {...field} />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />

                        {!initialData && (
                            <div className="grid grid-cols-2 gap-4">
                                <FormField
                                    control={form.control}
                                    name="password"
                                    render={({ field }) => (
                                        <FormItem>
                                            <FormLabel>Senha</FormLabel>
                                            <FormControl>
                                                <Input type="password" {...field} />
                                            </FormControl>
                                            <FormMessage />
                                        </FormItem>
                                    )}
                                />
                                <FormField
                                    control={form.control}
                                    name="confirmPassword"
                                    render={({ field }) => (
                                        <FormItem>
                                            <FormLabel>Confirmar Senha</FormLabel>
                                            <FormControl>
                                                <Input type="password" {...field} />
                                            </FormControl>
                                            <FormMessage />
                                        </FormItem>
                                    )}
                                />
                            </div>
                        )}

                        <FormField
                            control={form.control}
                            name="roleId"
                            render={({ field }) => (
                                <FormItem>
                                    <FormLabel>Perfil (Role)</FormLabel>
                                    <Select
                                        onValueChange={(value) => field.onChange(value)}
                                        defaultValue={field.value}
                                        value={field.value}
                                    >
                                        <FormControl>
                                            <SelectTrigger>
                                                <SelectValue placeholder="Selecione um perfil" />
                                            </SelectTrigger>
                                        </FormControl>
                                        <SelectContent>
                                            {roles.map((role) => (
                                                <SelectItem key={role.id} value={role.id}>
                                                    {role.name}
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
                                <FormItem className="flex flex-row items-center justify-between rounded-lg border border-zinc-200 p-4 shadow-none">
                                    <div className="space-y-0.5">
                                        <FormLabel className="text-base">Ativo</FormLabel>
                                        <FormDescription>
                                            Colaborador com acesso habilitado
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

                        <DialogFooter>
                            <Button type="button" variant="outline" onClick={() => onOpenChange(false)}>
                                Cancelar
                            </Button>
                            <Button type="submit" disabled={isLoading}>
                                {isLoading ? "Salvando..." : "Salvar"}
                            </Button>
                        </DialogFooter>
                    </form>
                </Form>
            </DialogContent>
        </Dialog>
    );
}
