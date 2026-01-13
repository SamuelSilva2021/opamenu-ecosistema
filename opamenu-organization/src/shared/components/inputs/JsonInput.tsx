import { useState, useEffect } from 'react';

interface JsonInputProps {
  label?: string;
  value: string;
  onChange: (value: string) => void;
  placeholder?: string;
  error?: string;
  height?: string;
}

export const JsonInput = ({
  label,
  value,
  onChange,
  placeholder = '{}',
  error,
  height = 'h-32'
}: JsonInputProps) => {
  const [localValue, setLocalValue] = useState(value);
  const [isValid, setIsValid] = useState(true);

  useEffect(() => {
    setLocalValue(value);
  }, [value]);

  const handleChange = (e: React.ChangeEvent<HTMLTextAreaElement>) => {
    const newValue = e.target.value;
    setLocalValue(newValue);
    
    if (!newValue.trim()) {
      setIsValid(true);
      onChange('');
      return;
    }

    try {
      JSON.parse(newValue);
      setIsValid(true);
      onChange(newValue);
    } catch {
      setIsValid(false);
      // Still propagate change to allow typing, but parent can use error state if needed
      // Ideally parent handles validation, but we can signal invalid JSON here visually
      onChange(newValue);
    }
  };

  const handleBlur = () => {
    if (localValue.trim()) {
      try {
        const parsed = JSON.parse(localValue);
        // Auto-format on blur if valid
        const formatted = JSON.stringify(parsed, null, 2);
        setLocalValue(formatted);
        onChange(formatted);
        setIsValid(true);
      } catch {
        setIsValid(false);
      }
    }
  };

  return (
    <div>
      {label && (
        <label className="block text-sm font-medium text-slate-700 mb-1">
          {label}
        </label>
      )}
      <div className="relative">
        <textarea
          value={localValue}
          onChange={handleChange}
          onBlur={handleBlur}
          placeholder={placeholder}
          className={`block w-full rounded-md border-slate-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm p-2 border font-mono ${height} ${!isValid ? 'border-red-300 focus:border-red-500 focus:ring-red-500' : ''}`}
        />
        {!isValid && (
          <div className="absolute top-2 right-2 text-xs text-red-500 bg-white px-1 rounded border border-red-200">
            JSON Inválido
          </div>
        )}
      </div>
      {error && <p className="text-red-500 text-xs mt-1">{error}</p>}
    </div>
  );
};
