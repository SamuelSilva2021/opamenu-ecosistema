import { useDeviceType } from './useDeviceType';
import { getDeviceTokens } from '@/design-system/tokens';

/**
 * Hook para acessar design tokens responsivos baseado no dispositivo atual
 * Facilita o uso de estilos adaptativos em componentes
 */
export const useResponsiveTokens = () => {
  const deviceType = useDeviceType();
  return getDeviceTokens(deviceType);
};

/**
 * Hook para aplicar classes CSS condicionalmente baseado no dispositivo
 */
export const useResponsiveClasses = (classes: {
  mobile?: string;
  tablet?: string;
  desktop?: string;
  base?: string; // Classes aplicadas em todos os dispositivos
}) => {
  const deviceType = useDeviceType();
  const tokens = useResponsiveTokens();
  
  const baseClasses = classes.base || '';
  
  switch (deviceType) {
    case 'mobile':
      return `${baseClasses} ${classes.mobile || ''}`.trim();
    case 'tablet':
      return `${baseClasses} ${classes.tablet || classes.mobile || ''}`.trim();
    case 'desktop':
      return `${baseClasses} ${classes.desktop || ''}`.trim();
    default:
      return baseClasses;
  }
};

/**
 * Hook para estilos condicionais mais complexos
 */
export const useResponsiveStyles = () => {
  const deviceType = useDeviceType();
  const tokens = useResponsiveTokens();
  
  return {
    deviceType,
    tokens,
    
    // Helpers para tipos comuns de estilo
    spacing: tokens.spacing,
    typography: tokens.typography,
    sizes: tokens.sizes,
    layout: tokens.layout,
    animations: tokens.animations,
    
    // Helper para componentes de botão
    getButtonClasses: (variant: 'primary' | 'secondary' | 'outline' = 'primary', size: 'sm' | 'default' | 'lg' = 'default') => {
      const sizeClasses = tokens.sizes.button[size];
      const baseClasses = `${sizeClasses} ${tokens.typography.button} ${tokens.animations.transition}`;
      
      const variantClasses = {
        primary: 'bg-opamenu-orange hover:bg-opamenu-orange/90 text-white',
        secondary: 'bg-muted hover:bg-muted/80 text-foreground',
        outline: 'border border-input hover:bg-accent hover:text-accent-foreground',
      };
      
      return `${baseClasses} ${variantClasses[variant]} ${tokens.animations.scale}`;
    },
    
    // Helper para componentes de card
    getCardClasses: (variant: 'compact' | 'default' | 'expanded' = 'default') => {
      const sizeClasses = tokens.sizes.card[variant];
      return `${sizeClasses} ${tokens.spacing.card} border rounded-lg bg-card text-card-foreground shadow-sm ${tokens.animations.transition} ${tokens.animations.hover}`;
    },
    
    // Helper para modal
    getModalClasses: () => {
      const modalSize = tokens.sizes.modal;
      return `${modalSize.width} ${modalSize.height} ${modalSize.borderRadius} ${tokens.spacing.modal} ${tokens.animations.modal}`;
    },
    
    // Helper para grid responsivo
    getGridClasses: (type: 'products' | 'categories' = 'products') => {
      if (type === 'products') {
        return tokens.layout.productGrid;
      }
      return 'grid grid-cols-2 gap-4'; // Fallback para categorias
    },
    
    // Verificações de dispositivo
    isMobile: deviceType === 'mobile',
    isTablet: deviceType === 'tablet', 
    isDesktop: deviceType === 'desktop',
    isTouchDevice: deviceType === 'mobile' || deviceType === 'tablet',
  };
};
