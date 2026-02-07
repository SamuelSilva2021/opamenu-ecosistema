import { useState, useMemo } from "react";
import { useParams } from "react-router-dom";
import TenantHeader from "@/components/TenantHeader";
import ProductCard from "@/components/ProductCard";
import ProductModal from "@/components/ProductModal";
import ShoppingCart from "@/components/ShoppingCart";
import CheckoutPage from "@/pages/CheckoutPage";
import LoyaltyCard from "@/components/LoyaltyCard";
import DeliveryInfo from "@/components/DeliveryInfo";
import InlineCart from "@/components/InlineCart";
import BottomNavigation from "@/components/BottomNavigation";
import { couponService } from "@/services/coupon-service";
import { useCart, useProductModal, useStorefront, useCustomer } from "@/hooks";
import { Product } from "@/types/api";
import { CartItem } from "@/types/cart";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Search, ShoppingCart as ShoppingCartIcon, Loader2, ChevronDown } from "lucide-react";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { CouponBanner } from "@/components/CouponBanner";
import { LoginModal } from "@/components/auth/LoginModal";
import { ProfileModal } from "@/components/auth/ProfileModal";
import { OrdersModal } from "@/components/OrdersModal";
import { CustomerProvider } from "@/hooks/use-customer";
import { CartProvider } from "@/hooks/use-cart";
import NotFound from "@/pages/NotFound";

