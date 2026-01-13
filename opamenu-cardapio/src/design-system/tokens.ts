import { DeviceType } from '@/hooks/useDeviceType';

/**
 * Design tokens responsivos para o sistema opamenu
 * Implementa mobile-first strategy com escalabilidade para desktop
 */

export const SPACING = {
  [DeviceType.Mobile]: {
    // Containers e layouts
    container: 'px-4 py-4',
    section: 'space-y-4',
    modal: 'p-4',
    
    // Elementos individuais
    card: 'p-3',
    button: 'px-4 py-2',
    input: 'px-3 py-2',
    
    // Gaps e separações
    gap: {
      xs: 'gap-2',
      sm: 'gap-3', 
      md: 'gap-4',
      lg: 'gap-6',
    },
    
    // Margens específicas
    margin: {
      section: 'mb-4',
      item: 'mb-2',
      group: 'mb-6',
    }
  },
  
  [DeviceType.Tablet]: {
    container: 'px-6 py-5',
    section: 'space-y-5',
    modal: 'p-5',
    
    card: 'p-4',
    button: 'px-5 py-2',
    input: 'px-4 py-2',
    
    gap: {
      xs: 'gap-2',
      sm: 'gap-4',
      md: 'gap-5',
      lg: 'gap-7',
    },
    
    margin: {
      section: 'mb-5',
      item: 'mb-3',
      group: 'mb-7',
    }
  },
  
  [DeviceType.Desktop]: {
    container: 'px-8 py-6',
    section: 'space-y-6',
    modal: 'p-6',
    
    card: 'p-5',
    button: 'px-6 py-2',
    input: 'px-4 py-3',
    
    gap: {
      xs: 'gap-3',
      sm: 'gap-4',
      md: 'gap-6',
      lg: 'gap-8',
    },
    
    margin: {
      section: 'mb-6',
      item: 'mb-3',
      group: 'mb-8',
    }
  }
} as const;

export const TYPOGRAPHY = {
  [DeviceType.Mobile]: {
    // Hierarquia de títulos
    display: 'text-2xl font-bold leading-tight',
    h1: 'text-xl font-bold leading-tight',
    h2: 'text-lg font-semibold leading-snug',
    h3: 'text-base font-semibold leading-normal',
    h4: 'text-sm font-semibold leading-normal',
    
    // Textos corporais
    body: 'text-sm leading-relaxed',
    bodyLarge: 'text-base leading-relaxed',
    caption: 'text-xs leading-normal',
    
    // Textos especiais
    price: 'text-lg font-bold text-opamenu-orange',
    priceSmall: 'text-sm font-semibold text-opamenu-orange',
    button: 'text-sm font-medium',
    
    // Labels
    label: 'text-xs font-medium text-muted-foreground uppercase tracking-wide',
  },
  
  [DeviceType.Tablet]: {
    display: 'text-3xl font-bold leading-tight',
    h1: 'text-2xl font-bold leading-tight',
    h2: 'text-xl font-semibold leading-snug',
    h3: 'text-lg font-semibold leading-normal',
    h4: 'text-base font-semibold leading-normal',
    
    body: 'text-base leading-relaxed',
    bodyLarge: 'text-lg leading-relaxed',
    caption: 'text-sm leading-normal',
    
    price: 'text-xl font-bold text-opamenu-orange',
    priceSmall: 'text-base font-semibold text-opamenu-orange',
    button: 'text-base font-medium',
    
    label: 'text-sm font-medium text-muted-foreground uppercase tracking-wide',
  },
  
  [DeviceType.Desktop]: {
    display: 'text-4xl font-bold leading-tight',
    h1: 'text-3xl font-bold leading-tight',
    h2: 'text-2xl font-semibold leading-snug',
    h3: 'text-xl font-semibold leading-normal',
    h4: 'text-lg font-semibold leading-normal',
    
    body: 'text-base leading-relaxed',
    bodyLarge: 'text-lg leading-relaxed',
    caption: 'text-sm leading-normal',
    
    price: 'text-2xl font-bold text-opamenu-orange',
    priceSmall: 'text-lg font-semibold text-opamenu-orange',
    button: 'text-base font-medium',
    
    label: 'text-sm font-medium text-muted-foreground uppercase tracking-wide',
  }
} as const;

