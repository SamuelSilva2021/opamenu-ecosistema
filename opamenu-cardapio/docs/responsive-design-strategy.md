# Estratégia de Design Responsivo - Pedeja Cardápio

## 🎯 Contexto e Objetivos

### Mobile-First Strategy
- **Prioridade**: 85% dos usuários em dispositivos móveis
- **Contexto de uso**: Clientes em mesas de restaurante
- **Necessidades**: Navegação rápida, seleção intuitiva, checkout ágil

### Desktop Secondary
- **Uso**: Staff administrativo, clientes em casa/escritório
- **Necessidades**: Visão ampla do cardápio, gestão de pedidos grandes

## 🏗️ Arquitetura de Componentes Adaptativos

### 1. Sistema de Breakpoints Semânticos

```typescript
// src/hooks/useDeviceType.ts
export enum DeviceType {
  Mobile = 'mobile',     // 320px - 767px
  Tablet = 'tablet',     // 768px - 1023px  
  Desktop = 'desktop'    // 1024px+
}

export const BREAKPOINTS = {
  mobile: '(max-width: 767px)',
  tablet: '(min-width: 768px) and (max-width: 1023px)',
  desktop: '(min-width: 1024px)',
} as const;
```

### 2. Hook de Detecção de Dispositivo

```typescript
// src/hooks/useDeviceType.ts
import { useState, useEffect } from 'react';

export const useDeviceType = (): DeviceType => {
  const [deviceType, setDeviceType] = useState<DeviceType>(DeviceType.Mobile);

  useEffect(() => {
    const updateDeviceType = () => {
      if (window.matchMedia(BREAKPOINTS.desktop).matches) {
        setDeviceType(DeviceType.Desktop);
      } else if (window.matchMedia(BREAKPOINTS.tablet).matches) {
        setDeviceType(DeviceType.Tablet);
      } else {
        setDeviceType(DeviceType.Mobile);
      }
    };

    // Detectar mudanças
    const mediaQueries = Object.values(BREAKPOINTS).map(query => 
      window.matchMedia(query)
    );
    
    mediaQueries.forEach(mq => mq.addEventListener('change', updateDeviceType));
    updateDeviceType(); // Inicial
    
    return () => {
      mediaQueries.forEach(mq => mq.removeEventListener('change', updateDeviceType));
    };
  }, []);

  return deviceType;
};
```

### 3. Sistema de Variantes de Componentes

```typescript
// src/components/adaptive/AdaptiveComponent.tsx
interface AdaptiveComponentProps<T = any> {
  mobile?: React.ComponentType<T>;
  tablet?: React.ComponentType<T>;
  desktop?: React.ComponentType<T>;
  fallback?: React.ComponentType<T>;
  props?: T;
}

export const AdaptiveComponent = <T,>({
  mobile,
  tablet,
  desktop,
  fallback,
  props,
}: AdaptiveComponentProps<T>) => {
  const deviceType = useDeviceType();
  
  let Component: React.ComponentType<T> | undefined;
  
  switch (deviceType) {
    case DeviceType.Mobile:
      Component = mobile || fallback;
      break;
    case DeviceType.Tablet:
      Component = tablet || mobile || fallback;
      break;
    case DeviceType.Desktop:
      Component = desktop || fallback;
      break;
  }
  
  if (!Component) return null;
  
  return <Component {...(props as T)} />;
};
```

## 📱 Implementação para ProductModal

### 1. Estrutura de Componentes Específicos

```
src/components/ProductModal/
├── ProductModal.tsx              # Orchestrator principal
├── mobile/
│   ├── MobileProductModal.tsx    # Layout otimizado mobile
│   ├── MobileProductCard.tsx     # Card de produto mobile
│   └── MobileAddonSelector.tsx   # Seletor de adicionais mobile
├── desktop/
│   ├── DesktopProductModal.tsx   # Layout grid desktop
│   ├── DesktopProductDetails.tsx # Detalhes expandidos
│   └── DesktopAddonPanel.tsx     # Panel lateral adicionais
└── shared/
    ├── ProductImage.tsx          # Componente de imagem adaptativo
    ├── PriceDisplay.tsx          # Exibição de preços
    └── AddonBadges.tsx          # Badges de adicionais
```

### 2. Design Patterns por Dispositivo

#### Mobile Pattern - Stack Vertical
```typescript
// Características:
- Single column layout
- Bottom sheet behavior para adicionais
- Sticky footer com CTA
- Gesture-based navigation
- Simplified addon selection
- Large touch targets (min 44px)
```

#### Desktop Pattern - Sidebar/Grid
```typescript
// Características:
- Two-column grid layout
- Hover states e micro-interactions
- Detailed addon descriptions
- Bulk selection capabilities
- Keyboard navigation support
- Dense information display
```

## 🎨 Design System Responsivo

### 1. Tokens de Design Adaptativos

```typescript
// src/design-system/tokens.ts
export const SPACING = {
  mobile: {
    container: 'p-4',
    section: 'space-y-4',
    item: 'p-3',
  },
  desktop: {
    container: 'p-6',
    section: 'space-y-6', 
    item: 'p-4',
  }
} as const;

export const TYPOGRAPHY = {
  mobile: {
    title: 'text-lg font-semibold',
    body: 'text-sm',
    caption: 'text-xs',
  },
  desktop: {
    title: 'text-xl font-semibold',
    body: 'text-base',
    caption: 'text-sm',
  }
} as const;
```

### 2. Component Size Variants

