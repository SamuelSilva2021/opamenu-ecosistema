import { useState } from 'react';
import type { ReactNode } from 'react';
import {
  Box,
  Toolbar,
  useTheme,
  useMediaQuery,
} from '@mui/material';
import { AppHeader } from './AppHeader';
import { Sidebar } from './Sidebar';
import { layout } from '../../theme';

export interface MainLayoutProps {
  children: ReactNode;
}

/**
 * Layout principal da aplicação com sidebar e header
 * Responsivo com controle de estado da sidebar
 */
export const MainLayout = ({ children }: MainLayoutProps) => {
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down('lg'));
  
  const [sidebarOpen, setSidebarOpen] = useState(!isMobile);

  const handleSidebarToggle = () => {
    setSidebarOpen(!sidebarOpen);
  };

  const handleSidebarClose = () => {
    setSidebarOpen(false);
  };

  return (
    <Box sx={{ display: 'flex', minHeight: '100vh' }}>
      {/* Header */}
      <AppHeader 
        open={sidebarOpen} 
        onMenuClick={handleSidebarToggle} 
      />

      {/* Sidebar */}
      <Sidebar
        open={sidebarOpen}
        onClose={handleSidebarClose}
        onToggle={handleSidebarToggle}
      />

      {/* Main Content */}
      <Box
        component="main"
        sx={{
          flexGrow: 1,
          display: 'flex',
          flexDirection: 'column',
          minHeight: '100vh',
          backgroundColor: theme.palette.background.default,
          width: '100%', // Garante que o container ocupe a largura disponível
          overflow: 'hidden', // Evita scroll horizontal indesejado
        }}
      >
        {/* Toolbar spacer */}
        <Toolbar sx={{ minHeight: `${layout.headerHeight}px !important` }} />
        
        {/* Content */}
        <Box sx={{ flex: 1, overflow: 'auto' }}>
          {children}
        </Box>
      </Box>
    </Box>
  );
};