import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import type { TenantProduct } from '../../../types/tenant-product.types';
import { EProductStatus, ETenantProductCategory, ETenantProductPricingModel } from '../../../types/tenant-product.types';
import { useEffect } from 'react';
import { JsonInput } from '../../../shared/components/inputs/JsonInput';

const schema = z.object({
  name: z.string().min(3, 'Nome deve ter no mínimo 3 caracteres'),
  slug: z.string().min(3, 'Slug deve ter no mínimo 3 caracteres').regex(/^[a-z0-9-]+$/, 'Slug deve conter apenas letras minúsculas, números e hífens'),
  description: z.string().optional(),
  category: z.nativeEnum(ETenantProductCategory),
  version: z.string().min(1, 'Versão é obrigatória').default('1.0.0'),
  status: z.nativeEnum(EProductStatus).default(EProductStatus.Ativo),
  configurationSchema: z.string().optional(),
  pricingModel: z.nativeEnum(ETenantProductPricingModel).default(ETenantProductPricingModel.Assinatura),
  basePrice: z.coerce.number().min(0, 'Preço base não pode ser negativo'),
  setupFee: z.coerce.number().min(0, 'Taxa de configuração não pode ser negativa'),
});

type FormData = z.infer<typeof schema>;

interface TenantProductFormProps {
  initialData?: TenantProduct;
  onSubmit: (data: FormData) => void;
  onCancel: () => void;
  isLoading?: boolean;
}

export const TenantProductForm = ({ initialData, onSubmit, onCancel, isLoading }: TenantProductFormProps) => {
  const { register, handleSubmit, formState: { errors }, reset, setValue, watch } = useForm<FormData>({
    resolver: zodResolver(schema) as any,
    defaultValues: {
      name: '',
      slug: '',
      description: '',
      category: ETenantProductCategory.WebApp,
      version: '1.0.0',
      status: EProductStatus.Ativo,
      configurationSchema: '{}',
      pricingModel: ETenantProductPricingModel.Assinatura,
      basePrice: 0,
      setupFee: 0,
    }
  });

  const configurationSchema = watch('configurationSchema');

  useEffect(() => {
    if (initialData) {
      reset({
        name: initialData.name,
        slug: initialData.slug,
        description: initialData.description || '',
        category: initialData.category,
        version: initialData.version,
        status: initialData.status,
        configurationSchema: initialData.configurationSchema || '{}',
        pricingModel: initialData.pricingModel,
        basePrice: initialData.basePrice,
        setupFee: initialData.setupFee,
      });
    }
  }, [initialData, reset]);

  const onFormSubmit = (data: FormData) => {
    onSubmit(data);
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
          <label className="block text-sm font-medium text-slate-700">Categoria</label>
          <select
            {...register('category')}
            className="mt-1 block w-full rounded-md border-slate-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm p-2 border"
          >
            {Object.values(ETenantProductCategory).map((category) => (
              <option key={category} value={category}>
                {category}
              </option>
            ))}
          </select>
          {errors.category && <p className="text-red-500 text-xs mt-1">{errors.category.message}</p>}
        </div>

        <div>
          <label className="block text-sm font-medium text-slate-700">Versão</label>
          <input
            {...register('version')}
            className="mt-1 block w-full rounded-md border-slate-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm p-2 border"
          />
          {errors.version && <p className="text-red-500 text-xs mt-1">{errors.version.message}</p>}
        </div>

        <div>
          <label className="block text-sm font-medium text-slate-700">Status</label>
          <select
            {...register('status')}
            className="mt-1 block w-full rounded-md border-slate-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm p-2 border"
          >
            {Object.values(EProductStatus).map((status) => (
              <option key={status} value={status}>
                {status}
              </option>
            ))}
          </select>
          {errors.status && <p className="text-red-500 text-xs mt-1">{errors.status.message}</p>}
        </div>

        <div>
          <label className="block text-sm font-medium text-slate-700">Modelo de Preço</label>
          <select
            {...register('pricingModel')}
            className="mt-1 block w-full rounded-md border-slate-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm p-2 border"
          >
            {Object.values(ETenantProductPricingModel).map((model) => (
              <option key={model} value={model}>
                {model}
              </option>
            ))}
          </select>
          {errors.pricingModel && <p className="text-red-500 text-xs mt-1">{errors.pricingModel.message}</p>}
        </div>

        <div>
          <label className="block text-sm font-medium text-slate-700">Preço Base</label>
          <input
            type="number"
            step="0.01"
            {...register('basePrice')}
            className="mt-1 block w-full rounded-md border-slate-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm p-2 border"
          />
          {errors.basePrice && <p className="text-red-500 text-xs mt-1">{errors.basePrice.message}</p>}
        </div>

        <div>
          <label className="block text-sm font-medium text-slate-700">Taxa de Setup</label>
          <input
            type="number"
            step="0.01"
            {...register('setupFee')}
            className="mt-1 block w-full rounded-md border-slate-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm p-2 border"
          />
          {errors.setupFee && <p className="text-red-500 text-xs mt-1">{errors.setupFee.message}</p>}
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
        <JsonInput
          label="Schema de Configuração (JSON)"
          value={configurationSchema || '{}'}
          onChange={(val) => setValue('configurationSchema', val)}
          error={errors.configurationSchema?.message}
          height="h-64"
        />
        <p className="text-xs text-slate-500 mt-1">Defina a estrutura de configuração deste produto em formato JSON.</p>
      </div>

      <div className="flex justify-end gap-3 pt-4 border-t">
        <button
          type="button"
          onClick={onCancel}
          className="px-4 py-2 text-sm font-medium text-slate-700 bg-white border border-slate-300 rounded-md shadow-sm hover:bg-slate-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
          disabled={isLoading}
        >
          Cancelar
        </button>
        <button
          type="submit"
          className="px-4 py-2 text-sm font-medium text-white bg-blue-600 border border-transparent rounded-md shadow-sm hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 disabled:opacity-50"
          disabled={isLoading}
        >
          {isLoading ? 'Salvando...' : 'Salvar Produto'}
        </button>
      </div>
    </form>
  );
};
