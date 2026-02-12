import { Dialog, DialogContent, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Minus, Plus } from "lucide-react";
import { useState, useMemo, useRef, useEffect } from "react";
import { ProductWithAddons, AddonGroup, Addon, SelectedAddon, ProductSelection, AddonGroupType } from "@/types/api";
import { getFullImageUrl } from "@/utils/image-url";

interface ProductModalProps {
  isOpen: boolean;
  onClose: () => void;
  product: ProductWithAddons | null;
  onAddToCart: (selection: ProductSelection) => void;
  initialSelectedAddons?: SelectedAddon[];
  initialQuantity?: number;
  confirmLabel?: string;
}

const DEFAULT_ADDONS: SelectedAddon[] = [];

const ProductModal = ({
  isOpen,
  onClose,
  product,
  onAddToCart,
  initialSelectedAddons = DEFAULT_ADDONS,
  initialQuantity = 1,
  confirmLabel = "Adicionar"
}: ProductModalProps) => {
  const [selectedAddons, setSelectedAddons] = useState<SelectedAddon[]>(initialSelectedAddons);
  const [quantity, setQuantity] = useState(initialQuantity);
  const scrollContainerRef = useRef<HTMLDivElement>(null);
  const groupRefs = useRef<{ [key: string]: HTMLDivElement | null }>({});

  // Resetar estado quando modal abre/fecha
  const handleModalChange = (open: boolean) => {
    if (!open) {
      // Delay reset to avoid flicker
      setTimeout(() => {
        setSelectedAddons([]);
        setQuantity(1);
      }, 200);
      onClose();
    }
  };

  // Reset/Initialize when product/props change
  useEffect(() => {
    if (isOpen) {
      setSelectedAddons(initialSelectedAddons);
      setQuantity(initialQuantity);
    }
  }, [isOpen, initialSelectedAddons, initialQuantity]);

  // Calcular preço total
  const totalPrice = useMemo(() => {
    if (!product) return 0;

    const basePrice = product.price * quantity;
    const addonsPrice = selectedAddons.reduce((sum, selectedAddon) => {
      return sum + (selectedAddon.addon.price * selectedAddon.quantity);
    }, 0) * quantity;

    return basePrice + addonsPrice;
  }, [product, selectedAddons, quantity]);

  // Formatar preço
  const formatPrice = (price: number) => {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(price);
  };

  // Verificar se um addon está selecionado
  const isAddonSelected = (addonId: string) => {
    return selectedAddons.some(selected => selected.addonId === addonId);
  };

  // Obter quantidade de um addon selecionado
  const getAddonQuantity = (addonId: string) => {
    const selected = selectedAddons.find(selected => selected.addonId === addonId);
    return selected?.quantity || 0;
  };

  // Obter quantidade total selecionada em um grupo
  const getGroupTotalQuantity = (group: AddonGroup) => {
    return selectedAddons
      .filter(selected => group.addons.some(addon => addon.id === selected.addonId))
      .reduce((sum, selected) => sum + selected.quantity, 0);
  };

  // Adicionar ou atualizar addon
  const handleAddonChange = (addon: Addon, addonGroup: AddonGroup, newQuantity: number) => {
    setSelectedAddons(prev => {
      let updated = [...prev];
      const existingIndex = updated.findIndex(selected => selected.addonId === addon.id);

      if (newQuantity === 0) {
        // Remover addon
        if (existingIndex >= 0) {
          updated.splice(existingIndex, 1);
        }
      } else {
        // Para grupos de seleção única, remover outros addons do mesmo grupo
        if (addonGroup.type === AddonGroupType.Single) {
          updated = updated.filter(selected =>
            !addonGroup.addons.some(groupAddon => groupAddon.id === selected.addonId)
          );
        }

        if (existingIndex >= 0) {
          // Atualizar existente
          updated[existingIndex] = { ...updated[existingIndex], quantity: newQuantity };
        } else {
          // Adicionar novo
          updated.push({ addonId: addon.id, quantity: newQuantity, addon });
        }
      }

      return updated;
    });
  };

  // Verificar se um grupo obrigatório tem seleção
  const isRequiredGroupValid = (group: AddonGroup) => {
    if (!group.isRequired) return true;

    const groupSelections = selectedAddons.filter(selected =>
      group.addons.some(addon => addon.id === selected.addonId)
    );

    const totalSelected = groupSelections.reduce((sum, selected) => sum + selected.quantity, 0);

    if (group.minSelections && totalSelected < group.minSelections) return false;
    if (group.maxSelections && totalSelected > group.maxSelections) return false;

    return totalSelected > 0;
  };

  // Verificar se pode adicionar ao carrinho
  const canAddToCart = useMemo(() => {
    if (!product) return false;

    return product.addonGroups.every(group => isRequiredGroupValid(group));
  }, [product, selectedAddons]);

  // Encontrar primeiro grupo obrigatório não válido
  const getFirstInvalidRequiredGroup = () => {
    if (!product) return null;

    for (const group of product.addonGroups) {
      if (!isRequiredGroupValid(group)) {
        return group;
      }
    }
    return null;
  };

  // Rolar para grupo específico
  const scrollToGroup = (groupId: string) => {
    const groupElement = groupRefs.current[groupId];
    const scrollContainer = scrollContainerRef.current;

    if (groupElement && scrollContainer) {
      const containerRect = scrollContainer.getBoundingClientRect();
      const elementRect = groupElement.getBoundingClientRect();

      // Calcular posição relativa dentro do container
      const scrollTop = scrollContainer.scrollTop + (elementRect.top - containerRect.top) - 20; // 20px de margem

      scrollContainer.scrollTo({
        top: scrollTop,
        behavior: 'smooth'
      });

      // Destacar o grupo temporariamente
      groupElement.style.animation = 'pulse 2s ease-in-out';
      setTimeout(() => {
        if (groupElement) {
          groupElement.style.animation = '';
        }
      }, 2000);
    }
  };

  // Adicionar ao carrinho
  const handleAddToCart = () => {
    if (!product) return;

    // Verificar se todos os grupos obrigatórios estão válidos
    const invalidGroup = getFirstInvalidRequiredGroup();

    if (invalidGroup) {
      // Se há grupo obrigatório não preenchido, rolar para ele
      scrollToGroup(invalidGroup.id);
      return;
    }

    // Se tudo válido, adicionar ao carrinho
    const selection: ProductSelection = {
      product: {
        id: product.id,
        name: product.name,
        description: product.description,
        price: product.price,
        categoryId: product.categoryId,
        imageUrl: product.imageUrl,
        isActive: true,
        displayOrder: 0,
        createdAt: '',
        updatedAt: '',
      },
      selectedAddons,
      quantity,
    };

    onAddToCart(selection);
    handleModalChange(false);
  };

  if (!product) return null;

  return (
    <Dialog open={isOpen} onOpenChange={handleModalChange}>
      <DialogContent className="max-w-4xl h-[95vh] w-[95vw] sm:w-full flex flex-col p-0 overflow-hidden rounded-[2.5rem] border-none shadow-2xl">
        <div className="flex flex-col h-full bg-background">
          <DialogHeader className="shrink-0 p-6 pb-2">
            <DialogTitle className="text-2xl sm:text-3xl font-black uppercase italic tracking-tighter text-foreground pr-8">
              {product.name}
            </DialogTitle>
          </DialogHeader>

          <div className="flex-1 min-h-0 overflow-hidden p-6 pt-2">
            <div className="h-full flex flex-col lg:grid lg:grid-cols-2 lg:gap-8">

              <div className="shrink-0 lg:shrink flex flex-col">
                <div className="h-32 sm:h-48 lg:h-auto lg:aspect-square overflow-hidden rounded-3xl bg-white mb-3 lg:mb-0 flex items-center justify-center p-4 shadow-inner border border-border/50">
                  {product.imageUrl ? (
                    <img
                      src={getFullImageUrl(product.imageUrl) || ''}
                      alt={product.name}
                      className="w-full h-full object-contain"
                    />
                  ) : (
                    <div className="w-full h-full flex items-center justify-center bg-muted/20 rounded-2xl">
                      <div className="text-center text-muted-foreground/30">
                        <div className="text-4xl lg:text-8xl mb-2 lg:mb-4">🍔</div>
                        <p className="text-[10px] sm:text-xs font-black uppercase tracking-widest leading-none">Apetitoso por natureza</p>
                      </div>
                    </div>
                  )}
                </div>

                <div className="block lg:hidden mb-2">
                  <div className="flex items-center justify-between mb-1">
                    <span className="text-[10px] font-black text-primary/50 uppercase tracking-widest">Preço Individual</span>
                    <p className="text-xl font-black text-primary tracking-tighter">
                      {formatPrice(product.price)}
                    </p>
                  </div>
                  {product.description && (
                    <p className="text-[11px] sm:text-xs font-medium text-text-secondary leading-tight opacity-80 line-clamp-2">{product.description}</p>
                  )}
                </div>
              </div>

              <div className="flex-1 min-h-0 flex flex-col">

                <div className="hidden lg:block mb-6 shrink-0 border-b pb-6">
                  {product.description && (
                    <p className="text-lg font-medium text-text-secondary leading-relaxed mb-4">{product.description}</p>
                  )}
                  <div className="flex items-center gap-3">
                    <span className="text-xs font-black text-primary/50 uppercase tracking-widest">Base</span>
                    <p className="text-3xl font-black text-primary tracking-tighter">
                      {formatPrice(product.price)}
                    </p>
                  </div>
                </div>

                <div
                  ref={scrollContainerRef}
                  className="flex-1 min-h-0 scroll-smooth overflow-y-auto overflow-x-hidden pr-1"
                >
                  <div className="space-y-4 pb-4">
                    {product.addonGroups.length > 0 ? (
                      product.addonGroups.map((group) => (
                        <div
                          key={group.id}
                          ref={(el) => {
                            groupRefs.current[group.id] = el;
                          }}
                          className={`space-y-4 p-4 rounded-[1.5rem] border-2 transition-all duration-500 ${!isRequiredGroupValid(group) && group.isRequired
                            ? 'bg-primary/5 border-primary shadow-lg shadow-primary/5'
                            : 'bg-white border-border/50 shadow-sm'
                            }`}
                        >
                          <div className="flex flex-col gap-2">
                            <div className="flex items-center justify-between">
                              <h4 className="text-base lg:text-xl font-black uppercase italic tracking-tighter text-foreground flex items-center gap-2">
                                {group.name}
                                {!isRequiredGroupValid(group) && group.isRequired && (
                                  <span className="text-primary text-xl animate-bounce">!</span>
                                )}
                              </h4>
                              {group.isRequired && (
                                <Badge
                                  className={`font-black uppercase tracking-widest text-[9px] px-2 h-5 ${!isRequiredGroupValid(group)
                                    ? "bg-primary text-white"
                                    : "bg-muted text-foreground"
                                    }`}
                                >
                                  Obrigatório
                                </Badge>
                              )}
                            </div>

                            <p className="text-[10px] font-bold text-text-secondary/60 bg-muted/40 w-fit px-2 py-0.5 rounded-full">
                              {group.type === AddonGroupType.Single ? 'Escolha 1' : `Max ${group.maxSelections || '∞'}`}
                            </p>
                          </div>

                          {group.description && (
                            <p className="text-[11px] font-medium text-text-secondary/70 italic border-l-2 border-primary/20 pl-3">
                              {group.description}
                            </p>
                          )}

                          <div className="grid grid-cols-1 gap-2">
                            {group.addons.filter(addon => addon.isActive).map((addon) => (
                              <div
                                key={addon.id}
                                className={`flex items-center justify-between p-3 rounded-xl border-2 transition-all duration-300 gap-3 cursor-pointer ${isAddonSelected(addon.id)
                                  ? 'bg-accent/10 border-accent'
                                  : 'bg-white border-border hover:border-primary/50'
                                  }`}
                                onClick={() => {
                                  if (group.type === AddonGroupType.Single) {
                                    handleAddonChange(addon, group, isAddonSelected(addon.id) ? 0 : 1);
                                  }
                                }}
                              >
                                <div className="flex-1 min-w-0">
                                  <h5 className="text-xs sm:text-sm font-black uppercase tracking-tight text-foreground">{addon.name}</h5>
                                  {addon.description && (
                                    <p className="text-[10px] font-medium text-text-secondary/60 line-clamp-1">
                                      {addon.description}
                                    </p>
                                  )}
                                </div>

                                <div className="flex items-center gap-3 shrink-0">
                                  <span className="text-xs font-black text-primary whitespace-nowrap">
                                    + {formatPrice(addon.price)}
                                  </span>

                                  {group.type === AddonGroupType.Single ? (
                                    <div className={`
                                    w-5 h-5 rounded-full border-2 flex items-center justify-center transition-all duration-500
                                    ${isAddonSelected(addon.id)
                                        ? 'border-accent bg-accent'
                                        : 'border-border bg-transparent'}
                                  `}>
                                      {isAddonSelected(addon.id) && (
                                        <div className="w-2 h-2 rounded-full bg-accent-foreground" />
                                      )}
                                    </div>
                                  ) : (
                                    <div className="flex items-center gap-2" onClick={(e) => e.stopPropagation()}>
                                      <Button
                                        variant="outline"
                                        size="icon"
                                        onClick={() => handleAddonChange(addon, group, Math.max(0, getAddonQuantity(addon.id) - 1))}
                                        disabled={getAddonQuantity(addon.id) === 0}
                                        className="h-8 w-8 p-0 touch-manipulation rounded-lg border-2 font-black"
                                      >
                                        <Minus className="h-3 w-3" />
                                      </Button>
                                      <span className="w-4 text-center text-xs font-black text-foreground">
                                        {getAddonQuantity(addon.id)}
                                      </span>
                                      <Button
                                        variant="outline"
                                        size="icon"
                                        onClick={() => handleAddonChange(addon, group, getAddonQuantity(addon.id) + 1)}
                                        disabled={!!group.maxSelections && getGroupTotalQuantity(group) >= group.maxSelections}
                                        className="h-8 w-8 p-0 hover:bg-accent/10 hover:border-accent touch-manipulation rounded-lg border-2 font-black transition-all"
                                      >
                                        <Plus className="h-3 w-3" />
                                      </Button>
                                    </div>
                                  )}
                                </div>
                              </div>
                            ))}
                          </div>
                        </div>
                      ))
                    ) : (
                      <div className="text-center text-muted-foreground py-8 bg-white rounded-[2rem] border-2 border-dashed border-border/50">
                        <div className="text-4xl mb-3 opacity-20 text-primary">🍟</div>
                        <p className="text-base font-black uppercase tracking-tighter opacity-50">Sem extras</p>
                        <p className="text-[10px] font-bold mt-1 opacity-40">Este item já é perfeito como está!</p>
                      </div>
                    )}
                  </div>
                </div>
              </div>
            </div>
          </div>

          {/* Footer controls - BK Style Sticky Footer */}
          <div className="shrink-0 bg-white border-t p-4 sm:p-6 pb-6 shadow-[0_-10px_40px_-15px_rgba(0,0,0,0.1)] rounded-t-[2.5rem]">
            <div className="flex flex-col gap-4">
              {/* Info Row: Quantity and Total price */}
              <div className="flex items-center justify-between">
                <div className="flex items-center bg-muted p-1 rounded-2xl border-2 border-border/50 h-12">
                  <Button
                    variant="ghost"
                    size="icon"
                    onClick={() => setQuantity(Math.max(1, quantity - 1))}
                    disabled={quantity <= 1}
                    className="h-10 w-10 p-0 touch-manipulation hover:bg-white rounded-xl"
                  >
                    <Minus className="h-4 w-4" />
                  </Button>
                  <span className="w-8 text-center font-black text-lg tracking-tighter">{quantity}</span>
                  <Button
                    variant="ghost"
                    size="icon"
                    onClick={() => setQuantity(quantity + 1)}
                    className="h-10 w-10 p-0 touch-manipulation hover:bg-white rounded-xl"
                  >
                    <Plus className="h-4 w-4" />
                  </Button>
                </div>

                <div className="flex flex-col items-end">
                  <span className="text-[9px] font-black uppercase tracking-[0.2em] text-primary opacity-50 leading-none mb-1">Total do Item</span>
                  <span className="text-xl font-black tracking-tighter leading-none">{formatPrice(totalPrice)}</span>
                </div>
              </div>

              {/* Action Button */}
              <Button
                onClick={handleAddToCart}
                className={`h-14 sm:h-16 rounded-[1.5rem] w-full font-black uppercase italic tracking-wider text-base sm:text-lg shadow-2xl transition-all duration-500 scale-100 active:scale-95 ${canAddToCart
                  ? "bg-primary hover:bg-primary-hover text-white shadow-primary/30"
                  : "bg-amber-500 hover:bg-amber-600 text-white shadow-amber-500/30"
                  }`}
              >
                {canAddToCart ? (confirmLabel || "Adicionar") : "Escolha os obrigatórios"}
              </Button>
            </div>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
};

export default ProductModal;