import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { api } from '@/lib/api';

export interface Producto {
  id: string;
  nombre: string;
  categoriaId: string;
  categoriaNombre: string;
  unidadMedidaId: string;
  unidadMedidaNombre: string;
  pesoUnitarioKg: number;
  umbralMinimo: number;
  stockActual: number;
  stockActualKg: number;
  isActive: boolean;
  tipoUnidad: number;
  equivalenciaEnKg: number;
  periodoRetiroDias: number;
  version?: string;
}

export interface CrearProductoRequest {
  nombre: string;
  categoriaProductoId: string;
  unidadMedidaId: string;
  pesoUnitarioKg: number;
  umbralMinimo: number;
  stockInicial: number;
  periodoRetiroDias: number;
}

export interface ActualizarProductoRequest {
  id: string;
  nombre: string;
  categoriaProductoId: string;
  unidadMedidaId: string;
  pesoUnitarioKg: number;
  umbralMinimo: number;
  stockInicial: number;
  periodoRetiroDias: number;
  version?: string;
}

export function useProductos() {
  const queryClient = useQueryClient();

  const productos = useQuery({
    queryKey: ['productos'],
    queryFn: () => api.get<Producto[]>('/api/Productos'),
  });

  const crearProducto = useMutation({
    mutationFn: (data: CrearProductoRequest) => api.post<{ productoId: string }>('/api/Productos', data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['productos'] });
      queryClient.invalidateQueries({ queryKey: ['catalogos', 'productos'] });
    },
  });

  return {
    productos: productos.data || [],
    isLoading: productos.isLoading,
    crearProducto,
    refresh: () => productos.refetch(),
  };
}

export function useProducto(id: string) {
  const queryClient = useQueryClient();

  const producto = useQuery({
    queryKey: ['productos', id],
    queryFn: () => api.get<Producto>(`/api/Productos/${id}`),
    enabled: !!id,
  });

  const actualizarProducto = useMutation({
    mutationFn: (data: ActualizarProductoRequest) => api.put(`/api/Productos/${data.id}`, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['productos'] });
      queryClient.invalidateQueries({ queryKey: ['productos', id] });
    },
  });

  const eliminarProducto = useMutation({
    mutationFn: () => api.delete(`/api/Productos/${id}`),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['productos'] }),
  });

  return {
    producto: producto.data,
    isLoading: producto.isLoading,
    actualizarProducto,
    eliminarProducto,
  };
}
