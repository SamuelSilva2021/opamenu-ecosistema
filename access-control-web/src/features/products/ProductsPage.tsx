import { useState, useEffect } from 'react';
import {
  Typography,
  Box,
  Button,
  Paper,
  Alert,
  CircularProgress,
} from '@mui/material';
import { Add as AddIcon } from '@mui/icons-material';
import { ResponsiveContainer } from '../../shared/components';
import { ProductService } from '../../shared/services';
import type { TenantProduct } from '../../shared/types';
import { ProductsList } from './components/ProductsList';
import { ProductFormDialog } from './components/ProductFormDialog';

export function ProductsPage() {
  const [products, setProducts] = useState<TenantProduct[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [formOpen, setFormOpen] = useState(false);
  const [editingProduct, setEditingProduct] = useState<TenantProduct | undefined>();

  const loadProducts = async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await ProductService.getProducts();
      setProducts(data);
    } catch (err: any) {
      setError(err.message || 'Erro ao carregar produtos');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadProducts();
  }, []);

  const handleAddProduct = () => {
    setEditingProduct(undefined);
    setFormOpen(true);
  };

  const handleEditProduct = (product: TenantProduct) => {
    setEditingProduct(product);
    setFormOpen(true);
  };

  const handleDeleteProduct = async (id: string) => {
    if (!window.confirm('Tem certeza que deseja remover este produto?')) return;

    try {
      await ProductService.deleteProduct(id);
      await loadProducts();
    } catch (err: any) {
      setError(err.message || 'Erro ao remover produto');
    }
  };

  const handleFormClose = () => {
    setFormOpen(false);
    setEditingProduct(undefined);
  };

  const handleFormSubmit = async () => {
    setFormOpen(false);
    setEditingProduct(undefined);
    await loadProducts();
  };

  return (
    <ResponsiveContainer>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Typography variant="h4" component="h1">
          Produtos
        </Typography>
        <Button
          variant="contained"
          startIcon={<AddIcon />}
          onClick={handleAddProduct}
        >
          Novo Produto
        </Button>
      </Box>

      {error && (
        <Box sx={{ mb: 2 }}>
          <Alert severity="error" onClose={() => setError(null)}>
            {error}
          </Alert>
        </Box>
      )}

      {loading ? (
        <Box display="flex" justifyContent="center" py={4}>
          <CircularProgress />
        </Box>
      ) : (
        <Paper sx={{ p: 2 }}>
          <ProductsList
            products={products}
            onEdit={handleEditProduct}
            onDelete={handleDeleteProduct}
          />
        </Paper>
      )}

      <ProductFormDialog
        open={formOpen}
        product={editingProduct}
        onClose={handleFormClose}
        onSuccess={handleFormSubmit}
      />
    </ResponsiveContainer>
  );
}
