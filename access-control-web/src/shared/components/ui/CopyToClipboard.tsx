import React, { useState } from 'react';
import { IconButton, Tooltip, Zoom } from '@mui/material';
import { ContentCopy as ContentCopyIcon, Check as CheckIcon } from '@mui/icons-material';

interface CopyToClipboardProps {
  content: string;
  tooltipTitle?: string;
  successTitle?: string;
}

export const CopyToClipboard: React.FC<CopyToClipboardProps> = ({
  content,
  tooltipTitle = 'Copiar',
  successTitle = 'Copiado!',
}) => {
  const [copied, setCopied] = useState(false);

  const handleCopy = async (e: React.MouseEvent) => {
    e.stopPropagation();
    try {
      await navigator.clipboard.writeText(content);
      setCopied(true);
      setTimeout(() => setCopied(false), 2000);
    } catch (error) {
      console.error('Failed to copy:', error);
    }
  };

  return (
    <Tooltip
      title={copied ? successTitle : tooltipTitle}
      TransitionComponent={Zoom}
      arrow
      placement="top"
    >
      <IconButton size="small" onClick={handleCopy} color={copied ? 'success' : 'default'}>
        {copied ? <CheckIcon fontSize="small" /> : <ContentCopyIcon fontSize="small" />}
      </IconButton>
    </Tooltip>
  );
};
