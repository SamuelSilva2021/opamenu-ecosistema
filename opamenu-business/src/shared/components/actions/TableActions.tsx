import { Edit, Trash2 } from 'lucide-react';
import type { ComponentProps } from 'react';

interface ActionButtonProps extends ComponentProps<'button'> {
  title?: string;
}

export const EditAction = ({ className = '', title = 'Editar', ...props }: ActionButtonProps) => (
  <button
    className={`text-blue-600 hover:text-blue-900 transition-colors p-1 rounded hover:bg-blue-50 ${className}`}
    title={title}
    type="button"
    {...props}
  >
    <Edit size={18} />
  </button>
);

export const DeleteAction = ({ className = '', title = 'Excluir', ...props }: ActionButtonProps) => (
  <button
    className={`text-red-600 hover:text-red-900 transition-colors p-1 rounded hover:bg-red-50 ${className}`}
    title={title}
    type="button"
    {...props}
  >
    <Trash2 size={18} />
  </button>
);
