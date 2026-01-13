import { useState, useEffect } from 'react';

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

/**
 * Hook para detectar o tipo de dispositivo baseado em media queries
 * Retorna o tipo atual e atualiza automaticamente quando a tela é redimensionada
 */
export const useDeviceType = (): DeviceType => {
  const [deviceType, setDeviceType] = useState<DeviceType>(() => {
    // Detectar tipo inicial no lado cliente
    if (typeof window === 'undefined') return DeviceType.Mobile;
    
    if (window.matchMedia(BREAKPOINTS.desktop).matches) {
      return DeviceType.Desktop;
    } else if (window.matchMedia(BREAKPOINTS.tablet).matches) {
      return DeviceType.Tablet;
    }
    return DeviceType.Mobile;
  });

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

    // Detectar mudanças de breakpoint
    const mediaQueries = Object.values(BREAKPOINTS).map(query => 
      window.matchMedia(query)
    );
    
    mediaQueries.forEach(mq => mq.addEventListener('change', updateDeviceType));
    
    // Verificar estado inicial
    updateDeviceType();
    
    return () => {
      mediaQueries.forEach(mq => mq.removeEventListener('change', updateDeviceType));
    };
  }, []);

  return deviceType;
};

/**
 * Hook para verificar se está em um dispositivo específico
 */
export const useIsDevice = (targetDevice: DeviceType): boolean => {
  const currentDevice = useDeviceType();
  return currentDevice === targetDevice;
};

/**
 * Hook para verificar se está em mobile (mobile ou tablet)
 */
export const useIsMobile = (): boolean => {
  const deviceType = useDeviceType();
  return deviceType === DeviceType.Mobile || deviceType === DeviceType.Tablet;
};

/**
 * Hook para verificar se está em desktop
 */
export const useIsDesktop = (): boolean => {
  const deviceType = useDeviceType();
  return deviceType === DeviceType.Desktop;
};
