import { useState, useEffect } from "react";
import { useQuery } from "@tanstack/react-query";
import {
    Dialog,
    DialogContent,
    DialogHeader,
    DialogTitle,
    DialogFooter,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { employeesService } from "../employees.service";
import type { Employee } from "../types";
import { Badge } from "@/components/ui/badge";
import { Label } from "@/components/ui/label";
import { Shield, ChevronLeft, ChevronRight, Search, Check } from "lucide-react";
import { Input } from "@/components/ui/input";
import { cn } from "@/lib/utils";

interface EmployeeRoleModalProps {
    open: boolean;
    onOpenChange: (open: boolean) => void;
    employee: Employee | null;
    onSuccess: (roleId: string) => void;
    isLoading?: boolean;
}

export function EmployeeRoleModal({
    open,
    onOpenChange,
    employee,
    onSuccess,
    isLoading: isSubmitting,
}: EmployeeRoleModalProps) {
    const [selectedRoleId, setSelectedRoleId] = useState<string>("");
    const [page, setPage] = useState(1);
    const [searchTerm, setSearchTerm] = useState("");
    const limit = 5;

    const { data: rolesResponse, isLoading: isLoadingRoles } = useQuery({
        queryKey: ["roles", page, searchTerm],
        queryFn: () => employeesService.getRoles({ page, limit, search: searchTerm }),
        enabled: open,
    });

    useEffect(() => {
        if (open && employee) {
            setSelectedRoleId(employee.roleId || "");
            setPage(1);
            setSearchTerm("");
        }
    }, [open, employee]);

    const handleSave = () => {
        if (selectedRoleId) {
            onSuccess(selectedRoleId);
        }
    };

    const roles = rolesResponse?.data?.items || [];
    const totalItems = rolesResponse?.data?.total || 0;
    const totalPages = Math.ceil(totalItems / limit);

    return (
        <Dialog open={open} onOpenChange={onOpenChange}>
            <DialogContent className="sm:max-w-[500px]">
                <DialogHeader>
                    <DialogTitle className="flex items-center gap-2">
                        <Shield className="h-5 w-5 text-primary" />
                        Vincular Perfil
                    </DialogTitle>
                    <div className="text-sm text-muted-foreground mt-1">
                        Defina o cargo de {employee?.fullName}
                    </div>
                </DialogHeader>

                <div className="space-y-4 py-4">
                    <div className="relative">
                        <Search className="absolute left-2 top-2.5 h-4 w-4 text-muted-foreground" />
                        <Input
                            placeholder="Buscar perfis..."
                            className="pl-8"
                            value={searchTerm}
                            onChange={(e) => {
                                setSearchTerm(e.target.value);
                                setPage(1);
                            }}
                        />
                    </div>

                    <div className="space-y-3">
                        {isLoadingRoles ? (
                            <div className="space-y-2 py-8 text-center text-sm text-muted-foreground animate-pulse">
                                Carregando perfis...
                            </div>
                        ) : roles.length > 0 ? (
                            <div className="grid gap-2">
                                {roles.map((role) => {
                                    const isSelected = selectedRoleId === role.id;
                                    return (
                                        <div
                                            key={role.id}
                                            onClick={() => setSelectedRoleId(role.id)}
                                            className={cn(
                                                "flex items-start gap-4 rounded-lg border p-4 transition-all cursor-pointer hover:bg-zinc-50 dark:hover:bg-zinc-800/50",
                                                isSelected
                                                    ? "border-primary bg-primary/5 ring-1 ring-primary"
                                                    : "border-zinc-200 dark:border-zinc-800"
                                            )}
                                        >
                                            <div className={cn(
                                                "mt-0.5 flex h-4 w-4 shrink-0 items-center justify-center rounded-full border border-primary text-primary-foreground",
                                                isSelected ? "bg-primary" : "bg-transparent"
                                            )}>
                                                {isSelected && <Check className="h-3 w-3 text-white" />}
                                            </div>
                                            <Label className="flex flex-1 flex-col cursor-pointer pointer-events-none">
                                                <span className="font-semibold text-sm">{role.name}</span>
                                                {role.description && (
                                                    <span className="text-xs text-muted-foreground line-clamp-2 mt-0.5">
                                                        {role.description}
                                                    </span>
                                                )}
                                                {role.isDefault && (
                                                    <div className="mt-1">
                                                        <Badge variant="secondary" className="text-[10px] h-4 px-1">Padrão</Badge>
                                                    </div>
                                                )}
                                            </Label>
                                        </div>
                                    );
                                })}
                            </div>
                        ) : (
                            <div className="text-center py-8 text-muted-foreground">
                                Nenhum perfil encontrado.
                            </div>
                        )}

                        {totalPages > 1 && (
                            <div className="flex items-center justify-between pt-2">
                                <Button
                                    variant="outline"
                                    size="sm"
                                    onClick={() => setPage(p => Math.max(1, p - 1))}
                                    disabled={page === 1}
                                >
                                    <ChevronLeft className="h-4 w-4" />
                                </Button>
                                <span className="text-xs text-muted-foreground">
                                    Página {page} de {totalPages}
                                </span>
                                <Button
                                    variant="outline"
                                    size="sm"
                                    onClick={() => setPage(p => Math.min(totalPages, p + 1))}
                                    disabled={page === totalPages}
                                >
                                    <ChevronRight className="h-4 w-4" />
                                </Button>
                            </div>
                        )}
                    </div>
                </div>

                <DialogFooter>
                    <Button variant="outline" onClick={() => onOpenChange(false)}>
                        Cancelar
                    </Button>
                    <Button onClick={handleSave} disabled={!selectedRoleId || isSubmitting}>
                        {isSubmitting ? "Salvando..." : "Confirmar Seleção"}
                    </Button>
                </DialogFooter>
            </DialogContent>
        </Dialog>
    );
}
