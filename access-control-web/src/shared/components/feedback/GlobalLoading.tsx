import React from 'react';
import { Box, CircularProgress, Typography, useTheme } from '@mui/material';

export interface GlobalLoadingProps {
  message?: string;
  submessage?: string;
}

/**
 * Componente de loading global da aplicação
 * Usado durante inicializações críticas como carregamento de permissões
 */
export const GlobalLoading: React.FC<GlobalLoadingProps> = ({
  message = 'Carregando...',
  submessage = 'Aguarde enquanto preparamos tudo para você'
}) => {
  const theme = useTheme();

  return (
    <Box
      sx={{
        position: 'fixed',
        top: 0,
        left: 0,
        right: 0,
        bottom: 0,
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
        justifyContent: 'center',
        backgroundColor: theme.palette.background.default,
        zIndex: 9999,
      }}
    >
      <Box
        sx={{
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
          textAlign: 'center',
          gap: 3,
        }}
      >
        {/* Loading Spinner */}
        <CircularProgress
          size={60}
          thickness={4}
          sx={{
            color: theme.palette.primary.main,
          }}
        />

        {/* Título principal */}
        <Typography
          variant="h6"
          color="text.primary"
          sx={{
            fontWeight: 600,
            fontSize: '1.25rem',
          }}
        >
          {message}
        </Typography>

        {/* Submensagem */}
        {submessage && (
          <Typography
            variant="body2"
            color="text.secondary"
            sx={{
              fontSize: '0.875rem',
              maxWidth: 400,
              lineHeight: 1.5,
            }}
          >
            {submessage}
          </Typography>
        )}

        {/* Logo ou branding */}
        <Box
          sx={{
            mt: 2,
            display: 'flex',
            alignItems: 'center',
            gap: 1,
            opacity: 0.7,
          }}
        >
          <Typography
            variant="caption"
            color="text.secondary"
            sx={{
              fontSize: '0.75rem',
              fontWeight: 500,
            }}
          >
            🔐 Access Control System
          </Typography>
        </Box>
      </Box>
    </Box>
  );
};