```typescript
// src/components/ui/Button/Button.tsx
const buttonVariants = cva(
  "inline-flex items-center justify-center rounded-md font-medium transition-colors",
  {
    variants: {
      size: {
        // Mobile optimized
        'mobile-sm': 'h-9 px-3 text-xs',
        'mobile-default': 'h-11 px-4 text-sm',
        'mobile-lg': 'h-12 px-6 text-base',
        
        // Desktop optimized  
        'desktop-sm': 'h-8 px-3 text-xs',
        'desktop-default': 'h-9 px-4 text-sm',
        'desktop-lg': 'h-10 px-6 text-base',
      }
    }
  }
);
```

## 🔄 Estratégias de Carregamento

### 1. Code Splitting por Dispositivo

```typescript
// src/components/ProductModal/index.ts
import { lazy } from 'react';

// Lazy load baseado no dispositivo
export const MobileProductModal = lazy(() => 
  import('./mobile/MobileProductModal')
);

export const DesktopProductModal = lazy(() => 
  import('./desktop/DesktopProductModal')
);

// HOC para carregamento condicional
export const ProductModal = (props: ProductModalProps) => {
  const deviceType = useDeviceType();
  
  return (
    <Suspense fallback={<ProductModalSkeleton />}>
      <AdaptiveComponent
        mobile={MobileProductModal}
        desktop={DesktopProductModal}
        props={props}
      />
    </Suspense>
  );
};
```

### 2. Asset Optimization

```typescript
// src/utils/image-optimization.ts
export const getOptimizedImageUrl = (
  url: string, 
  deviceType: DeviceType
): string => {
  const sizes = {
    [DeviceType.Mobile]: { width: 400, quality: 75 },
    [DeviceType.Tablet]: { width: 600, quality: 80 },
    [DeviceType.Desktop]: { width: 800, quality: 85 },
  };
  
  const { width, quality } = sizes[deviceType];
  return `${url}?w=${width}&q=${quality}&f=webp`;
};
```

## 📊 Performance Strategy

### 1. Bundle Splitting
- **Mobile bundle**: Core components + mobile variants
- **Desktop bundle**: Desktop-specific components (lazy loaded)
- **Shared bundle**: Common utilities e business logic

### 2. Resource Hints
```html
<!-- Para mobile -->
<link rel="preload" href="/mobile-styles.css" as="style" media="(max-width: 767px)">

<!-- Para desktop -->
<link rel="preload" href="/desktop-styles.css" as="style" media="(min-width: 1024px)">
```

## 🧪 Testing Strategy

### 1. Device-Specific Tests
```typescript
// __tests__/ProductModal.mobile.test.tsx
describe('ProductModal - Mobile', () => {
  beforeEach(() => {
    mockMatchMedia('(max-width: 767px)');
  });
  
  it('should render mobile layout with touch-optimized controls', () => {
    // Mobile-specific tests
  });
});

// __tests__/ProductModal.desktop.test.tsx  
describe('ProductModal - Desktop', () => {
  beforeEach(() => {
    mockMatchMedia('(min-width: 1024px)');
  });
  
  it('should render desktop layout with grid system', () => {
    // Desktop-specific tests
  });
});
```

### 2. Visual Regression Testing
```typescript
// .storybook/stories/ProductModal.stories.tsx
export default {
  title: 'Components/ProductModal',
  component: ProductModal,
  parameters: {
    viewport: {
      viewports: {
        mobile: { name: 'Mobile', styles: { width: '375px', height: '667px' } },
        desktop: { name: 'Desktop', styles: { width: '1440px', height: '900px' } },
      },
    },
  },
};

export const Mobile = {
  parameters: { viewport: { defaultViewport: 'mobile' } },
};

export const Desktop = {
  parameters: { viewport: { defaultViewport: 'desktop' } },
};
```

## 🎯 Implementation Roadmap

### Phase 1: Foundation (1 semana)
- [ ] Setup device detection hook
- [ ] Create adaptive component system
- [ ] Implement design tokens
- [ ] Setup bundle splitting

### Phase 2: Mobile Optimization (1 semana)  
- [ ] Refactor ProductModal para mobile-first
- [ ] Implement touch-optimized interactions
- [ ] Add gesture support
- [ ] Optimize loading performance

### Phase 3: Desktop Enhancement (1 semana)
- [ ] Create desktop-specific layouts
- [ ] Add advanced interactions (hover, keyboard)
- [ ] Implement bulk operations
- [ ] Add detailed views

### Phase 4: Polish & Testing (1 semana)
- [ ] Cross-device testing
- [ ] Performance optimization
- [ ] Accessibility improvements
- [ ] Documentation final

## 📈 Success Metrics

### Mobile KPIs
- Time to first interaction < 1.5s
- Touch target success rate > 95%
- Scroll performance 60fps
- Cart completion rate > 80%

### Desktop KPIs  
- Information density 3x maior que mobile
- Hover feedback latency < 100ms
- Keyboard navigation support 100%
- Multi-selection efficiency 2x melhor

## 🔧 Tools & Libraries

### Development
- **React Query**: Para cache de dados por dispositivo
- **Framer Motion**: Animações responsivas
- **React Hook Form**: Formulários adaptativos
- **Storybook**: Desenvolvimento isolado por dispositivo

### Build & Deploy
- **Vite**: Bundle splitting automático
- **Lighthouse CI**: Performance monitoring
- **Chromatic**: Visual regression testing
- **Sentry**: Error monitoring por dispositivo

---

Esta estratégia garante uma experiência otimizada para cada contexto de uso, mantendo o código maintível e performático.
