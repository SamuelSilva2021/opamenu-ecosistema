import { useEffect, useState } from 'react';
import { Plus, X } from 'lucide-react';
import { toast } from 'sonner';
import { type TenantProduct, TenantProductStatus } from '../../types/tenant-product.types';
import { tenantProductService } from '../../services/tenant-product.service';
import { TenantProductForm } from './components/TenantProductForm';
import { SearchFilter } from '../../shared/components/data-display/SearchFilter';
import { DataTable, type Column } from '../../shared/components/data-display/DataTable';
import { EditAction, DeleteAction } from '../../shared/components/actions/TableActions';
import { useConfirmation } from '../../context/ConfirmationContext';

export const TenantProductsPage = () => {
  const { confirm } = useConfirmation();
  const [products, setProducts] = useState<TenantProduct[]>([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editingProduct, setEditingProduct] = useState<TenantProduct | undefined>(undefined);
  const [saving, setSaving] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');

  const loadProducts = async () => {
    try {
      setLoading(true);
      const response = await tenantProductService.getAll();
      if (Array.isArray(response)) {
        setProducts(response);
      }
    } catch (error) {
      console.error('Erro ao carregar produtos:', error);
      toast.error('Erro ao carregar produtos');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadProducts();
  }, []);

  const handleCreate = () => {
    setEditingProduct(undefined);
    setShowModal(true);
  };

  const handleEdit = (product: TenantProduct) => {
    setEditingProduct(product);
    setShowModal(true);
  };

  const handleDelete = (id: string) => {
    confirm({
      title: 'Excluir Produto',
      message: 'Tem certeza que deseja excluir este produto? Esta ação não pode ser desfeita.',
      confirmText: 'Excluir',
      variant: 'danger',
      onConfirm: async () => {
        try {
          const response = await tenantProductService.delete(id);
          if (response) {
            toast.success('Produto excluído com sucesso');
            loadProducts();
          } else {
            toast.error('Erro ao excluir produto');
          }
        } catch (error) {
          console.error('Erro ao excluir produto:', error);
          toast.error('Erro ao excluir produto');
        }
      }
    });
  };

  const handleSubmit = async (data: any) => {
    try {
      setSaving(true);
      let response;
      if (editingProduct) {
        response = await tenantProductService.update(editingProduct.id, data);
      } else {
        response = await tenantProductService.create(data);
      }

      // Verifica se retornou o objeto criado/editado (que deve ter ID)
      if (response && response.id) {
        toast.success(editingProduct ? 'Produto atualizado com sucesso' : 'Produto criado com sucesso');
        setShowModal(false);
        loadProducts();
      } else {
        toast.error('Erro ao salvar produto: Resposta inesperada da API');
      }
    } catch (error: any) {
      console.error('Erro ao salvar produto:', error);
      const message = error.response?.data?.message || 'Erro ao salvar produto';
      toast.error(message);
    } finally {
      setSaving(false);
    }
  };

  const filteredProducts = products.filter(product => 
    product.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
    product.slug.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const columns: Column<TenantProduct>[] = [
    {
      header: 'Produto',
      cell: (product) => (
        <div className="flex flex-col">
          <span className="font-medium text-slate-900">{product.name}</span>
          <span className="text-xs text-slate-500">{product.slug}</span>
        </div>
      )
    },
    {
      header: 'Categoria',
      accessorKey: 'category',
      className: 'whitespace-nowrap text-sm text-slate-500'
    },
    {
      header: 'Versão',
      accessorKey: 'version',
      className: 'whitespace-nowrap text-sm text-slate-500'
    },
    {
      header: 'Preço',
      className: 'whitespace-nowrap text-sm text-slate-500',
      cell: (product) => (
        <>
          R$ {product.basePrice.toFixed(2)}
          {product.pricingModel === 'subscription' && <span className="text-xs">/mês</span>}
        </>
      )
    },
    {
      header: 'Status',
      cell: (product) => {
        const statusMap: Record<TenantProductStatus, { label: string; className: string }> = {
          [TenantProductStatus.Ativo]: { label: 'Ativo', className: 'bg-green-100 text-green-800' },
          [TenantProductStatus.Inativo]: { label: 'Inativo', className: 'bg-red-100 text-red-800' },
          [TenantProductStatus.Descontinuado]: { label: 'Descontinuado', className: 'bg-yellow-100 text-yellow-800' },
        };
        const status = statusMap[product.status] || { label: 'Desconhecido', className: 'bg-gray-100 text-gray-800' };
        
        return (
          <span className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full ${status.className}`}>
            {status.label}
          </span>
        );
      }
    },
    {
      header: 'Ações',
      align: 'right',
      className: 'whitespace-nowrap text-sm font-medium',
      cell: (product) => (
        <div className="flex items-center justify-end gap-2">
          <EditAction onClick={() => handleEdit(product)} className="mr-2" />
          <DeleteAction onClick={() => handleDelete(product.id)} />
        </div>
      )
    }
  ];

  return (
    <div className="p-6 space-y-6">
      <div className="flex justify-between items-center">
        <div>
          <h1 className="text-2xl font-bold text-slate-900">Produtos</h1>
          <p className="text-slate-500">Gerencie os produtos/aplicações disponíveis para tenants.</p>
        </div>
        <button
          onClick={handleCreate}
          className="flex items-center gap-2 bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors"
        >
          <Plus size={20} />
          Novo Produto
        </button>
      </div>

      <div className="bg-white rounded-lg shadow overflow-hidden">
        <SearchFilter
          value={searchTerm}
          onChange={setSearchTerm}
          placeholder="Buscar produtos..."
        />

        <DataTable
          data={filteredProducts}
          columns={columns}
          loading={loading}
          keyExtractor={(product) => product.id}
          emptyMessage="Nenhum produto encontrado."
        />
      </div>

      {/* Modal */}
      {showModal && (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/50 backdrop-blur-sm">
          <div className="bg-white rounded-xl shadow-xl w-full max-w-4xl max-h-[90vh] overflow-y-auto">
            <div className="p-6 border-b border-slate-200 flex justify-between items-center sticky top-0 bg-white z-10">
              <h2 className="text-xl font-bold text-slate-900">
                {editingProduct ? 'Editar Produto' : 'Novo Produto'}
              </h2>
              <button
                onClick={() => setShowModal(false)}
                className="text-slate-400 hover:text-slate-600"
              >
                <X size={24} />
              </button>
            </div>
            
            <div className="p-6">
              <TenantProductForm
                initialData={editingProduct}
                onSubmit={handleSubmit}
                onCancel={() => setShowModal(false)}
                isLoading={saving}
              />
            </div>
          </div>
        </div>
      )}
    </div>
  );
};
