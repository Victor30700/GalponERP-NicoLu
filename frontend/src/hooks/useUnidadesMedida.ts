import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { api } from '@/lib/api';

export interface UnidadMedida {
  id: string;
  nombre: string;
  abreviatura: string;
}

export interface CreateUnidadMedidaRequest {
  nombre: string;
  abreviatura: string;
}

export interface UpdateUnidadMedidaRequest {
  id: string;
  nombre: string;
  abreviatura: string;
}

export function useUnidadesMedida() {
  const queryClient = useQueryClient();

  const unidades = useQuery({
    queryKey: ['unidades-medida'],
    queryFn: () => api.get<UnidadMedida[]>('/api/UnidadesMedida'),
  });

  const createUnidad = useMutation({
    mutationFn: (data: CreateUnidadMedidaRequest) => api.post<string>('/api/UnidadesMedida', data),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['unidades-medida'] }),
  });

  return {
    unidades: unidades.data || [],
    isLoading: unidades.isLoading,
    error: unidades.error,
    createUnidad,
    refresh: () => unidades.refetch(),
  };
}

export function useUnidadMedida(id: string) {
  const queryClient = useQueryClient();

  const unidad = useQuery({
    queryKey: ['unidades-medida', id],
    queryFn: () => api.get<UnidadMedida>(`/api/UnidadesMedida/${id}`),
    enabled: !!id,
  });

  const updateUnidad = useMutation({
    mutationFn: (data: UpdateUnidadMedidaRequest) => api.put(`/api/UnidadesMedida/${id}`, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['unidades-medida'] });
      queryClient.invalidateQueries({ queryKey: ['unidades-medida', id] });
    },
  });

  const deleteUnidad = useMutation({
    mutationFn: () => api.delete(`/api/UnidadesMedida/${id}`),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['unidades-medida'] });
    },
  });

  return {
    unidad: unidad.data,
    isLoading: unidad.isLoading,
    error: unidad.error,
    updateUnidad,
    deleteUnidad,
  };
}
