import { useState, useCallback, useEffect, useMemo, createContext, useContext, ReactNode } from 'react';
import { CartItem } from '@/types/cart';
import { Product, ProductSelection, Coupon, EDiscountType } from '@/types/api';

export interface CartContextType {
  items: CartItem[];
  totalItems: number;
  subtotal: number;
  discount: number;
  totalPrice: number;
  coupon: Coupon | null;
  addToCart: (product: Product, quantity?: number) => void;
  addProductSelection: (selection: ProductSelection) => void;
  removeFromCart: (itemId: number | string) => void;
  updateQuantity: (itemId: number | string, quantity: number) => void;
  increaseQuantity: (itemId: number | string) => void;
  decreaseQuantity: (itemId: number | string) => void;
  clearCart: () => void;
  getItemQuantity: (productId: number) => number;
  applyCoupon: (coupon: Coupon) => void;
  removeCoupon: () => void;
}

// Alias para compatibilidade
export type CartHookReturn = CartContextType;

const CartContext = createContext<CartContextType | undefined>(undefined);

const CART_STORAGE_KEY = 'opamenu-cart';

// Função para calcular preço unitário com adicionais
const calculateUnitPrice = (basePrice: number, selectedAddons: any[] = []): number => {
  const addonsPrice = selectedAddons.reduce((sum, addon) => {
    return sum + (addon.addon.price * addon.quantity);
  }, 0);
  return basePrice + addonsPrice;
};

// Helper para comparar addons
const areAddonsEqual = (addons1: any[] = [], addons2: any[] = []) => {
  if (addons1.length !== addons2.length) return false;
  
  // Ordenar para garantir comparação consistente
  const sorted1 = [...addons1].sort((a, b) => a.addon.id - b.addon.id);
  const sorted2 = [...addons2].sort((a, b) => a.addon.id - b.addon.id);
  
  return sorted1.every((item, index) => {
    const item2 = sorted2[index];
    return item.addon.id === item2.addon.id && item.quantity === item2.quantity;
  });
};

const generateCartItemId = () => Math.random().toString(36).substr(2, 9);

