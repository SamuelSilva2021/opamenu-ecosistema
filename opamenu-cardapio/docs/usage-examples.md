# Exemplo de Uso - Sistema Responsivo Pedeja Cardápio

## 🎯 Demonstração Prática

Aqui estão exemplos reais de como usar o sistema de design responsivo implementado:

### 1. Componente de Produto com Variantes

```tsx
// src/components/ProductCard/index.tsx
import { AdaptiveComponent } from '@/components/adaptive/AdaptiveComponent';
import { useResponsiveStyles } from '@/hooks/useResponsiveTokens';
import { Product } from '@/types/api';

// Variants específicas por dispositivo
const MobileProductCard = ({ product }: { product: Product }) => {
  const responsive = useResponsiveStyles();
  
  return (
    <div className={responsive.getCardClasses('compact')}>
      {/* Layout vertical para mobile */}
      <div className="aspect-[4/3] rounded-lg overflow-hidden mb-2">
        <img src={product.imageUrl} className="w-full h-full object-cover" />
      </div>
      <h3 className={responsive.typography.h4}>{product.name}</h3>
      <p className={responsive.typography.priceSmall}>{formatPrice(product.price)}</p>
      <Button className={`${responsive.getButtonClasses('primary', 'default')} w-full mt-2`}>
        Ver Detalhes
      </Button>
    </div>
  );
};

const DesktopProductCard = ({ product }: { product: Product }) => {
  const responsive = useResponsiveStyles();
  
  return (
    <div className={`${responsive.getCardClasses('default')} group`}>
      {/* Layout horizontal para desktop */}
      <div className="flex gap-4">
        <div className="w-24 h-24 rounded-lg overflow-hidden shrink-0">
          <img src={product.imageUrl} className="w-full h-full object-cover" />
        </div>
        <div className="flex-1">
          <h3 className={responsive.typography.h3}>{product.name}</h3>
          <p className={responsive.typography.body}>{product.description}</p>
          <div className="flex items-center justify-between mt-2">
            <p className={responsive.typography.price}>{formatPrice(product.price)}</p>
            <Button className={responsive.getButtonClasses('primary', 'sm')}>
              Ver Detalhes
            </Button>
          </div>
        </div>
      </div>
    </div>
  );
};

// Componente principal que escolhe a variant
export const ProductCard = (props: { product: Product }) => (
  <AdaptiveComponent
    mobile={MobileProductCard}
    desktop={DesktopProductCard}
    fallback={MobileProductCard}
    props={props}
  />
);
```

### 2. Grid Responsivo de Produtos

```tsx
// src/components/ProductGrid.tsx
import { useResponsiveStyles } from '@/hooks/useResponsiveTokens';
import { MobileOnly, DesktopOnly } from '@/components/adaptive/AdaptiveComponent';

export const ProductGrid = ({ products }: { products: Product[] }) => {
  const responsive = useResponsiveStyles();
  
  return (
    <>
      {/* Grid mobile: 1 coluna, cards compactos */}
      <MobileOnly>
        <div className="grid grid-cols-1 gap-3">
          {products.map(product => (
            <ProductCard key={product.id} product={product} />
          ))}
        </div>
      </MobileOnly>

      {/* Grid desktop: 3-4 colunas, cards expandidos */}
      <DesktopOnly>
        <div className={responsive.getGridClasses('products')}>
          {products.map(product => (
            <ProductCard key={product.id} product={product} />
          ))}
        </div>
      </DesktopOnly>
    </>
  );
};
```

### 3. Sistema de Navegação Adaptativo

```tsx
// src/components/Navigation/index.tsx
import { useResponsiveStyles } from '@/hooks/useResponsiveTokens';
import { MobileAndTablet, DesktopOnly } from '@/components/adaptive/AdaptiveComponent';

export const Navigation = () => {
  const responsive = useResponsiveStyles();
  
  return (
    <>
      {/* Mobile: Bottom navigation */}
      <MobileAndTablet>
        <nav className="fixed bottom-0 left-0 right-0 bg-background border-t z-50">
          <div className={`${responsive.spacing.container} py-2`}>
            <div className="flex justify-around">
              <NavItem icon="🏠" label="Início" />
              <NavItem icon="📋" label="Cardápio" />
              <NavItem icon="🛒" label="Carrinho" />
              <NavItem icon="👤" label="Perfil" />
            </div>
          </div>
        </nav>
      </MobileAndTablet>

      {/* Desktop: Header navigation */}
      <DesktopOnly>
        <nav className="sticky top-0 bg-background border-b z-50">
          <div className={responsive.spacing.container}>
            <div className="flex items-center justify-between h-16">
              <div className="flex items-center gap-8">
                <Logo />
                <NavLinks />
              </div>
              <div className="flex items-center gap-4">
                <SearchBar />
                <CartButton />
                <UserMenu />
              </div>
            </div>
          </div>
        </nav>
      </DesktopOnly>
    </>
  );
};
```

