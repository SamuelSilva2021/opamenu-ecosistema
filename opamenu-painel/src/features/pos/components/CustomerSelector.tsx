import { useState, useEffect } from "react";
import { useInfiniteQuery } from "@tanstack/react-query";
import { customersService } from "../../customers/customers.service";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Search, UserPlus, X, Loader2 } from "lucide-react";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger } from "@/components/ui/dialog";
import type { Customer } from "../../customers/types";
import { OrderType } from "../types";

import { useDebounce } from "@/hooks/use-debounce";
import { useIntersectionObserver } from "@/hooks/use-intersection-observer";

interface CustomerSelectorProps {
  selectedCustomer: Customer | null;
  onSelect: (customer: Customer | null) => void;
  tempCustomerName?: string;
  onTempCustomerNameChange?: (name: string) => void;
  tempCustomerPhone?: string;
  onTempCustomerPhoneChange?: (phone: string) => void;
  tempCustomerAddress?: any;
  onTempCustomerAddressChange?: (address: any) => void;
  orderType?: OrderType;
}

export function CustomerSelector({ 
    selectedCustomer, 
    onSelect, 
    tempCustomerName, 
    onTempCustomerNameChange,
    tempCustomerPhone,
    onTempCustomerPhoneChange,
    onTempCustomerAddressChange,
    orderType
}: CustomerSelectorProps) {
  const [open, setOpen] = useState(false);
  const [search, setSearch] = useState("");
  const debouncedSearch = useDebounce(search, 500);
  const [isTempCustomer, setIsTempCustomer] = useState(false);
  
  // Estados locais para edição antes de confirmar
  const [localName, setLocalName] = useState("");
  const [localPhone, setLocalPhone] = useState("");
  
  // Estados de endereço
  const [localZipCode, setLocalZipCode] = useState("");
  const [localStreet, setLocalStreet] = useState("");
  const [localNumber, setLocalNumber] = useState("");
  const [localComplement, setLocalComplement] = useState("");
  const [localNeighborhood, setLocalNeighborhood] = useState("");
  const [localCity, setLocalCity] = useState("");
  const [localState, setLocalState] = useState("");

  const { 
    data, 
    fetchNextPage, 
    hasNextPage, 
    isFetchingNextPage, 
    isLoading 
  } = useInfiniteQuery({
    queryKey: ["customers", { search: debouncedSearch }],
    queryFn: ({ pageParam }) => customersService.getCustomersPaginated({ 
      page: pageParam as number, 
      limit: 5, 
      search: debouncedSearch 
    }),
    initialPageParam: 1,
    getNextPageParam: (lastPage) => lastPage.nextCursor,
    enabled: open && !isTempCustomer,
  });

  const customers = data?.pages.flatMap(page => page.data) || [];
  
  const { ref: loadMoreRef, isIntersecting } = useIntersectionObserver();

  useEffect(() => {
    if (isIntersecting && hasNextPage) {
      fetchNextPage();
    }
  }, [isIntersecting, hasNextPage, fetchNextPage]);

  const handleClear = () => {
    onSelect(null);
    onTempCustomerNameChange?.("");
    onTempCustomerPhoneChange?.("");
    onTempCustomerAddressChange?.(null);
    setLocalName("");
    setLocalPhone("");
    setLocalZipCode("");
    setLocalStreet("");
    setLocalNumber("");
    setLocalComplement("");
    setLocalNeighborhood("");
    setLocalCity("");
    setLocalState("");
    setIsTempCustomer(false);
  };

  const handleConfirmTempCustomer = () => {
    onTempCustomerNameChange?.(localName);
    // Remove tudo que não é número antes de enviar
    const plainPhone = localPhone.replace(/\D/g, "");
    onTempCustomerPhoneChange?.(plainPhone);

    if (orderType === OrderType.Delivery) {
        onTempCustomerAddressChange?.({
            zipCode: localZipCode,
            street: localStreet,
            number: localNumber,
            complement: localComplement,
            neighborhood: localNeighborhood,
            city: localCity,
            state: localState
        });
    }

    setOpen(false);
  };

  const handlePhoneChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    let value = e.target.value.replace(/\D/g, "");
    if (value.length > 11) value = value.slice(0, 11);

    let formatted = value;
    if (value.length > 10) {
      formatted = value.replace(/^(\d{2})(\d{5})(\d{4}).*/, "($1) $2-$3");
    } else if (value.length > 6) {
      formatted = value.replace(/^(\d{2})(\d{4})(\d{0,4}).*/, "($1) $2-$3");
    } else if (value.length > 2) {
      formatted = value.replace(/^(\d{2})(\d{0,5})/, "($1) $2");
    } else if (value.length > 0) {
      formatted = value.replace(/^(\d*)/, "($1");
    }
    
    setLocalPhone(formatted);
  };

  // Sincronizar estados locais quando abrir o modal ou mudar as props, se necessário
  // Mas como o objetivo é limpar ao cancelar, podemos deixar assim.
  // Apenas garantimos que se já houver valor no pai, ele venha para o local ao editar?
  // Por enquanto, assumimos que ao limpar tudo reseta.


  return (
    <div className="flex flex-col gap-2">
      {selectedCustomer ? (
        <div className="flex items-center justify-between p-2 border rounded-md bg-muted/50">
          <div>
            <p className="font-medium text-sm">{selectedCustomer.name}</p>
            <p className="text-xs text-muted-foreground">{selectedCustomer.phone}</p>
          </div>
          <Button variant="ghost" size="icon" onClick={handleClear} className="h-8 w-8">
            <X className="h-4 w-4" />
          </Button>
        </div>
      ) : tempCustomerName ? (
        <div className="flex items-center justify-between p-2 border rounded-md bg-orange-50 dark:bg-orange-950/20 border-orange-200 dark:border-orange-800">
             <div>
                <p className="font-medium text-sm">{tempCustomerName} (Não cadastrado)</p>
                <p className="text-xs text-muted-foreground">{tempCustomerPhone}</p>
             </div>
             <Button variant="ghost" size="icon" onClick={handleClear} className="h-8 w-8">
                <X className="h-4 w-4" />
             </Button>
        </div>
      ) : (
        <Dialog open={open} onOpenChange={setOpen}>
          <DialogTrigger asChild>
            <Button variant="outline" className="w-full justify-start text-muted-foreground">
              <UserPlus className="mr-2 h-4 w-4" />
              Selecionar Cliente
            </Button>
          </DialogTrigger>
          <DialogContent>
            <DialogHeader>
              <DialogTitle>Selecionar Cliente</DialogTitle>
            </DialogHeader>
            
            <div className="flex flex-col gap-4 py-4">
                <div className="flex gap-2">
                    <Button 
                        variant={!isTempCustomer ? "default" : "outline"} 
                        onClick={() => setIsTempCustomer(false)}
                        className="flex-1"
                    >
                        Cadastrado
                    </Button>
                    <Button 
                        variant={isTempCustomer ? "default" : "outline"} 
                        onClick={() => setIsTempCustomer(true)}
                        className="flex-1"
                    >
                        Novo / Rápido
                    </Button>
                </div>

                {!isTempCustomer ? (
                    <>
                        <div className="relative">
                            <Search className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
                            <Input
                            placeholder="Buscar por nome ou telefone..."
                            className="pl-8"
                            value={search}
                            onChange={(e) => setSearch(e.target.value)}
                            />
                            {(isLoading || isFetchingNextPage) && (
                                <div className="absolute right-2.5 top-2.5">
                                    <Loader2 className="h-4 w-4 animate-spin text-muted-foreground" />
                                </div>
                            )}
                        </div>
                        <div className="max-h-[300px] overflow-y-auto flex flex-col gap-1">
                            {customers.map((customer) => (
                                <Button
                                    key={customer.id}
                                    variant="ghost"
                                    className="justify-start font-normal h-auto py-2"
                                    onClick={() => {
                                        onSelect(customer);
                                        setOpen(false);
                                    }}
                                >
                                    <div className="flex flex-col items-start">
                                        <span className="font-medium">{customer.name}</span>
                                        {customer.phone && (
                                            <span className="text-xs text-muted-foreground">{customer.phone}</span>
                                        )}
                                    </div>
                                </Button>
                            ))}
                            
                            {/* Elemento sentinela para carregar mais */}
                            {hasNextPage && (
                                <div ref={loadMoreRef} className="py-2 text-center text-xs text-muted-foreground">
                                    {isFetchingNextPage ? "Carregando mais..." : "Role para ver mais"}
                                </div>
                            )}
                            
                            {!isLoading && customers.length === 0 && (
                                <div className="py-4 text-center text-sm text-muted-foreground">
                                    Nenhum cliente encontrado
                                </div>
                            )}
                        </div>
                    </>
                ) : (
                    <div className="flex flex-col gap-3">
                         <div className="grid gap-1">
                             <label className="text-sm font-medium">Nome</label>
                             <Input 
                                placeholder="Nome do cliente" 
                                value={localName}
                                onChange={(e) => setLocalName(e.target.value)}
                             />
                         </div>
                         <div className="grid gap-1">
                             <label className="text-sm font-medium">Telefone</label>
                             <Input 
                                placeholder="(00) 00000-0000" 
                                value={localPhone}
                                onChange={handlePhoneChange}
                             />
                         </div>
                         
                         {orderType === OrderType.Delivery && (
                             <div className="space-y-3 pt-2 border-t mt-2">
                                <h4 className="font-medium text-sm text-muted-foreground">Endereço de Entrega</h4>
                                
                                <div className="grid grid-cols-2 gap-2">
                                    <div className="grid gap-1">
                                        <label className="text-xs font-medium">CEP</label>
                                        <Input 
                                            placeholder="00000-000" 
                                            value={localZipCode}
                                            onChange={(e) => setLocalZipCode(e.target.value)}
                                            className="h-8"
                                        />
                                    </div>
                                    <div className="grid gap-1">
                                        <label className="text-xs font-medium">Estado (UF)</label>
                                        <Input 
                                            placeholder="SP" 
                                            value={localState}
                                            onChange={(e) => setLocalState(e.target.value.toUpperCase())}
                                            maxLength={2}
                                            className="h-8"
                                        />
                                    </div>
                                </div>

                                <div className="grid gap-1">
                                    <label className="text-xs font-medium">Cidade</label>
                                    <Input 
                                        placeholder="Cidade" 
                                        value={localCity}
                                        onChange={(e) => setLocalCity(e.target.value)}
                                        className="h-8"
                                    />
                                </div>

                                <div className="grid gap-1">
                                    <label className="text-xs font-medium">Bairro</label>
                                    <Input 
                                        placeholder="Bairro" 
                                        value={localNeighborhood}
                                        onChange={(e) => setLocalNeighborhood(e.target.value)}
                                        className="h-8"
                                    />
                                </div>

                                <div className="grid grid-cols-3 gap-2">
                                    <div className="col-span-2 grid gap-1">
                                        <label className="text-xs font-medium">Rua</label>
                                        <Input 
                                            placeholder="Rua" 
                                            value={localStreet}
                                            onChange={(e) => setLocalStreet(e.target.value)}
                                            className="h-8"
                                        />
                                    </div>
                                    <div className="grid gap-1">
                                        <label className="text-xs font-medium">Número</label>
                                        <Input 
                                            placeholder="123" 
                                            value={localNumber}
                                            onChange={(e) => setLocalNumber(e.target.value)}
                                            className="h-8"
                                        />
                                    </div>
                                </div>

                                <div className="grid gap-1">
                                    <label className="text-xs font-medium">Complemento</label>
                                    <Input 
                                        placeholder="Apto 101, Casa 2..." 
                                        value={localComplement}
                                        onChange={(e) => setLocalComplement(e.target.value)}
                                        className="h-8"
                                    />
                                </div>
                             </div>
                         )}

                         <Button 
                            className="mt-2 w-full" 
                            onClick={handleConfirmTempCustomer}
                            disabled={!localName || !localPhone || (orderType === OrderType.Delivery && (!localStreet || !localNumber))}
                         >
                            Confirmar
                         </Button>
                    </div>
                )}
            </div>
          </DialogContent>
        </Dialog>
      )}
    </div>
  );
}
