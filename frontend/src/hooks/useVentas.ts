import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { api } from '@/lib/api';

export interface Venta {
  id: string;
  loteId: string;
  clienteId: string;
  clienteNombre: string;
  fecha: string;
  cantidadPollos: number;
  pesoTotalKg: number;
  precioPorKilo: number;
  total: number;
  saldoPendiente: number;
  estadoPago: string;
  version?: string;
}

export interface Pago {
  id: string;
  monto: number;
  fechaPago: string;
  metodoPago: number;
}

export interface RegistrarVentaDto {
  loteId: string;
  clienteId: string;
  fecha: string;
  cantidadPollos: number;
  pesoTotalVendido: number;
  precioPorKilo: number;
}

export interface ActualizarVentaDto {
  ventaId: string;
  cantidadPollos: number;
  pesoTotalVendido: number;
  precioPorKilo: number;
  version?: string;
}

export interface RegistrarPagoDto {
  monto: number;
  fechaPago: string;
  metodoPago: number;
}

export function useVentas() {
  const queryClient = useQueryClient();

  // GET /api/Ventas
  const todasLasVentas = useQuery({
    queryKey: ['ventas'],
    queryFn: () => api.get<Venta[]>('/api/Ventas'),
  });

  // GET /api/Ventas/{id}
  const useVenta = (id: string) => useQuery({
    queryKey: ['ventas', id],
    queryFn: () => api.get<Venta>(`/api/Ventas/${id}`),
    enabled: !!id,
  });

  // GET /api/Ventas/lote/{loteId}
  const useVentasPorLote = (loteId: string) => useQuery({
    queryKey: ['ventas', 'lote', loteId],
    queryFn: () => api.get<Venta[]>(`/api/Ventas/lote/${loteId}`),
    enabled: !!loteId,
  });

  // GET /api/Ventas/{id}/pagos
  const usePagosDeVenta = (id: string) => useQuery({
    queryKey: ['ventas', id, 'pagos'],
    queryFn: () => api.get<Pago[]>(`/api/Ventas/${id}/pagos`),
    enabled: !!id,
  });

  // POST /api/Ventas/parcial (Registrar Venta)
  const registrarVenta = useMutation({
    mutationFn: ({ data, idempotencyKey }: { data: RegistrarVentaDto; idempotencyKey?: string }) => 
      api.post('/api/Ventas/parcial', data, idempotencyKey),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['ventas'] });
      queryClient.invalidateQueries({ queryKey: ['finanzas', 'cuentas-por-cobrar'] });
    },
  });

  // PUT /api/Ventas/{id}
  const actualizarVenta = useMutation({
    mutationFn: ({ id, data }: { id: string; data: ActualizarVentaDto }) => 
      api.put(`/api/Ventas/${id}`, data),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['ventas'] });
      queryClient.invalidateQueries({ queryKey: ['ventas', variables.id] });
      queryClient.invalidateQueries({ queryKey: ['finanzas', 'cuentas-por-cobrar'] });
    },
  });

  // POST /api/Ventas/{id}/pagos
  const registrarPago = useMutation({
    mutationFn: ({ id, data, idempotencyKey }: { id: string; data: RegistrarPagoDto; idempotencyKey?: string }) => 
      api.post(`/api/Ventas/${id}/pagos`, data, idempotencyKey),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['ventas'] });
      queryClient.invalidateQueries({ queryKey: ['ventas', variables.id] });
      queryClient.invalidateQueries({ queryKey: ['ventas', variables.id, 'pagos'] });
      queryClient.invalidateQueries({ queryKey: ['finanzas', 'cuentas-por-cobrar'] });
    },
  });

  // POST /api/Ventas/{id}/anular
  const anularVenta = useMutation({
    mutationFn: (id: string) => api.post(`/api/Ventas/${id}/anular`, {}),
    onSuccess: (_, id) => {
      queryClient.invalidateQueries({ queryKey: ['ventas'] });
      queryClient.invalidateQueries({ queryKey: ['ventas', id] });
      queryClient.invalidateQueries({ queryKey: ['finanzas', 'cuentas-por-cobrar'] });
    },
  });

  // DELETE /api/Ventas/{id}/pagos/{pagoId}
  const eliminarPago = useMutation({
    mutationFn: ({ ventaId, pagoId }: { ventaId: string; pagoId: string }) => 
      api.delete(`/api/Ventas/${ventaId}/pagos/${pagoId}`),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['ventas'] });
      queryClient.invalidateQueries({ queryKey: ['ventas', variables.ventaId] });
      queryClient.invalidateQueries({ queryKey: ['ventas', variables.ventaId, 'pagos'] });
      queryClient.invalidateQueries({ queryKey: ['finanzas', 'cuentas-por-cobrar'] });
    },
  });

  return {
    todasLasVentas: todasLasVentas.data || [],
    isLoadingVentas: todasLasVentas.isLoading,
    useVenta,
    useVentasPorLote,
    usePagosDeVenta,
    registrarVenta,
    actualizarVenta,
    registrarPago,
    anularVenta,
    eliminarPago,
  };
}
