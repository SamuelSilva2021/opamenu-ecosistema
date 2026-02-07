import { Dialog, DialogContent, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Minus, Plus } from "lucide-react";
import { useState, useMemo, useRef } from "react";
import { ProductWithAddons, AddonGroup, Addon, SelectedAddon, ProductSelection, AddonGroupType } from "@/types/api";
import { getFullImageUrl } from "@/utils/image-url";

interface ProductModalProps {
  isOpen: boolean;
  onClose: () => void;
  product: ProductWithAddons | null;
  onAddToCart: (selection: ProductSelection) => void;
}

const ProductModal = ({ isOpen, onClose, product, onAddToCart }: ProductModalProps) => {
  const [selectedAddons, setSelectedAddons] = useState<SelectedAddon[]>([]);
  const [quantity, setQuantity] = useState(1);
  const scrollContainerRef = useRef<HTMLDivElement>(null);
  const groupRefs = useRef<{ [key: string]: HTMLDivElement | null }>({});

  // Resetar estado quando modal abre/fecha
  const handleModalChange = (open: boolean) => {
    if (!open) {
      setSelectedAddons([]);
      setQuantity(1);
      onClose();
    }
  };

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
        categoryId: 0,
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
      <DialogContent className="max-w-4xl h-[95vh] w-[95vw] sm:w-full flex flex-col p-0 overflow-hidden">
        <div className="flex flex-col h-full">
          <DialogHeader className="shrink-0 p-4 pb-2">
            <DialogTitle className="text-lg sm:text-xl pr-8">
              {product.name}
            </DialogTitle>
          </DialogHeader>
          
          <div className="flex-1 min-h-0 overflow-hidden p-4 pt-2">
            <div className="h-full flex flex-col lg:grid lg:grid-cols-2 lg:gap-6">
              
              <div className="shrink-0 lg:shrink">
                <div className="aspect-[16/9] lg:aspect-square overflow-hidden rounded-lg bg-white mb-4 lg:mb-0 flex items-center justify-center p-4 border">
                  {product.imageUrl ? (
                    <img 
                      src={getFullImageUrl(product.imageUrl) || ''} 
                      alt={product.name}
                      className="w-full h-full object-contain"
                    />
                  ) : (
                    <div className="w-full h-full flex items-center justify-center bg-gradient-to-br from-opamenu-orange/20 to-opamenu-green/20 rounded-md">
                      <div className="text-center text-muted-foreground">
                        <div className="text-4xl lg:text-6xl mb-2 lg:mb-4">🍽️</div>
                        <p className="text-sm lg:text-base">Imagem não disponível</p>
                      </div>
                    </div>
                  )}
                </div>

                <div className="block lg:hidden mb-4">
                  <h3 className="text-lg font-semibold text-foreground mb-1">{product.name}</h3>
                  {product.description && (
                    <p className="text-sm text-muted-foreground mb-2">{product.description}</p>
                  )}
                  <p className="text-xl font-bold text-opamenu-orange">
                    {formatPrice(product.price)}
                  </p>
                </div>
              </div>
              
              <div className="flex-1 min-h-0 flex flex-col">
                
                <div className="hidden lg:block mb-4 shrink-0">
                  <h3 className="text-xl font-semibold text-foreground mb-2">{product.name}</h3>
                  {product.description && (
                    <p className="text-base text-muted-foreground mb-3">{product.description}</p>
                  )}
                  <p className="text-2xl font-bold text-opamenu-orange">
                    {formatPrice(product.price)}
                  </p>
                </div>
                
                <div 
                  ref={scrollContainerRef}
                  className="flex-1 min-h-0" 
                  style={{ 
                    maxHeight: 'calc(100vh - 400px)',
                    overflowY: 'auto',
                    overflowX: 'hidden'
                  }}
                >
                  <div className="space-y-6 pb-6 pr-2">
                    {product.addonGroups.length > 0 ? (
                      product.addonGroups.map((group) => (
                      <div 
                        key={group.id} 
                        ref={(el) => {
                          groupRefs.current[group.id] = el;
                        }}
                        className={`space-y-4 p-4 rounded-lg border transition-all duration-300 ${
                          !isRequiredGroupValid(group) && group.isRequired 
                            ? 'bg-red-50/50 border-red-200 shadow-md' 
                            : 'bg-background/50'
                        }`}
                      >
                        <div className="flex flex-col gap-2">
                          <h4 className="text-sm lg:text-base font-semibold text-foreground flex items-center gap-2">
                            {group.name}
                            {!isRequiredGroupValid(group) && group.isRequired && (
                              <span className="text-red-500 text-lg animate-bounce">⚠️</span>
                            )}
                          </h4>
                          <div className="flex gap-2 flex-wrap">
                            {group.isRequired && (
                              <Badge 
                                variant={!isRequiredGroupValid(group) ? "destructive" : "secondary"}
                                className={`text-xs ${
                                  !isRequiredGroupValid(group) 
                                    ? "bg-red-500 text-white" 
                                    : ""
                                }`}
                              >
                                Obrigatório
                              </Badge>
                            )}
                            <Badge variant="outline" className="text-xs">
                              {group.type === AddonGroupType.Single ? 'Escolha 1' : 'Múltiplas opções'}
                            </Badge>
                          </div>
                        </div>
                        
                        {group.description && (
                          <p className="text-xs lg:text-sm text-muted-foreground bg-muted/30 p-2 rounded">
                            {group.description}
                          </p>
                        )}
                        
                        <div className="space-y-3">
                          {group.addons.filter(addon => addon.isActive).map((addon) => (
                            <div 
                              key={addon.id} 
                              className={`flex items-center justify-between p-3 border rounded-lg transition-all duration-200 gap-3 cursor-pointer ${
                                isAddonSelected(addon.id) 
                                  ? 'bg-opamenu-orange/5 border-opamenu-orange/50 shadow-sm'  
                                  : 'bg-background hover:bg-muted/50 border-border'
                              }`}
                              onClick={() => {
                                if (group.type === AddonGroupType.Single) {
                                  handleAddonChange(addon, group, isAddonSelected(addon.id) ? 0 : 1);
                                }
                              }}
                            >
                              <div className="flex-1 min-w-0">
                                <h5 className="text-sm lg:text-base font-medium text-foreground">{addon.name}</h5>
                                {addon.description && (
                                  <p className="text-xs text-muted-foreground line-clamp-2 mt-0.5">
                                    {addon.description}
                                  </p>
                                )}
                              </div>
                              
                              <div className="flex items-center gap-3 shrink-0">
                                <span className="text-sm font-semibold text-opamenu-orange whitespace-nowrap">
                                  + {formatPrice(addon.price)}
                                </span>

                                {group.type === AddonGroupType.Single ? (
                                  <div className={`
                                    w-5 h-5 rounded-full border flex items-center justify-center transition-all duration-200
                                    ${isAddonSelected(addon.id) 
                                      ? 'border-opamenu-orange bg-opamenu-orange' 
                                      : 'border-muted-foreground/30 bg-transparent'}
                                  `}>
                                    {isAddonSelected(addon.id) && (
                                      <div className="w-2.5 h-2.5 rounded-full bg-white" />
                                    )}
                                  </div>
                                ) : (
                                  <div className="flex items-center gap-3" onClick={(e) => e.stopPropagation()}>
                                    <Button
                                      variant="outline"
                                      size="sm"
                                      onClick={() => handleAddonChange(addon, group, Math.max(0, getAddonQuantity(addon.id) - 1))}
                                      disabled={getAddonQuantity(addon.id) === 0}
                                      className="h-8 w-8 p-0 touch-manipulation rounded-full"
                                    >
                                      <Minus className="h-3 w-3" />
                                    </Button>
                                    <span className="w-6 text-center text-sm font-medium">
                                      {getAddonQuantity(addon.id)}
                                    </span>
                                    <Button
                                      variant="outline"
                                      size="sm"
                                      onClick={() => handleAddonChange(addon, group, getAddonQuantity(addon.id) + 1)}
                                      className="h-8 w-8 p-0 hover:bg-opamenu-orange/10 hover:border-opamenu-orange touch-manipulation rounded-full"  
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
                    <div className="text-center text-muted-foreground py-8">
                      <div className="text-4xl mb-4">🍽️</div>
                      <p className="text-sm lg:text-base">Nenhum adicional disponível</p>
                      <p className="text-xs lg:text-sm mt-2 opacity-70">Este produto não possui opções de personalização</p>
                    </div>
                  )}
                </div>
              </div>
            </div>
          </div>
          </div>
          
          {/* Footer com controles - sempre visível */}
          <div className="shrink-0 flex flex-col sm:flex-row items-stretch sm:items-center justify-between pt-4 border-t gap-4 mt-4 p-4">
            <div className="flex flex-col sm:flex-row sm:items-center gap-2 sm:gap-4 order-2 sm:order-1">
              <span className="text-sm text-muted-foreground">Quantidade:</span>
              <div className="flex items-center gap-2 justify-center sm:justify-start">
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => setQuantity(Math.max(1, quantity - 1))}
                  disabled={quantity <= 1}
                  className="h-9 w-9 p-0 touch-manipulation"
                >
                  <Minus className="h-4 w-4" />
                </Button>
                <span className="w-8 text-center font-medium text-sm lg:text-base">{quantity}</span>
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => setQuantity(quantity + 1)}
                  className="h-9 w-9 p-0 touch-manipulation"
                >
                  <Plus className="h-4 w-4" />
                </Button>
              </div>
            </div>
            
            <div className="flex flex-col sm:flex-row items-center gap-3 sm:gap-4 order-1 sm:order-2">
              <Button
                onClick={handleAddToCart}
                className={`px-6 sm:px-8 w-full sm:w-auto h-11 sm:h-10 touch-manipulation text-sm sm:text-base transition-all duration-300 ${
                  canAddToCart 
                    ? "bg-opamenu-orange hover:bg-opamenu-orange/90 text-white"   
                    : "bg-amber-500 hover:bg-amber-600 text-white"
                }`}
              >
                {canAddToCart ? `Adicionar ${formatPrice(totalPrice)}` : `${formatPrice(totalPrice)}`}
              </Button>
            </div>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
};

export default ProductModal;