import { useEffect, useState } from 'react';
import { Plus, X } from 'lucide-react';
import { toast } from 'sonner';
import type { Plan, PlanFilter } from '../../types/plan.types';
import { planService } from '../../services/plan.service';
import { PlanForm } from './components/PlanForm';
import { SearchFilter } from '../../shared/components/data-display/SearchFilter';
import { DataTable, type Column } from '../../shared/components/data-display/DataTable';
import { EditAction, DeleteAction } from '../../shared/components/actions/TableActions';
import { useConfirmation } from '../../context/ConfirmationContext';

export const PlansPage = () => {
  const { confirm } = useConfirmation();
  const [plans, setPlans] = useState<Plan[]>([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editingPlan, setEditingPlan] = useState<Plan | undefined>(undefined);
  const [saving, setSaving] = useState(false);
  const [filter, setFilter] = useState<PlanFilter>({
    page: 1,
    pageSize: 10,
    status: 'Ativo'
  });
  const [total, setTotal] = useState(0);

  const loadPlans = async () => {
    try {
      setLoading(true);
      const response = await planService.getAll(filter);
      if (response.succeeded) {
        setPlans(response.items);
        setTotal(response.total);
      }
    } catch (error) {
      console.error('Erro ao carregar planos:', error);
      toast.error('Erro ao carregar planos');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadPlans();
  }, [filter]);

  const handleCreate = () => {
    setEditingPlan(undefined);
    setShowModal(true);
  };

  const handleEdit = (plan: Plan) => {
    setEditingPlan(plan);
    setShowModal(true);
  };

  const handleDelete = (id: string) => {
    confirm({
      title: 'Excluir Plano',
      message: 'Tem certeza que deseja excluir este plano? Esta ação não pode ser desfeita.',
      confirmText: 'Excluir',
      variant: 'danger',
      onConfirm: async () => {
        try {
          const response = await planService.delete(id);
          if (response) {
            toast.success('Plano excluído com sucesso');
            loadPlans();
          } else {
            toast.error('Erro ao excluir plano: Resposta inesperada da API');
          }
        } catch (error: any) {
          console.error('Erro ao excluir plano:', error);
          const message = error.response?.data?.message || 'Erro ao excluir plano';
          toast.error(message);
        }
      }
    });
  };

  const handleSubmit = async (data: any) => {
    try {
      setSaving(true);
      let response;
      if (editingPlan) {
        response = await planService.update(editingPlan.id, data);
      } else {
        response = await planService.create(data);
      }

      // Se chegou aqui é porque deu sucesso (status 2xx) e retornou o objeto
      if (response && response.id) {
        toast.success(editingPlan ? 'Plano atualizado com sucesso' : 'Plano criado com sucesso');
        setShowModal(false);
        loadPlans();
      } else {
        // Caso inesperado
        toast.error('Erro ao salvar plano: Resposta inválida da API');
      }
    } catch (error: any) {
      console.error('Erro ao salvar plano:', error);
      // Tentar extrair mensagem de erro do backend se houver
      const message = error.response?.data?.message || 'Erro ao salvar plano';
      toast.error(message);
    } finally {
      setSaving(false);
    }
  };

  const columns: Column<Plan>[] = [
    {
      header: 'Nome',
      accessorKey: 'name',
      className: 'font-medium text-slate-900'
    },
    {
      header: 'Preço',
      cell: (plan) => `R$ ${plan.price.toFixed(2)}`
    },
    {
      header: 'Ciclo',
      cell: (plan) => <span className="capitalize">{plan.billingCycle}</span>
    },
    {
      header: 'Usuários',
      accessorKey: 'maxUsers'
    },
    {
      header: 'Status',
      cell: (plan) => {
        const statusColors: Record<string, string> = {
          'Ativo': 'bg-green-100 text-green-800',
          'Inativo': 'bg-red-100 text-red-800',
          'Pendente': 'bg-yellow-100 text-yellow-800',
          'Cancelado': 'bg-gray-100 text-gray-800',
        };
        return (
          <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${statusColors[plan.status] || 'bg-gray-100 text-gray-800'}`}>
            {plan.status}
          </span>
        );
      }
    },
    {
      header: 'Ações',
      align: 'right',
      cell: (plan) => (
        <div className="flex items-center justify-end gap-2">
          <EditAction onClick={() => handleEdit(plan)} className="mr-2" />
          <DeleteAction onClick={() => handleDelete(plan.id)} />
        </div>
      )
    }
  ];

  return (
    <div className="p-6 space-y-6">
      <div className="flex justify-between items-center">
        <div>
          <h1 className="text-2xl font-bold text-slate-900">Planos</h1>
          <p className="text-slate-500">Gerencie os planos de assinatura disponíveis.</p>
        </div>
        <button
          onClick={handleCreate}
          className="flex items-center gap-2 bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors"
        >
          <Plus size={20} />
          Novo Plano
        </button>
      </div>

      <div className="bg-white rounded-xl shadow-sm border border-slate-200 overflow-hidden">
        <SearchFilter
          value={filter.name || ''}
          onChange={(val) => setFilter({ ...filter, name: val, page: 1 })}
          placeholder="Buscar planos..."
        />
        
        <DataTable
          data={plans}
          columns={columns}
          loading={loading}
          keyExtractor={(plan) => plan.id}
          pagination={{
            currentPage: filter.page || 1,
            totalItems: total,
            pageSize: filter.pageSize || 10,
            onPageChange: (page) => setFilter({ ...filter, page })
          }}
        />
      </div>

      {/* Modal */}
      {showModal && (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/50 backdrop-blur-sm">
          <div className="bg-white rounded-xl shadow-xl w-full max-w-2xl max-h-[90vh] overflow-y-auto">
            <div className="p-6 border-b border-slate-200 flex justify-between items-center sticky top-0 bg-white z-10">
              <h2 className="text-xl font-bold text-slate-900">
                {editingPlan ? 'Editar Plano' : 'Novo Plano'}
              </h2>
              <button
                onClick={() => setShowModal(false)}
                className="text-slate-400 hover:text-slate-600"
              >
                <X size={24} />
              </button>
            </div>
            
            <div className="p-6">
              <PlanForm
                initialData={editingPlan}
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
