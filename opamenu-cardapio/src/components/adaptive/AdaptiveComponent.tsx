import React, { Suspense } from 'react';
import { useDeviceType, DeviceType } from '@/hooks/useDeviceType';

interface AdaptiveComponentProps<T = any> {
  /** Componente para dispositivos móveis (320px - 767px) */
  mobile?: React.ComponentType<T>;
  /** Componente para tablets (768px - 1023px) */
  tablet?: React.ComponentType<T>;
  /** Componente para desktop (1024px+) */
  desktop?: React.ComponentType<T>;
  /** Componente fallback se não houver variant específica */
  fallback?: React.ComponentType<T>;
  /** Props a serem passadas para o componente renderizado */
  props?: T;
  /** Loading component durante lazy loading */
  loading?: React.ReactNode;
}

/**
 * Componente que renderiza diferentes variants baseado no dispositivo
 * Implementa mobile-first: mobile -> tablet -> desktop -> fallback
 */
export const AdaptiveComponent = <T,>({
  mobile,
  tablet,
  desktop,
  fallback,
  props,
  loading,
}: AdaptiveComponentProps<T>) => {
  const deviceType = useDeviceType();
  
  let Component: React.ComponentType<T> | undefined;
  
  switch (deviceType) {
    case DeviceType.Mobile:
      Component = mobile || fallback;
      break;
    case DeviceType.Tablet:
      // Tablet fallback para mobile, depois fallback geral
      Component = tablet || mobile || fallback;
      break;
    case DeviceType.Desktop:
      // Desktop específico, senão fallback geral
      Component = desktop || fallback;
      break;
  }
  
  if (!Component) {
    console.warn('AdaptiveComponent: No component found for device type:', deviceType);
    return null;
  }
  
  return (
    <Suspense fallback={loading || null}>
      <Component {...(props as T)} />
    </Suspense>
  );
};

/**
 * HOC para criar componentes adaptativos
 */
export const withAdaptiveVariants = <T,>(variants: {
  mobile?: React.ComponentType<T>;
  tablet?: React.ComponentType<T>;
  desktop?: React.ComponentType<T>;
  fallback?: React.ComponentType<T>;
}) => {
  return (props: T) => (
    <AdaptiveComponent
      mobile={variants.mobile}
      tablet={variants.tablet}
      desktop={variants.desktop}
      fallback={variants.fallback}
      props={props}
    />
  );
};

/**
 * Componente condicional baseado em breakpoint
 */
interface ResponsiveProps {
  children: React.ReactNode;
  /** Mostrar apenas em mobile */
  showOn?: DeviceType | DeviceType[];
  /** Esconder em dispositivos específicos */
  hideOn?: DeviceType | DeviceType[];
}

export const Responsive: React.FC<ResponsiveProps> = ({ 
  children, 
  showOn, 
  hideOn 
}) => {
  const deviceType = useDeviceType();
  
  // Verificar se deve mostrar
  if (showOn) {
    const showDevices = Array.isArray(showOn) ? showOn : [showOn];
    if (!showDevices.includes(deviceType)) {
      return null;
    }
  }
  
  // Verificar se deve esconder
  if (hideOn) {
    const hideDevices = Array.isArray(hideOn) ? hideOn : [hideOn];
    if (hideDevices.includes(deviceType)) {
      return null;
    }
  }
  
  return <>{children}</>;
};

// Componentes de conveniência
export const MobileOnly: React.FC<{ children: React.ReactNode }> = ({ children }) => (
  <Responsive showOn={DeviceType.Mobile}>{children}</Responsive>
);

export const TabletOnly: React.FC<{ children: React.ReactNode }> = ({ children }) => (
  <Responsive showOn={DeviceType.Tablet}>{children}</Responsive>
);

export const DesktopOnly: React.FC<{ children: React.ReactNode }> = ({ children }) => (
  <Responsive showOn={DeviceType.Desktop}>{children}</Responsive>
);

export const MobileAndTablet: React.FC<{ children: React.ReactNode }> = ({ children }) => (
  <Responsive showOn={[DeviceType.Mobile, DeviceType.Tablet]}>{children}</Responsive>
);

export const NotMobile: React.FC<{ children: React.ReactNode }> = ({ children }) => (
  <Responsive hideOn={DeviceType.Mobile}>{children}</Responsive>
);
