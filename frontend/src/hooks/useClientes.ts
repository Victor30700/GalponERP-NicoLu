import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { api } from '@/lib/api';

export interface Cliente {
  id: string;
  nombre: string;
  ruc?: string;
  direccion?: string;
  telefono?: string;
  email?: string;
  totalVentasRealizadas?: number;
  saldoPendienteTotal?: number;
}

export interface ClienteHistorial {
  id: string;
  fecha: string;
  tipo: string;
  referencia: string;
  monto: number;
  saldo: number;
}

export interface ClienteRequest {
  nombre: string;
  ruc?: string;
  direccion?: string;
  telefono?: string;
  email?: string;
}

export function useClientes() {
  const queryClient = useQueryClient();

  const clientes = useQuery({
    queryKey: ['clientes'],
    queryFn: () => api.get<Cliente[]>('/api/Clientes'),
  });

  const crearCliente = useMutation({
    mutationFn: (data: ClienteRequest) => api.post('/api/Clientes', data),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['clientes'] }),
  });

  return {
    clientes: clientes.data || [],
    isLoading: clientes.isLoading,
    crearCliente,
    refresh: () => clientes.refetch(),
  };
}

export function useCliente(id: string) {
  const queryClient = useQueryClient();

  const cliente = useQuery({
    queryKey: ['clientes', id],
    queryFn: () => api.get<Cliente>(`/api/Clientes/${id}`),
    enabled: !!id,
  });

  const historial = useQuery({
    queryKey: ['clientes', id, 'historial'],
    queryFn: () => api.get<ClienteHistorial[]>(`/api/Clientes/${id}/historial`),
    enabled: !!id,
  });

  const actualizarCliente = useMutation({
    mutationFn: (data: ClienteRequest & { id: string }) => api.put(`/api/Clientes/${id}`, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['clientes'] });
      queryClient.invalidateQueries({ queryKey: ['clientes', id] });
    },
  });

  const eliminarCliente = useMutation({
    mutationFn: () => api.delete(`/api/Clientes/${id}`),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['clientes'] }),
  });

  return {
    cliente: cliente.data,
    historial: historial.data || [],
    isLoading: cliente.isLoading || historial.isLoading,
    actualizarCliente,
    eliminarCliente,
  };
}