export const CartProvider = ({ children, slug }: { children: ReactNode; slug?: string }) => {
  const [items, setItems] = useState<CartItem[]>([]);
  const [coupon, setCoupon] = useState<Coupon | null>(null);
  const [isInitialized, setIsInitialized] = useState(false);
  
  const storageKey = slug ? `opamenu-cart-${slug}` : CART_STORAGE_KEY;

  // Carregar carrinho do localStorage
  useEffect(() => {
    try {
      const savedCart = localStorage.getItem(storageKey);
      if (savedCart) {
        const parsedCart = JSON.parse(savedCart);
        // Migração: garantir que itens antigos tenham ID se possível
        const migratedCart = parsedCart.map((item: CartItem) => ({
            ...item,
            cartItemId: item.cartItemId || generateCartItemId()
        }));
        setItems(migratedCart);
      } else {
        // Se mudou de loja ou não tem carrinho salvo, inicia vazio
        setItems([]);
      }
    } catch (error) {
      console.error('Error loading cart from localStorage:', error);
    } finally {
      setIsInitialized(true);
    }
  }, [storageKey]); // Recarregar quando a chave mudar (troca de slug)

  // Salvar carrinho no localStorage
  useEffect(() => {
    if (!slug || !isInitialized) return; // Não salvar se não tiver slug definido ou não estiver inicializado

    try {
      localStorage.setItem(storageKey, JSON.stringify(items));
    } catch (error) {
      console.error('Error saving cart to localStorage:', error);
    }
  }, [items, storageKey, slug, isInitialized]);

  // Helper para encontrar item
  const findItemIndex = useCallback((currentItems: CartItem[], itemId: number | string) => {
    if (typeof itemId === 'string') {
        return currentItems.findIndex(item => item.cartItemId === itemId);
    }
    return currentItems.findIndex(item => item.product.id === itemId);
  }, []);

  // Adicionar produto simples ao carrinho (sem adicionais)
  const addToCart = useCallback((product: Product, quantity: number = 1) => {
    if (quantity <= 0) return;

    const unitPrice = product.price;
    const totalPrice = unitPrice * quantity;

    setItems(currentItems => {
      const existingItemIndex = currentItems.findIndex(item => 
        item.product.id === product.id && (!item.selectedAddons || item.selectedAddons.length === 0)
      );
      
      if (existingItemIndex >= 0) {
        // Atualizar item existente
        return currentItems.map((item, index) =>
          index === existingItemIndex
            ? { 
                ...item, 
                quantity: item.quantity + quantity,
                totalPrice: item.unitPrice * (item.quantity + quantity)
              }
            : item
        );
      } else {
        // Adicionar novo item
        const newItem: CartItem = {
          cartItemId: generateCartItemId(),
          product,
          quantity,
          selectedAddons: [],
          unitPrice,
          totalPrice,
        };
        return [...currentItems, newItem];
      }
    });
  }, []);

  // Adicionar produto com adicionais ao carrinho
  const addProductSelection = useCallback((selection: ProductSelection) => {
    const { product, selectedAddons, quantity } = selection;
    
    if (quantity <= 0) return;

    const unitPrice = calculateUnitPrice(product.price, selectedAddons);
    const totalPrice = unitPrice * quantity;

    setItems(currentItems => {
        // Verificar se já existe um item com os mesmos adicionais
        const existingItemIndex = currentItems.findIndex(item => 
            item.product.id === product.id && 
            areAddonsEqual(item.selectedAddons, selectedAddons)
        );

        if (existingItemIndex >= 0) {
            // Atualizar item existente
            return currentItems.map((item, index) =>
              index === existingItemIndex
                ? { 
                    ...item, 
                    quantity: item.quantity + quantity,
                    totalPrice: item.unitPrice * (item.quantity + quantity)
                  }
                : item
            );
        }

      const newItem: CartItem = {
        cartItemId: generateCartItemId(),
        product,
        quantity,
        selectedAddons,
        unitPrice,
        totalPrice,
      };
      
      return [...currentItems, newItem];
    });
  }, []);

  // Remover produto do carrinho
  const removeFromCart = useCallback((itemId: number | string) => {
    setItems(currentItems => {
        if (typeof itemId === 'string') {
            return currentItems.filter(item => item.cartItemId !== itemId);
        }
        return currentItems.filter(item => item.product.id !== itemId);
    });
  }, []);

  // Atualizar quantidade
  const updateQuantity = useCallback((itemId: number | string, quantity: number) => {
    if (quantity <= 0) {
      removeFromCart(itemId);
      return;
    }

    setItems(currentItems =>
      currentItems.map(item => {
        const isTarget = typeof itemId === 'string' 
            ? item.cartItemId === itemId 
            : item.product.id === itemId;

        return isTarget
          ? { 
              ...item, 
              quantity,
              totalPrice: item.unitPrice * quantity
            }
          : item;
      })
    );
  }, [removeFromCart]);

  // Aumentar quantidade
  const increaseQuantity = useCallback((itemId: number | string) => {
    setItems(currentItems =>
      currentItems.map(item => {
        const isTarget = typeof itemId === 'string' 
            ? item.cartItemId === itemId 
            : item.product.id === itemId;

        return isTarget
          ? { 
              ...item, 
              quantity: item.quantity + 1,
              totalPrice: item.unitPrice * (item.quantity + 1)
            }
          : item;
      })
    );
  }, []);

  // Diminuir quantidade
  const decreaseQuantity = useCallback((itemId: number | string) => {
    setItems(currentItems =>
      currentItems.map(item => {
        const isTarget = typeof itemId === 'string' 
            ? item.cartItemId === itemId 
            : item.product.id === itemId;

        if (isTarget) {
          const newQuantity = item.quantity - 1;
          if (newQuantity <= 0) {
            return null; // Remover item
          }
          return {
            ...item,
            quantity: newQuantity,
            totalPrice: item.unitPrice * newQuantity
          };
        }
        return item;
      }).filter(Boolean) as CartItem[]
    );
  }, []);

  // Limpar carrinho
  const clearCart = useCallback(() => {
    setItems([]);
  }, []);

  // Obter quantidade de um produto específico
  const getItemQuantity = useCallback((productId: number) => {
    return items
      .filter(item => item.product.id === productId)
      .reduce((total, item) => total + item.quantity, 0);
  }, [items]);

  const applyCoupon = useCallback((newCoupon: Coupon) => {
    setCoupon(newCoupon);
  }, []);

  const removeCoupon = useCallback(() => {
    setCoupon(null);
  }, []);

  // Cálculos totais
  const totalItems = useMemo(() => {
    return items.reduce((total, item) => total + item.quantity, 0);
  }, [items]);

  const subtotal = useMemo(() => {
    return items.reduce((total, item) => total + item.totalPrice, 0);
  }, [items]);

  const discount = useMemo(() => {
    if (!coupon) return 0;
    
    if (coupon.eDiscountType === EDiscountType.Porcentagem) {
      return (subtotal * coupon.discountValue) / 100;
    } else {
      return Math.min(coupon.discountValue, subtotal);
    }
  }, [subtotal, coupon]);

  const totalPrice = useMemo(() => {
    return Math.max(0, subtotal - discount);
  }, [subtotal, discount]);

  const value = {
    items,
    totalItems,
    subtotal,
    discount,
    totalPrice,
    coupon,
    addToCart,
    addProductSelection,
    removeFromCart,
    updateQuantity,
    increaseQuantity,
    decreaseQuantity,
    clearCart,
    getItemQuantity,
    applyCoupon,
    removeCoupon,
  };

  return (
    <CartContext.Provider value={value}>
      {children}
    </CartContext.Provider>
  );
};

export const useCart = () => {
  const context = useContext(CartContext);
  if (context === undefined) {
    throw new Error('useCart must be used within a CartProvider');
  }
  return context;
};
