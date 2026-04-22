import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { api } from '@/lib/api';

export interface FormulaDetalle {
  id: string;
  productoId: string;
  productoNombre: string;
  cantidadPorBase: number;
}

export interface Formula {
  id: string;
  nombre: string;
  etapa: string;
  cantidadBase: number;
  isActive: boolean;
  detalles: FormulaDetalle[];
}

export interface FormulaFormValues {
  nombre: string;
  etapa: string;
  cantidadBase: number;
  detalles: {
    productoId: string;
    cantidadPorBase: number;
  }[];
}

export interface ConsumoFormulaFormValues {
  loteId: string;
  formulaId: string;
  cantidadTotalPreparada: number;
  fecha: string;
  justificacion?: string;
}

export function useFormulas() {
  const queryClient = useQueryClient();

  const formulas = useQuery({
    queryKey: ['formulas'],
    queryFn: () => api.get<Formula[]>('/api/Formulas'),
  });

  const crearFormula = useMutation({
    mutationFn: (data: FormulaFormValues) => api.post('/api/Formulas', data),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['formulas'] }),
  });

  const actualizarFormula = useMutation({
    mutationFn: (data: { id: string } & FormulaFormValues) => api.put(`/api/Formulas/${data.id}`, data),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['formulas'] }),
  });

  const eliminarFormula = useMutation({
    mutationFn: (id: string) => api.delete(`/api/Formulas/${id}`),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['formulas'] }),
  });

  const registrarConsumo = useMutation({
    mutationFn: ({ data, idempotencyKey }: { data: ConsumoFormulaFormValues; idempotencyKey?: string }) => 
      api.post(`/api/Lotes/${data.loteId}/consumo-formula`, data, idempotencyKey),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['inventario'] });
      queryClient.invalidateQueries({ queryKey: ['lotes'] });
    },
  });

  return {
    formulas: formulas.data || [],
    isLoading: formulas.isLoading,
    crearFormula,
    actualizarFormula,
    eliminarFormula,
    registrarConsumo,
  };
}
