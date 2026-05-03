import {
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  IconButton,
  Tooltip,
  Chip,
  Typography,
} from '@mui/material';
import {
  Edit as EditIcon,
  Delete as DeleteIcon,
} from '@mui/icons-material';
import type { TenantProduct } from '../../../shared/types';

interface ProductsListProps {
  products: TenantProduct[];
  onEdit: (product: TenantProduct) => void;
  onDelete: (id: string) => void;
}

export function ProductsList({ products, onEdit, onDelete }: ProductsListProps) {
  if (products.length === 0) {
    return (
      <Typography variant="body1" sx={{ textAlign: 'center', py: 4 }}>
        Nenhum produto cadastrado.
      </Typography>
    );
  }

  return (
    <TableContainer>
      <Table>
        <TableHead>
          <TableRow>
            <TableCell>Nome</TableCell>
            <TableCell>Slug</TableCell>
            <TableCell>Categoria</TableCell>
            <TableCell>Versão</TableCell>
            <TableCell>Preço Base</TableCell>
            <TableCell>Assinaturas</TableCell>
            <TableCell>Status</TableCell>
            <TableCell align="right">Ações</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {products.map((product) => (
            <TableRow key={product.id}>
              <TableCell>
                <Typography variant="body2" fontWeight="medium">
                  {product.name}
                </Typography>
              </TableCell>
              <TableCell>{product.slug}</TableCell>
              <TableCell>
                <Chip label={product.category} size="small" />
              </TableCell>
              <TableCell>{product.version}</TableCell>
              <TableCell>
                {product.basePrice.toLocaleString('pt-BR', {
                  style: 'currency',
                  currency: 'BRL',
                })}
              </TableCell>
              <TableCell>
                <Tooltip title={`${product.activeSubscriptions} ativas / ${product.totalSubscriptions} totais`}>
                  <Typography variant="body2">
                    {product.activeSubscriptions} / {product.totalSubscriptions}
                  </Typography>
                </Tooltip>
              </TableCell>
              <TableCell>
                <Chip
                  label={product.status}
                  size="small"
                  color={product.status === 'Ativo' ? 'success' : 'default'}
                />
              </TableCell>
              <TableCell align="right">
                <Tooltip title="Editar">
                  <IconButton onClick={() => onEdit(product)} size="small">
                    <EditIcon fontSize="small" />
                  </IconButton>
                </Tooltip>
                <Tooltip title="Remover">
                  <IconButton
                    onClick={() => onDelete(product.id)}
                    size="small"
                    color="error"
                  >
                    <DeleteIcon fontSize="small" />
                  </IconButton>
                </Tooltip>
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </TableContainer>
  );
}
