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
  descripcion?: string | null;
  tipo: TipoCategoria;
  version?: string;
}

export interface CategoriaRequest {
  nombre: string;
  descripcion?: string | null;
  tipo: TipoCategoria;
  version?: string;
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

  const actualizarCategoria = useMutation({
    mutationFn: (data: CategoriaRequest & { id: string }) => api.put(`/api/Categorias/${data.id}`, data),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['categorias'] }),
  });

  const eliminarCategoria = useMutation({
    mutationFn: (id: string) => api.delete(`/api/Categorias/${id}`),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['categorias'] }),
  });

  return {
    categorias: categorias.data || [],
    isLoading: categorias.isLoading,
    crearCategoria,
    actualizarCategoria,
    eliminarCategoria,
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

  return {
    categoria: categoria.data,
    isLoading: categoria.isLoading,
  };
}
