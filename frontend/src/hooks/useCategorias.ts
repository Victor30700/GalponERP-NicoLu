import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { api } from '@/lib/api';

export enum TipoCategoria {
  Otros = 0,
  Alimento = 1,
  Medicamento = 2,
  Vacuna = 3,
  Insumo = 4,
  ActivoFijo = 5
}

export interface Categoria {
  id: string;
  nombre: string;
  descripcion?: string;
  tipo: TipoCategoria;
}

export interface CategoriaRequest {
  nombre: string;
  descripcion?: string;
  tipo: TipoCategoria;
}

export function useCategorias() {
  const queryClient = useQueryClient();

  const categorias = useQuery({
    queryKey: ['categorias'],
    queryFn: () => api.get<Categoria[]>('/api/Categorias'),
  });

  const crearCategoria = useMutation({
    mutationFn: (data: CategoriaRequest) => api.post('/api/Categorias', data),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['categorias'] }),
  });

  return {
    categorias: categorias.data || [],
    isLoading: categorias.isLoading,
    crearCategoria,
    refresh: () => categorias.refetch(),
  };
}

export function useCategoria(id: string) {
  const queryClient = useQueryClient();

  const categoria = useQuery({
    queryKey: ['categorias', id],
    queryFn: () => api.get<Categoria>(`/api/Categorias/${id}`),
    enabled: !!id,
  });

  const actualizarCategoria = useMutation({
    mutationFn: (data: CategoriaRequest & { id: string }) => api.put(`/api/Categorias/${id}`, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['categorias'] });
      queryClient.invalidateQueries({ queryKey: ['categorias', id] });
    },
  });

  const eliminarCategoria = useMutation({
    mutationFn: () => api.delete(`/api/Categorias/${id}`),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['categorias'] }),
  });

  return {
    categoria: categoria.data,
    isLoading: categoria.isLoading,
    actualizarCategoria,
    eliminarCategoria,
  };
}