const StorefrontContent = () => {
  const { slug } = useParams<{ slug: string }>();
  const [searchQuery, setSearchQuery] = useState("");
  const [selectedCategory, setSelectedCategory] = useState("all");
  const [isCartOpen, setIsCartOpen] = useState(false);
  const [isCheckoutMode, setIsCheckoutMode] = useState(false);
  const [isLoginModalOpen, setIsLoginModalOpen] = useState(false);
  const [isProfileModalOpen, setIsProfileModalOpen] = useState(false);
  const [isOrdersModalOpen, setIsOrdersModalOpen] = useState(false);
  const [editingItem, setEditingItem] = useState<CartItem | null>(null);

  // Usar hooks customizados para dados da API
  const {
    tenantBusiness,
    products,
    categories,
    coupons,
    loading: storefrontLoading,
    error: storefrontError
  } = useStorefront(slug);

  const { customer, logout } = useCustomer();

  const {
    items: cartItems,
    totalItems: totalCartItems,
    subtotal: cartSubtotal,
    discount: cartDiscount,
    totalPrice: cartTotal,
    coupon: activeCoupon,
    addToCart,
    addProductSelection,
    removeFromCart,
    updateQuantity,
    clearCart,
    applyCoupon,
    removeCoupon,
    editCartItem
  } = useCart();

  // Hook para gerenciar modal de produto
  const {
    isOpen: isModalOpen,
    selectedProduct,
    openModal: openProductModal,
    closeModal: closeProductModal,
    error: modalError
  } = useProductModal(slug);

  // Show toast on modal error
  if (modalError) {
    console.error("Product Modal Error:", modalError);
  }

  // Use unified loading/error states
  const tenantLoading = storefrontLoading;
  const productsLoading = storefrontLoading;
  const categoriesLoading = storefrontLoading;
  const productsError = storefrontError;

  const filteredProducts = useMemo(() => {
    if (!Array.isArray(products)) {
      console.warn('⚠️ Products is not an array:', products);
      return [];
    }

    return products.filter((product) => {
      const matchesSearch = product.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
        (product.description?.toLowerCase().includes(searchQuery.toLowerCase()) || false);
      const matchesCategory = selectedCategory === "all" || product.categoryId?.toString() === selectedCategory;
      return matchesSearch && matchesCategory && product.isActive;
    });
  }, [products, searchQuery, selectedCategory]);

  const productsWithCart = filteredProducts.map((product) => {
    const cartItem = cartItems.find((item) => item.product.id === product.id);
    return {
      ...product,
      cartQuantity: cartItem?.quantity || 0,
    };
  });

  const categoryOptions = useMemo(() => {
    if (!Array.isArray(categories) || !Array.isArray(products)) {
      return [];
    }

    const activeCategories = categories.filter(cat => cat.isActive);
    return activeCategories.map(cat => ({
      id: cat.id.toString(),
      name: cat.name,
      count: products.filter(p => p.categoryId === cat.id && p.isActive).length
    }));
  }, [categories, products]);

  const handleRemoveFromCart = (productId: string) => {
    const cartItem = cartItems.find(item => item.product.id === productId);
    if (cartItem && cartItem.quantity > 1) {
      updateQuantity(productId, cartItem.quantity - 1);
    } else {
      removeFromCart(productId);
    }
  };

  const handleValidateCoupon = async (code: string): Promise<boolean> => {
    if (!slug) return false;
    try {
      const coupon = await couponService.validateCoupon(slug, code, cartSubtotal);
      if (coupon) {
        applyCoupon(coupon);
        return true;
      }
      return false;
    } catch (error) {
      console.error("Invalid coupon:", error);
      return false;
    }
  };

  // Se houver erro crítico OU (não estiver carregando E não tiver tenant), mostra 404
  if (productsError === 'COMMON_INTERNAL_SERVER_ERROR' || (!tenantLoading && !tenantBusiness)) {
    console.log('Rendering NotFound State', { productsError, tenantLoading, tenantBusiness });
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-100">
        <div className="text-center p-8">
          <h1 className="text-4xl font-bold mb-4 text-gray-800">Loja não encontrada</h1>
          <p className="text-xl text-gray-600 mb-6">Não foi possível encontrar o estabelecimento solicitado.</p>
        </div>
      </div>
    );
  }

  // Se houver outro erro de carregamento (ex: rede), mostra erro genérico com retry
  if (productsError) {
    return (
      <div className="min-h-screen flex flex-col items-center justify-center bg-gray-50">
        <div className="text-center p-8">
          <h2 className="text-2xl font-bold text-gray-800 mb-4">Falha ao carregar dados</h2>
          <p className="text-gray-600 mb-6">{productsError}</p>
          <Button onClick={() => window.location.reload()}>
            Tentar novamente
          </Button>
        </div>
      </div>
    );
  }

  if (isCheckoutMode) {
    return (
      <CheckoutPage
        onBackToMenu={() => setIsCheckoutMode(false)}
        tenant={tenantBusiness || undefined}
      />
    );
  }

  return (
    <div className="min-h-screen bg-gray-50 pb-20">
      {/* 1. Tenant Header */}
      {tenantLoading ? (
        <div className="h-64 flex items-center justify-center bg-white">
          <Loader2 className="h-8 w-8 animate-spin text-primary" />
        </div>
      ) : tenantBusiness ? (
        <TenantHeader
          tenant={tenantBusiness}
          customer={customer}
          onLoginClick={() => setIsLoginModalOpen(true)}
          onLogoutClick={logout}
          onEditProfileClick={() => setIsProfileModalOpen(true)}
          onOrdersClick={() => {
            if (!customer) {
              setIsLoginModalOpen(true);
            } else {
              setIsOrdersModalOpen(true);
            }
          }}
        />
      ) : (
        <div className="h-64 flex items-center justify-center bg-white text-gray-500">
          Informações do estabelecimento indisponíveis.
        </div>
      )}

      {/* Coupons Section */}
      <div id="coupon-banner">
        <CouponBanner coupons={coupons || []} />
      </div>

      {/* 2. Main Layout Grid */}
      <div className="container mx-auto px-4 py-8 grid grid-cols-1 lg:grid-cols-3 gap-8">

        {/* LEFT COLUMN - Main Content */}
        <div className="lg:col-span-2 space-y-6">

          {/* Mobile Loyalty Card */}
          <div className="lg:hidden">
            {tenantBusiness?.loyaltyProgram?.isActive && (
              <LoyaltyCard program={tenantBusiness.loyaltyProgram} />
            )}
          </div>

          <div className="hidden lg:flex gap-4">
            <DropdownMenu>
              <DropdownMenuTrigger asChild>
                <Button variant="outline" className="shrink-0 gap-2 h-12 bg-white border-gray-200">
                  Lista de categorias
                  <ChevronDown className="h-4 w-4 text-gray-500" />
                </Button>
              </DropdownMenuTrigger>
              <DropdownMenuContent align="start" className="w-56 bg-white z-50 shadow-lg border border-gray-200">
                <DropdownMenuItem onClick={() => setSelectedCategory("all")}>
                  Todas as categorias
                </DropdownMenuItem>
                {categoryOptions.map(cat => (
                  <DropdownMenuItem key={cat.id} onClick={() => setSelectedCategory(cat.id)}>
                    {cat.name} ({cat.count})
                  </DropdownMenuItem>
                ))}
              </DropdownMenuContent>
            </DropdownMenu>

            <div className="relative flex-1">
              <Search className="absolute left-4 top-1/2 transform -translate-y-1/2 text-gray-400 h-5 w-5" />
              <Input
                type="text"
                placeholder="Busque por um produto..."
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                className="pl-12 pr-12 h-12 bg-white border-gray-200 focus:border-primary focus:ring-primary/20 transition-all text-base"
              />
              {searchQuery && (
                <button
                  onClick={() => setSearchQuery("")}
                  className="absolute right-4 top-1/2 transform -translate-y-1/2 text-gray-400 hover:text-gray-600 transition-colors"
                >
                  <span className="sr-only">Limpar busca</span>
                  <svg className="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                  </svg>
                </button>
              )}
            </div>
          </div>

          {/* Mobile Category Navigation */}
          <div className="lg:hidden">
            <div className="flex overflow-x-auto gap-3 pb-2 -mx-4 px-4 scrollbar-hide">
              <button
                onClick={() => setSelectedCategory("all")}
                className={`
                  whitespace-nowrap px-4 py-2 rounded-full text-sm font-medium transition-colors
                  ${selectedCategory === "all"
                    ? "bg-primary text-primary-foreground"
                    : "bg-white text-gray-600 border border-gray-200"}
                `}
              >
                Destaques
              </button>
              {categoryOptions.map(cat => (
                <button
                  key={cat.id}
                  onClick={() => setSelectedCategory(cat.id)}
                  className={`
                    whitespace-nowrap px-4 py-2 rounded-full text-sm font-medium transition-colors
                    ${selectedCategory === cat.id
                      ? "bg-primary text-primary-foreground"
                      : "bg-white text-gray-600 border border-gray-200"}
                  `}
                >
                  {cat.name}
                </button>
              ))}
            </div>
          </div>

          {/* Destaques / Product Grid */}
          <div>
            <div className="flex items-center justify-between mb-6">
              <h2 className="text-xl lg:text-2xl font-bold text-gray-800">
                {selectedCategory === "all" ? "Pincipais do Dia" : categoryOptions.find(c => c.id === selectedCategory)?.name}
              </h2>
              <span className="text-sm text-gray-500 font-medium bg-gray-100 px-3 py-1 rounded-full">
                {filteredProducts.length} {filteredProducts.length === 1 ? 'produto' : 'produtos'}
              </span>
            </div>

            {productsLoading ? (
              <div className="flex justify-center py-12">
                <Loader2 className="h-8 w-8 animate-spin text-primary" />
              </div>
            ) : productsError ? (
              <div className="text-center py-12">
                <p className="text-red-500 mb-4">{productsError}</p>
                <Button onClick={() => window.location.reload()}>Tentar novamente</Button>
              </div>
            ) : (
              <div className="grid grid-cols-1 sm:grid-cols-2 gap-6">
                {productsWithCart.length > 0 ? (
                  productsWithCart.map((product) => (
                    <ProductCard
                      key={product.id}
                      product={product}
                      onAddToCart={() => openProductModal(product.id)}
                      onRemoveFromCart={() => handleRemoveFromCart(product.id)}
                      onProductClick={() => openProductModal(product.id)}
                      cartQuantity={product.cartQuantity}
                    />
                  ))
                ) : (
                  <div className="col-span-full text-center py-20 bg-white rounded-2xl border-2 border-dashed border-gray-100">
                    <div className="flex flex-col items-center max-w-xs mx-auto">
                      <div className="bg-gray-50 p-4 rounded-full mb-4">
                        <Search className="h-8 w-8 text-gray-300" />
                      </div>
                      <h3 className="text-lg font-semibold text-gray-800 mb-2">Sem resultados</h3>
                      <p className="text-gray-500 mb-6">Não encontramos nada para "{searchQuery}". Tente outro termo ou limpe os filtros.</p>
                      <Button
                        variant="outline"
                        onClick={() => {
                          setSearchQuery("");
                          setSelectedCategory("all");
                        }}
                        className="w-full"
                      >
                        Limpar filtros
                      </Button>
                    </div>
                  </div>
                )}
              </div>
            )}
          </div>
        </div>

        {/* RIGHT COLUMN - Sidebar (Sticky on Desktop) */}
        <div className="hidden lg:block space-y-6">
          <div className="sticky top-4 space-y-6">
            {tenantBusiness?.loyaltyProgram?.isActive && (
              <LoyaltyCard program={tenantBusiness.loyaltyProgram} />
            )}
            {tenantBusiness && customer && customer.street && (
              <DeliveryInfo tenant={tenantBusiness} customer={customer} />
            )}
            <InlineCart
              cartItems={cartItems}
              total={cartSubtotal}
              onUpdateQuantity={updateQuantity}
              onRemoveItem={removeFromCart}
              onClearCart={clearCart}
              onCheckout={() => setIsCheckoutMode(true)}
              onEditItem={(item) => {
                setEditingItem(item);
                openProductModal(item.product.id);
              }}
            />
          </div>
        </div>

        {/* Mobile Bottom Navigation */}
        <div className="lg:hidden">
          <BottomNavigation
            cartItemCount={totalCartItems}
            cartTotal={cartSubtotal}
            onCartClick={() => setIsCartOpen(true)}
            onProfileClick={() => {
              if (customer) {
                setIsProfileModalOpen(true);
              } else {
                setIsLoginModalOpen(true);
              }
            }}
            customer={customer}
            onLogoutClick={logout}
            onCouponsClick={() => {
              const element = document.getElementById('coupon-banner');
              if (element) {
                element.scrollIntoView({ behavior: 'smooth', block: 'center' });
              }
            }}
            onOrdersClick={() => {
              if (!customer) {
                setIsLoginModalOpen(true);
              } else {
                setIsOrdersModalOpen(true);
              }
            }}
          />
        </div>

      </div>

      {/* Modals */}

      {selectedProduct && (
        <ProductModal
          isOpen={isModalOpen}
          onClose={() => {
            closeProductModal();
            setEditingItem(null);
          }}
          product={selectedProduct}
          initialSelectedAddons={editingItem?.selectedAddons}
          initialQuantity={editingItem?.quantity}
          confirmLabel={editingItem ? "Atualizar" : "Adicionar"}
          onAddToCart={(selection) => {
            if (editingItem && editingItem.cartItemId) {
              editCartItem(editingItem.cartItemId, selection);
              setIsCartOpen(true);
            } else {
              addProductSelection(selection);
            }
            closeProductModal();
            setEditingItem(null);
          }}
        />
      )}

      <ShoppingCart
        isOpen={isCartOpen}
        onClose={() => setIsCartOpen(false)}
        cartItems={cartItems}
        onUpdateQuantity={updateQuantity}
        onRemoveItem={removeFromCart}
        onEditItem={(item) => {
          setEditingItem(item);
          openProductModal(item.product.id);
          setIsCartOpen(false); // Close cart to show modal? Or keep it open? Usually standard to close cart overlap.
        }}
        onCheckout={() => {
          setIsCartOpen(false);
          setIsCheckoutMode(true);
        }}
        subtotal={cartSubtotal}
        discount={cartDiscount}
        totalPrice={cartTotal}
        coupon={activeCoupon}
        onApplyCoupon={applyCoupon}
        onRemoveCoupon={removeCoupon}
        onValidateCoupon={handleValidateCoupon}
      />

      <LoginModal
        isOpen={isLoginModalOpen}
        onClose={() => setIsLoginModalOpen(false)}
      />

      <ProfileModal
        isOpen={isProfileModalOpen}
        onClose={() => setIsProfileModalOpen(false)}
      />

      <OrdersModal
        isOpen={isOrdersModalOpen}
        onClose={() => setIsOrdersModalOpen(false)}
      />
    </div>
  );
};

const Index = () => {
  const { slug } = useParams<{ slug: string }>();
  return (
    <CustomerProvider slug={slug}>
      <CartProvider slug={slug}>
        <StorefrontContent />
      </CartProvider>
    </CustomerProvider>
  );
};

export default Index;
