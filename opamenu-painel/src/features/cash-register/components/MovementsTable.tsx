import {
    Table,
    TableBody,
    TableCell,
    TableHead,
    TableHeader,
    TableRow,
} from "@/components/ui/table";
import { Badge } from "@/components/ui/badge";
import { type CashMovement, CashMovementType } from "../types";
import { format } from "date-fns";
import { ptBR } from "date-fns/locale";
import { ArrowUpRight, ArrowDownLeft, Store, Ticket, XCircle, LogOut } from "lucide-react";

const movementConfig: Record<number, { label: string; icon: any; color: string }> = {
    [CashMovementType.Opening]: { label: "Abertura", icon: Store, color: "text-blue-500" },
    [CashMovementType.OrderPayment]: { label: "Venda", icon: Ticket, color: "text-green-500" },
    [CashMovementType.Inbound]: { label: "Suprimento", icon: ArrowUpRight, color: "text-emerald-500" },
    [CashMovementType.Outbound]: { label: "Sangria", icon: ArrowDownLeft, color: "text-red-500" },
    [CashMovementType.Reversed]: { label: "Estorno", icon: XCircle, color: "text-orange-500" },
    [CashMovementType.Closing]: { label: "Fechamento", icon: LogOut, color: "text-zinc-500" },
};

interface MovementsTableProps {
    movements: CashMovement[];
}

export function MovementsTable({ movements }: MovementsTableProps) {
    return (
        <div className="rounded-md border bg-white dark:bg-zinc-800 shadow-sm overflow-hidden">
            <Table>
                <TableHeader className="bg-zinc-50 dark:bg-zinc-900/50">
                    <TableRow>
                        <TableHead className="w-[100px]">Hora</TableHead>
                        <TableHead>Tipo</TableHead>
                        <TableHead>Descrição</TableHead>
                        <TableHead>Método</TableHead>
                        <TableHead className="text-right">Valor</TableHead>
                    </TableRow>
                </TableHeader>
                <TableBody>
                    {movements.length === 0 ? (
                        <TableRow>
                            <TableCell colSpan={5} className="h-32 text-center text-muted-foreground">
                                Nenhuma movimentação registrada neste turno.
                            </TableCell>
                        </TableRow>
                    ) : (
                        [...movements].sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()).map((movement) => {
                            const config = movementConfig[movement.type];
                            const Icon = config.icon;

                            return (
                                <TableRow key={movement.id} className="hover:bg-zinc-50/50 dark:hover:bg-zinc-900/10 transition-colors">
                                    <TableCell className="text-muted-foreground text-sm">
                                        {format(new Date(movement.createdAt), "HH:mm", { locale: ptBR })}
                                    </TableCell>
                                    <TableCell>
                                        <div className="flex items-center gap-2">
                                            <div className={`p-1.5 rounded-full bg-zinc-100 dark:bg-zinc-900 ${config.color.replace('text-', 'bg-').replace('500', '100')}`}>
                                                <Icon className={`h-4 w-4 ${config.color}`} />
                                            </div>
                                            <span className="font-medium text-sm">{config.label}</span>
                                        </div>
                                    </TableCell>
                                    <TableCell>
                                        <div className="flex flex-col">
                                            <span className="text-sm">{movement.description}</span>
                                            {movement.orderId && (
                                                <span className="text-xs text-muted-foreground font-mono">ID: {movement.orderId.substring(0, 8)}</span>
                                            )}
                                        </div>
                                    </TableCell>
                                    <TableCell>
                                        {movement.paymentMethod ? (
                                            <Badge variant="secondary" className="font-normal text-[10px] uppercase tracking-wider">
                                                {movement.paymentMethod}
                                            </Badge>
                                        ) : <span className="text-muted-foreground text-xs text-center block w-full">-</span>}
                                    </TableCell>
                                    <TableCell className={`text-right font-bold text-sm ${movement.type === CashMovementType.Outbound || movement.type === CashMovementType.Reversed ? 'text-red-500' :
                                        movement.type === CashMovementType.Inbound || movement.type === CashMovementType.OrderPayment || movement.type === CashMovementType.Opening ? 'text-emerald-500' : ''
                                        }`}>
                                        {movement.type === CashMovementType.Outbound || movement.type === CashMovementType.Reversed ? '-' : '+'}
                                        {movement.amount.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })}
                                    </TableCell>
                                </TableRow>
                            );
                        })
                    )}
                </TableBody>
            </Table>
        </div>
    );
}
