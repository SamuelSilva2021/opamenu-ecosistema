import { Plus, Trash2 } from 'lucide-react';
import { useState, type KeyboardEvent } from 'react';

interface StringListInputProps {
  label?: string;
  value: string[];
  onChange: (value: string[]) => void;
  placeholder?: string;
  error?: string;
}

export const StringListInput = ({
  label,
  value = [],
  onChange,
  placeholder = 'Digite um item e pressione Enter',
  error
}: StringListInputProps) => {
  const [newItem, setNewItem] = useState('');

  const handleAdd = () => {
    if (newItem.trim()) {
      onChange([...value, newItem.trim()]);
      setNewItem('');
    }
  };

  const handleRemove = (index: number) => {
    onChange(value.filter((_, i) => i !== index));
  };

  const handleKeyDown = (e: KeyboardEvent) => {
    if (e.key === 'Enter') {
      e.preventDefault();
      handleAdd();
    }
  };

  return (
    <div>
      {label && (
        <label className="block text-sm font-medium text-slate-700 mb-1">
          {label}
        </label>
      )}
      
      <div className="flex gap-2 mb-2">
        <input
          type="text"
          value={newItem}
          onChange={(e) => setNewItem(e.target.value)}
          onKeyDown={handleKeyDown}
          placeholder={placeholder}
          className="flex-1 rounded-md border-slate-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm p-2 border"
        />
        <button
          type="button"
          onClick={handleAdd}
          className="p-2 bg-blue-100 text-blue-600 rounded-md hover:bg-blue-200 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
          title="Adicionar"
        >
          <Plus size={20} />
        </button>
      </div>

      {value.length > 0 ? (
        <ul className="space-y-2 bg-slate-50 p-3 rounded-md border border-slate-200">
          {value.map((item, index) => (
            <li key={index} className="flex items-center justify-between bg-white p-2 rounded border border-slate-200 shadow-sm">
              <span className="text-sm text-slate-700">{item}</span>
              <button
                type="button"
                onClick={() => handleRemove(index)}
                className="text-red-500 hover:text-red-700 p-1 rounded-full hover:bg-red-50"
                title="Remover"
              >
                <Trash2 size={16} />
              </button>
            </li>
          ))}
        </ul>
      ) : (
        <p className="text-sm text-slate-500 italic">Nenhum item adicionado.</p>
      )}
      
      {error && <p className="text-red-500 text-xs mt-1">{error}</p>}
    </div>
  );
};
