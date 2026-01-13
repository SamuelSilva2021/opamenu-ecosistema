import { API_CONFIG } from '@/config/api';

export const DebugInfo = () => {
  return (
    <div style={{ 
      position: 'fixed', 
      bottom: '10px', 
      right: '10px', 
      background: 'rgba(0,0,0,0.8)', 
      color: 'white', 
      padding: '10px', 
      borderRadius: '5px',
      fontSize: '12px',
      zIndex: 9999
    }}>
      <div><strong>API Base URL:</strong> {API_CONFIG.BASE_URL}</div>
      <div><strong>VITE_API_URL:</strong> {import.meta.env.VITE_API_URL || 'undefined'}</div>
      <div><strong>Mode:</strong> {import.meta.env.MODE}</div>
      <div><strong>Dev:</strong> {import.meta.env.DEV ? 'true' : 'false'}</div>
    </div>
  );
};

export default DebugInfo;
