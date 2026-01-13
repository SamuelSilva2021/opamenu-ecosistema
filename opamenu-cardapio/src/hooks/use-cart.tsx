import { useState, useCallback, useEffect, useMemo, createContext, useContext, ReactNode } from 'react';
import { CartItem } from '@/types/cart';
import { Product, ProductSelection, Coupon } from '@/types/api';

export interface CartContextType {
  items: CartItem[];
  totalItems: number;
  subtotal: number;
  discount: number;
  totalPrice: number;
  coupon: Coupon | null;
  addToCart: (product: Product, quantity?: number) => void;
  addProductSelection: (selection: ProductSelection) => void;
  removeFromCart: (productId: number) => void;
  updateQuantity: (productId: number, quantity: number) => void;
  increaseQuantity: (productId: number) => void;
  decreaseQuantity: (productId: number) => void;
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
        setItems(parsedCart);
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

  // Adicionar produto simples ao carrinho (sem adicionais)
  const addToCart = useCallback((product: Product, quantity: number = 1) => {
    if (quantity <= 0) return;

    const unitPrice = product.price;
    const totalPrice = unitPrice * quantity;

    setItems(currentItems => {
      const existingItemIndex = currentItems.findIndex(item => 
        item.product.id === product.id && !item.selectedAddons?.length
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
      // Para produtos com adicionais, sempre criar um novo item
      // (porque diferentes combinações de adicionais são itens separados)
      const newItem: CartItem = {
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
  const removeFromCart = useCallback((productId: number) => {
    setItems(currentItems => 
      currentItems.filter(item => item.product.id !== productId)
    );
  }, []);

  // Atualizar quantidade
  const updateQuantity = useCallback((productId: number, quantity: number) => {
    if (quantity <= 0) {
      removeFromCart(productId);
      return;
    }

    setItems(currentItems =>
      currentItems.map(item =>
        item.product.id === productId
          ? { 
              ...item, 
              quantity,
              totalPrice: item.unitPrice * quantity
            }
          : item
      )
    );
  }, [removeFromCart]);

  // Aumentar quantidade
  const increaseQuantity = useCallback((productId: number) => {
    setItems(currentItems =>
      currentItems.map(item =>
        item.product.id === productId
          ? { 
              ...item, 
              quantity: item.quantity + 1,
              totalPrice: item.unitPrice * (item.quantity + 1)
            }
          : item
      )
    );
  }, []);

  // Diminuir quantidade
  const decreaseQuantity = useCallback((productId: number) => {
    setItems(currentItems =>
      currentItems.map(item => {
        if (item.product.id === productId) {
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

  // Obter quantidade total de um produto (somando todos os itens com o mesmo produto)
  const getItemQuantity = useCallback((productId: number): number => {
    return items
      .filter(item => item.product.id === productId)
      .reduce((total, item) => total + item.quantity, 0);
  }, [items]);

  // Aplicar cupom
  const applyCoupon = useCallback((newCoupon: Coupon) => {
    setCoupon(newCoupon);
  }, []);

  // Remover cupom
  const removeCoupon = useCallback(() => {
    setCoupon(null);
  }, []);

  // Cálculos de totais
  const { totalItems, subtotal, discount, totalPrice } = useMemo(() => {
    const totalItems = items.reduce((sum, item) => sum + item.quantity, 0);
    const subtotal = items.reduce((sum, item) => sum + item.totalPrice, 0);
    
    let discount = 0;
    if (coupon) {
      // Validar valor mínimo do pedido
      if (coupon.minOrderValue && subtotal < coupon.minOrderValue) {
        // Se o valor do pedido cair abaixo do mínimo, o desconto é 0 (ou poderíamos remover o cupom)
        // Por enquanto, vamos manter o cupom mas sem aplicar desconto efetivo
        discount = 0;
      } else {
        if (coupon.discountType === 1) { // Porcentagem
          discount = subtotal * (coupon.discountValue / 100);
          if (coupon.maxDiscountValue && discount > coupon.maxDiscountValue) {
            discount = coupon.maxDiscountValue;
          }
        } else if (coupon.discountType === 2) { // Valor fixo
          discount = coupon.discountValue;
        }
      }
    }

    // Garantir que desconto não seja maior que subtotal
    if (discount > subtotal) {
      discount = subtotal;
    }

    const totalPrice = Math.max(0, subtotal - discount);

    return { totalItems, subtotal, discount, totalPrice };
  }, [items, coupon]);

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
    removeCoupon
  };

  return (
    <CartContext.Provider value={value}>
      {children}
    </CartContext.Provider>
  );
};

// Hook para consumir o contexto do carrinho
export const useCart = (slug?: string): CartContextType => {
  const context = useContext(CartContext);
  
  if (context === undefined) {
    // Se não estiver dentro do Provider, retorna um erro ou um fallback
    // Para manter compatibilidade durante a migração, você pode retornar a lógica original aqui se slug for fornecido,
    // mas o ideal é garantir que o Provider esteja sempre presente.
    throw new Error('useCart must be used within a CartProvider');
  }
  
  return context;
};
