import type { ReactNode } from 'react';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  Typography,
} from '@mui/material';

export interface ConfirmDialogProps {
  open: boolean;
  title: string;
  description?: ReactNode;
  confirmLabel?: string;
  cancelLabel?: string;
  isSubmitting?: boolean;
  onConfirm: () => void;
  onCancel: () => void;
}

export const ConfirmDialog = ({
  open,
  title,
  description,
  confirmLabel = 'Confirmar',
  cancelLabel = 'Cancelar',
  isSubmitting = false,
  onConfirm,
  onCancel,
}: ConfirmDialogProps) => {
  const handleClose = () => {
    if (!isSubmitting) {
      onCancel();
    }
  };

  const handleConfirm = () => {
    if (!isSubmitting) {
      onConfirm();
    }
  };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="xs" fullWidth>
      <DialogTitle>{title}</DialogTitle>
      {description && (
        <DialogContent>
          {typeof description === 'string' ? (
            <Typography variant="body1">{description}</Typography>
          ) : (
            description
          )}
        </DialogContent>
      )}
      <DialogActions>
        <Button onClick={handleClose} disabled={isSubmitting} color="inherit">
          {cancelLabel}
        </Button>
        <Button
          onClick={handleConfirm}
          color="error"
          variant="contained"
          disabled={isSubmitting}
        >
          {confirmLabel}
        </Button>
      </DialogActions>
    </Dialog>
  );
};

