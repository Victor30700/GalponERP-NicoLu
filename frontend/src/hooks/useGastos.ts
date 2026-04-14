import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { api } from '@/lib/api';
import { toast } from 'sonner';

export interface Gasto {
  id: string;
  galponId: string;
  loteId: string;
  descripcion: string;
  monto: number | { monto: number }; // Manejar ambos formatos observados en el API
  fecha: string;
  tipoGasto: string;
  usuarioId: string;
  isActive: boolean;
  fechaCreacion: string;
}

export interface GastoRequest {
  galponId: string;
  loteId: string;
  descripcion: string;
  monto: number;
  fecha: string;
  tipoGasto: string;
}

export function useGastos(filters?: { galponId?: string; loteId?: string }) {
  const queryClient = useQueryClient();

  const queryParams = new URLSearchParams();
  if (filters?.galponId) queryParams.append('galponId', filters.galponId);
  if (filters?.loteId) queryParams.append('loteId', filters.loteId);

  const queryStr = queryParams.toString() ? `?${queryParams.toString()}` : '';

  const { data: gastos = [], isLoading } = useQuery({
    queryKey: ['gastos', filters],
    queryFn: () => api.get<Gasto[]>(`/api/Gastos${queryStr}`),
  });

  const createGasto = useMutation({
    mutationFn: (data: GastoRequest) => api.post<string>('/api/Gastos', data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['gastos'] });
      queryClient.invalidateQueries({ queryKey: ['finanzas'] });
      queryClient.invalidateQueries({ queryKey: ['dashboard'] });
      toast.success('Gasto registrado con éxito');
    },
    onError: (error: any) => toast.error(`Error: ${error.message}`),
  });

  const updateGasto = useMutation({
    mutationFn: ({ id, data }: { id: string; data: Partial<GastoRequest> }) =>
      api.put(`/api/Gastos/${id}`, { id, ...data }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['gastos'] });
      queryClient.invalidateQueries({ queryKey: ['finanzas'] });
      toast.success('Gasto actualizado correctamente');
    },
    onError: (error: any) => toast.error(`Error: ${error.message}`),
  });

  const deleteGasto = useMutation({
    mutationFn: (id: string) => api.delete(`/api/Gastos/${id}`),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['gastos'] });
      queryClient.invalidateQueries({ queryKey: ['finanzas'] });
      toast.success('Gasto eliminado');
    },
    onError: (error: any) => toast.error(`Error: ${error.message}`),
  });

  return {
    gastos,
    isLoading,
    createGasto,
    updateGasto,
    deleteGasto,
  };
}
