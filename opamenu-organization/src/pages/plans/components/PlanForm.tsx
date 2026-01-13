import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import type { Plan } from '../../../types/plan.types';
import { useEffect, useState } from 'react';
import { StringListInput } from '../../../shared/components/inputs/StringListInput';

const schema = z.object({
  name: z.string().min(3, 'Nome deve ter no mínimo 3 caracteres'),
  slug: z.string().min(3, 'Slug deve ter no mínimo 3 caracteres').regex(/^[a-z0-9-]+$/, 'Slug deve conter apenas letras minúsculas, números e hífens'),
  description: z.string().optional(),
  price: z.coerce.number().min(0, 'Preço não pode ser negativo'),
  billingCycle: z.enum(['Mensal', 'Anual', 'Semestral', 'Semanal', 'Diario']),
  maxUsers: z.coerce.number().min(1, 'Mínimo de 1 usuário'),
  maxStorageGb: z.coerce.number().min(1, 'Mínimo de 1 GB'),
  features: z.string().optional(),
  status: z.enum(['Ativo', 'Inativo', 'Pendente', 'Cancelado']),
  sortOrder: z.coerce.number().int(),
  isTrial: z.boolean().optional(),
  trialPeriodDays: z.coerce.number().int().optional(),
});

type FormData = z.infer<typeof schema>;

interface PlanFormProps {
  initialData?: Plan;
  onSubmit: (data: FormData) => void;
  onCancel: () => void;
  isLoading?: boolean;
}

