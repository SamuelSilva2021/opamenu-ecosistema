import { Card, CardContent, Typography, Box, useTheme } from '@mui/material';
import { ReactNode } from 'react';

interface SummaryCardProps {
  title: string;
  value: string | number;
  icon: ReactNode;
  color?: string;
  subtitle?: string;
}

export const SummaryCard = ({ title, value, icon, color, subtitle }: SummaryCardProps) => {
  const theme = useTheme();
  const cardColor = color || theme.palette.primary.main;

  return (
    <Card sx={{ height: '100%' }}>
      <CardContent>
        <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 2 }}>
          <Box
            sx={{
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
              width: 48,
              height: 48,
              borderRadius: '12px',
              backgroundColor: `${cardColor}15`,
              color: cardColor,
            }}
          >
            {icon}
          </Box>
        </Box>
        <Typography variant="body2" color="text.secondary" gutterBottom>
          {title}
        </Typography>
        <Typography variant="h4" component="div" sx={{ fontWeight: 600 }}>
          {value}
        </Typography>
        {subtitle && (
          <Typography variant="caption" color="text.secondary" sx={{ mt: 1, display: 'block' }}>
            {subtitle}
          </Typography>
        )}
      </CardContent>
    </Card>
  );
};