### 4. Modal de Checkout Responsivo

```tsx
// src/components/CheckoutModal.tsx
import { useResponsiveStyles } from '@/hooks/useResponsiveTokens';

export const CheckoutModal = ({ isOpen, onClose }: CheckoutModalProps) => {
  const responsive = useResponsiveStyles();
  
  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className={responsive.getModalClasses()}>
        <DialogHeader>
          <DialogTitle className={responsive.typography.h2}>
            Finalizar Pedido
          </DialogTitle>
        </DialogHeader>
        
        <div className="flex-1">
          {responsive.isTouchDevice ? (
            // Mobile: Wizard steps
            <CheckoutWizard />
          ) : (
            // Desktop: Side-by-side layout
            <div className="grid grid-cols-2 gap-6">
              <OrderSummary />
              <PaymentForm />
            </div>
          )}
        </div>
        
        <div className={responsive.layout.footer}>
          <div className="flex gap-3">
            <Button 
              variant="outline"
              className={responsive.getButtonClasses('outline', 'default')}
              onClick={onClose}
            >
              Cancelar
            </Button>
            <Button 
              className={`${responsive.getButtonClasses('primary', 'default')} ${responsive.isTouchDevice ? 'flex-1' : ''}`}
            >
              Confirmar Pedido
            </Button>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
};
```

### 5. Hook Personalizado para Formulários

```tsx
// src/hooks/useResponsiveForm.ts
import { useResponsiveStyles } from '@/hooks/useResponsiveTokens';

export const useResponsiveForm = () => {
  const responsive = useResponsiveStyles();
  
  return {
    // Classes para inputs
    getInputClasses: (variant: 'default' | 'error' = 'default') => {
      const baseClasses = responsive.sizes.input.default;
      const variantClasses = variant === 'error' 
        ? 'border-destructive focus:border-destructive' 
        : 'border-input focus:border-primary';
      
      return `${baseClasses} ${variantClasses} ${responsive.animations.transition}`;
    },
    
    // Layout de formulário
    getFormLayout: () => responsive.isTouchDevice 
      ? 'space-y-4' // Mobile: stack vertical
      : 'grid grid-cols-2 gap-4', // Desktop: grid
    
    // Botões de formulário
    getSubmitButtonClasses: () => 
      `${responsive.getButtonClasses('primary', 'lg')} ${responsive.isTouchDevice ? 'w-full' : ''}`,
  };
};
```

## 🎨 Benefícios do Sistema

### Performance
- **Bundle splitting automático**: Componentes desktop só carregam quando necessário
- **Otimização de imagens**: URLs diferentes por dispositivo
- **Animations inteligentes**: Mais suaves no mobile, mais ricas no desktop

### Manutenibilidade  
- **Single source of truth**: Design tokens centralizados
- **Type safety**: TypeScript em todos os níveis
- **Reusabilidade**: Componentes adaptativos reutilizáveis

### UX Otimizada
- **Touch-first mobile**: Área de toque adequada (44px mínimo)
- **Hover-rich desktop**: Micro-interações e feedback visual
- **Responsive breakpoints**: Transições suaves entre dispositivos

## 🔧 Próximos Passos

### 1. Implementar em Todos os Componentes
```bash
# Refatorar componentes existentes
- ProductCard ✅
- ProductModal ✅ 
- Navigation ⏳
- Cart ⏳
- Checkout ⏳
```

### 2. Testes de Responsividade
```bash
# Testar em diferentes dispositivos
npm run test:mobile
npm run test:tablet  
npm run test:desktop
npm run test:visual-regression
```

### 3. Monitoramento de Performance
```bash
# Métricas por dispositivo
- Bundle size por breakpoint
- Time to interactive
- First contentful paint
- Core web vitals
```

Este sistema garante uma experiência otimizada e maintível para todos os dispositivos!