export const COMPONENT_SIZES = {
  [DeviceType.Mobile]: {
    // Botões otimizados para touch
    button: {
      xs: 'h-8 px-2 text-xs',
      sm: 'h-10 px-3 text-xs',
      default: 'h-11 px-4 text-sm',
      lg: 'h-12 px-6 text-base',
      xl: 'h-14 px-8 text-lg',
    },
    
    // Inputs com área de toque adequada
    input: {
      sm: 'h-9 px-3 text-sm',
      default: 'h-11 px-4 text-base',
      lg: 'h-12 px-4 text-base',
    },
    
    // Cards e containers
    card: {
      compact: 'min-h-[120px]',
      default: 'min-h-[160px]',
      expanded: 'min-h-[200px]',
    },
    
    // Modal específico para mobile
    modal: {
      width: 'w-[95vw]',
      height: 'max-h-[95vh]',
      borderRadius: 'rounded-t-lg',
    }
  },
  
  [DeviceType.Tablet]: {
    button: {
      xs: 'h-8 px-3 text-xs',
      sm: 'h-9 px-4 text-sm',
      default: 'h-10 px-5 text-base',
      lg: 'h-11 px-6 text-base',
      xl: 'h-12 px-8 text-lg',
    },
    
    input: {
      sm: 'h-9 px-3 text-sm',
      default: 'h-10 px-4 text-base',
      lg: 'h-11 px-4 text-base',
    },
    
    card: {
      compact: 'min-h-[140px]',
      default: 'min-h-[180px]',
      expanded: 'min-h-[220px]',
    },
    
    modal: {
      width: 'w-[90vw] max-w-2xl',
      height: 'max-h-[90vh]',
      borderRadius: 'rounded-lg',
    }
  },
  
  [DeviceType.Desktop]: {
    button: {
      xs: 'h-7 px-2 text-xs',
      sm: 'h-8 px-3 text-sm',
      default: 'h-9 px-4 text-sm',
      lg: 'h-10 px-6 text-base',
      xl: 'h-11 px-8 text-lg',
    },
    
    input: {
      sm: 'h-8 px-3 text-sm',
      default: 'h-9 px-3 text-sm',
      lg: 'h-10 px-4 text-base',
    },
    
    card: {
      compact: 'min-h-[160px]',
      default: 'min-h-[200px]',
      expanded: 'min-h-[280px]',
    },
    
    modal: {
      width: 'max-w-4xl',
      height: 'max-h-[90vh]',
      borderRadius: 'rounded-lg',
    }
  }
} as const;

/**
 * Layout patterns específicos por dispositivo
 */
export const LAYOUT_PATTERNS = {
  [DeviceType.Mobile]: {
    // Stack vertical para mobile
    productModal: 'flex flex-col',
    productGrid: 'grid grid-cols-1 gap-4',
    addonSection: 'space-y-3',
    footer: 'flex flex-col gap-3',
    
    // Navegação mobile
    navigation: 'fixed bottom-0 left-0 right-0',
    header: 'sticky top-0 z-50',
  },
  
  [DeviceType.Tablet]: {
    // Grid híbrido para tablet
    productModal: 'flex flex-col lg:grid lg:grid-cols-2',
    productGrid: 'grid grid-cols-2 gap-4',
    addonSection: 'space-y-4',
    footer: 'flex flex-row items-center justify-between gap-4',
    
    navigation: 'fixed bottom-0 left-0 right-0',
    header: 'sticky top-0 z-50',
  },
  
  [DeviceType.Desktop]: {
    // Layout desktop otimizado
    productModal: 'grid grid-cols-2 gap-6',
    productGrid: 'grid grid-cols-3 xl:grid-cols-4 gap-6',
    addonSection: 'space-y-4',
    footer: 'flex flex-row items-center justify-between gap-6',
    
    navigation: 'flex flex-row items-center justify-between',
    header: 'relative',
  }
} as const;

/**
 * Animations adaptativas por dispositivo
 */
export const ANIMATIONS = {
  [DeviceType.Mobile]: {
    // Animações mais suaves para performance mobile
    transition: 'transition-all duration-200 ease-out',
    hover: '', // Sem hover em mobile
    modal: 'animate-in slide-in-from-bottom duration-300',
    scale: 'active:scale-95',
  },
  
  [DeviceType.Tablet]: {
    transition: 'transition-all duration-250 ease-out',
    hover: 'hover:scale-105 hover:shadow-md',
    modal: 'animate-in slide-in-from-bottom-4 duration-300',
    scale: 'active:scale-95',
  },
  
  [DeviceType.Desktop]: {
    transition: 'transition-all duration-300 ease-out',
    hover: 'hover:scale-105 hover:shadow-lg hover:-translate-y-1',
    modal: 'animate-in fade-in-0 zoom-in-95 duration-200',
    scale: 'hover:scale-105 active:scale-95',
  }
} as const;

/**
 * Helper para obter tokens baseado no dispositivo atual
 */
export const getDeviceTokens = (deviceType: DeviceType) => ({
  spacing: SPACING[deviceType],
  typography: TYPOGRAPHY[deviceType],
  sizes: COMPONENT_SIZES[deviceType],
  layout: LAYOUT_PATTERNS[deviceType],
  animations: ANIMATIONS[deviceType],
});
