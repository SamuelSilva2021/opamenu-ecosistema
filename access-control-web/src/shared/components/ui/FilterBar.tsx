import React, { useState, useEffect } from 'react';
import { Paper, InputBase, IconButton, Divider } from '@mui/material';
import { Search as SearchIcon, Clear as ClearIcon } from '@mui/icons-material';


interface FilterBarProps {
  onSearch: (value: string) => void;
  placeholder?: string;
  initialValue?: string;
  debounceDelay?: number;
}

export const FilterBar: React.FC<FilterBarProps> = ({
  onSearch,
  placeholder = 'Buscar...',
  initialValue = '',
  debounceDelay = 500,
}) => {
  const [value, setValue] = useState(initialValue);
  
  // Simple internal debounce if hook not found, but let's try to be robust
  useEffect(() => {
    const handler = setTimeout(() => {
      onSearch(value);
    }, debounceDelay);

    return () => {
      clearTimeout(handler);
    };
  }, [value, debounceDelay, onSearch]);

  const handleClear = () => {
    setValue('');
    onSearch('');
  };

  return (
    <Paper
      component="form"
      sx={{ p: '2px 4px', display: 'flex', alignItems: 'center', width: '100%', mb: 2 }}
      onSubmit={(e) => e.preventDefault()}
    >
      <IconButton sx={{ p: '10px' }} aria-label="search" disabled>
        <SearchIcon />
      </IconButton>
      <InputBase
        sx={{ ml: 1, flex: 1 }}
        placeholder={placeholder}
        inputProps={{ 'aria-label': placeholder }}
        value={value}
        onChange={(e) => setValue(e.target.value)}
      />
      {value && (
        <>
          <IconButton sx={{ p: '10px' }} aria-label="clear" onClick={handleClear}>
            <ClearIcon />
          </IconButton>
          <Divider sx={{ height: 28, m: 0.5 }} orientation="vertical" />
        </>
      )}
    </Paper>
  );
};