export const PlanForm = ({ initialData, onSubmit, onCancel, isLoading }: PlanFormProps) => {
  const [featuresList, setFeaturesList] = useState<string[]>([]);

  const { register, handleSubmit, watch, formState: { errors }, reset } = useForm<FormData>({
    resolver: zodResolver(schema) as any,
    defaultValues: {
      name: '',
      slug: '',
      description: '',
      price: 0,
      billingCycle: 'Mensal',
      maxUsers: 1,
      maxStorageGb: 1,
      features: '',
      status: 'Ativo',
      sortOrder: 0,
      isTrial: false,
      trialPeriodDays: 0,
    }
  });

  const isTrial = watch('isTrial');

  useEffect(() => {
    if (initialData) {
      reset({
        name: initialData.name,
        slug: initialData.slug,
        description: initialData.description || '',
        price: initialData.price,
        billingCycle: initialData.billingCycle,
        maxUsers: initialData.maxUsers,
        maxStorageGb: initialData.maxStorageGb,
        features: initialData.features || '',
        status: initialData.status,
        sortOrder: initialData.sortOrder,
        isTrial: initialData.isTrial || false,
        trialPeriodDays: initialData.trialPeriodDays || 0,
      });

      if (initialData.features) {
        try {
          const parsed = JSON.parse(initialData.features);
          if (Array.isArray(parsed)) {
            setFeaturesList(parsed);
          } else {
            setFeaturesList([String(parsed)]);
          }
        } catch (e) {
          if (initialData.features.trim()) {
            setFeaturesList([initialData.features]);
          }
        }
      } else {
        setFeaturesList([]);
      }
    }
  }, [initialData, reset]);

  const onFormSubmit = (data: FormData) => {
    const featuresJson = JSON.stringify(featuresList);
    onSubmit({ ...data, features: featuresJson });
  };

  return (
    <form onSubmit={handleSubmit(onFormSubmit)} className="space-y-4">
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div>
          <label className="block text-sm font-medium text-slate-700">Nome</label>
          <input
            {...register('name')}
            className="mt-1 block w-full rounded-md border-slate-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm p-2 border"
          />
          {errors.name && <p className="text-red-500 text-xs mt-1">{errors.name.message}</p>}
        </div>

        <div>
          <label className="block text-sm font-medium text-slate-700">Slug</label>
          <input
            {...register('slug')}
            className="mt-1 block w-full rounded-md border-slate-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm p-2 border"
          />
          {errors.slug && <p className="text-red-500 text-xs mt-1">{errors.slug.message}</p>}
        </div>

        <div>
          <label className="block text-sm font-medium text-slate-700">Preço</label>
          <input
            type="number"
            step="0.01"
            {...register('price')}
            className="mt-1 block w-full rounded-md border-slate-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm p-2 border"
          />
          {errors.price && <p className="text-red-500 text-xs mt-1">{errors.price.message}</p>}
        </div>

        <div>
          <label className="block text-sm font-medium text-slate-700">Ciclo de Cobrança</label>
          <select
            {...register('billingCycle')}
            className="mt-1 block w-full rounded-md border-slate-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm p-2 border"
          >
            <option value="Mensal">Mensal</option>
            <option value="Anual">Anual</option>
            <option value="Semestral">Semestral</option>
            <option value="Semanal">Semanal</option>
            <option value="Diario">Diário</option>
          </select>
          {errors.billingCycle && <p className="text-red-500 text-xs mt-1">{errors.billingCycle.message}</p>}
        </div>

        <div>
          <label className="block text-sm font-medium text-slate-700">Máx. Usuários</label>
          <input
            type="number"
            {...register('maxUsers')}
            className="mt-1 block w-full rounded-md border-slate-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm p-2 border"
          />
          {errors.maxUsers && <p className="text-red-500 text-xs mt-1">{errors.maxUsers.message}</p>}
        </div>

        <div>
          <label className="block text-sm font-medium text-slate-700">Armazenamento (GB)</label>
          <input
            type="number"
            {...register('maxStorageGb')}
            className="mt-1 block w-full rounded-md border-slate-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm p-2 border"
          />
          {errors.maxStorageGb && <p className="text-red-500 text-xs mt-1">{errors.maxStorageGb.message}</p>}
        </div>

        <div>
          <label className="block text-sm font-medium text-slate-700">Status</label>
          <select
            {...register('status')}
            className="mt-1 block w-full rounded-md border-slate-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm p-2 border"
          >
            <option value="Ativo">Ativo</option>
            <option value="Inativo">Inativo</option>
            <option value="Pendente">Pendente</option>
            <option value="Cancelado">Cancelado</option>
          </select>
          {errors.status && <p className="text-red-500 text-xs mt-1">{errors.status.message}</p>}
        </div>

        <div>
          <label className="block text-sm font-medium text-slate-700">Ordem de Exibição</label>
          <input
            type="number"
            {...register('sortOrder')}
            className="mt-1 block w-full rounded-md border-slate-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm p-2 border"
          />
          {errors.sortOrder && <p className="text-red-500 text-xs mt-1">{errors.sortOrder.message}</p>}
        </div>
      </div>

      <div>
        <label className="block text-sm font-medium text-slate-700">Descrição</label>
        <textarea
          {...register('description')}
          rows={3}
          className="mt-1 block w-full rounded-md border-slate-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm p-2 border"
        />
        {errors.description && <p className="text-red-500 text-xs mt-1">{errors.description.message}</p>}
      </div>

      <div>
        <StringListInput
          label="Funcionalidades"
          value={featuresList}
          onChange={setFeaturesList}
          error={errors.features?.message}
        />
        <input type="hidden" {...register('features')} />
      </div>

      <div className="flex flex-col gap-4 p-4 border rounded-md bg-slate-50">
        <div className="flex items-center">
          <input
            type="checkbox"
            {...register('isTrial')}
            className="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
          />
          <label className="ml-2 block text-sm text-slate-900">É um plano de teste (Trial)?</label>
        </div>

        {isTrial && (
          <div>
            <label className="block text-sm font-medium text-slate-700">Dias de Teste</label>
            <input
              type="number"
              {...register('trialPeriodDays')}
              className="mt-1 block w-full rounded-md border-slate-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm p-2 border"
            />
            {errors.trialPeriodDays && <p className="text-red-500 text-xs mt-1">{errors.trialPeriodDays.message}</p>}
          </div>
        )}
      </div>

      <div className="flex justify-end gap-3 pt-4 border-t border-slate-200">
        <button
          type="button"
          onClick={onCancel}
          className="px-4 py-2 text-sm font-medium text-slate-700 bg-white border border-slate-300 rounded-md hover:bg-slate-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
        >
          Cancelar
        </button>
        <button
          type="submit"
          disabled={isLoading}
          className="px-4 py-2 text-sm font-medium text-white bg-blue-600 border border-transparent rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 disabled:opacity-50"
        >
          {isLoading ? 'Salvando...' : 'Salvar'}
        </button>
      </div>
    </form>
  );
};
