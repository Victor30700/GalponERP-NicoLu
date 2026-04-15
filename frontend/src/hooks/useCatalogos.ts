import { useQuery } from '@tanstack/react-query';
import { api } from '@/lib/api';

export interface ClienteCatalogo {
  id: string;
  nombre: string;
  ruc: string;
  direccion: string;
  telefono: string;
}

export interface ProductoCatalogo {
  id: string;
  nombre: string;
  categoriaId: string;
  categoriaNombre: string;
  unidadMedidaId: string;
  unidadMedidaNombre: string;
  pesoUnitarioKg: number;
  umbralMinimo: number;
  isActive: boolean;
}

export function useCatalogos() {
  const queryClientes = useQuery({
    queryKey: ['catalogos', 'clientes'],
    queryFn: () => api.get<ClienteCatalogo[]>('/api/Catalogos/clientes'),
  });

  const queryProductos = useQuery({
    queryKey: ['catalogos', 'productos'],
    queryFn: () => api.get<ProductoCatalogo[]>('/api/Catalogos/productos'),
  });

  return {
    clientes: queryClientes.data || [],
    isLoadingClientes: queryClientes.isLoading,
    productos: queryProductos.data || [],
    isLoadingProductos: queryProductos.isLoading,
    refreshClientes: () => queryClientes.refetch(),
    refreshProductos: () => queryProductos.refetch(),
  };
}
