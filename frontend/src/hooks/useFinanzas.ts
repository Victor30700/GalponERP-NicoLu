import { useQuery } from '@tanstack/react-query';
import { api } from '@/lib/api';

export interface VentaFlujo {
  id: string;
  fecha: string;
  lote: string;
  monto: number;
}

export interface GastoFlujo {
  id: string;
  fecha: string;
  descripcion: string;
  tipo: string;
  monto: number;
}

export interface FlujoCaja {
  totalIngresos: number;
  totalEgresos: number;
  utilidadNeta: number;
  ventas: VentaFlujo[];
  gastos: GastoFlujo[];
}

export interface GastoPorCategoria {
  categoria: string;
  total: number;
}

export interface Gasto {
  id: string;
  galponId: string;
  loteId: string;
  descripcion: string;
  monto: number;
  fecha: string;
  tipoGasto: string;
  usuarioId: string;
}

export interface CuentaPorCobrar {
  ventaId: string;
  fecha: string;
  clienteNombre: string;
  loteCodigo: string;
  totalVenta: number;
  totalPagado: number;
  saldoPendiente: number;
  estadoPago: string;
}

export interface DetalleFlujoProyectado {
  concepto: string;
  monto: number;
  tipo: string;
  fechaEstimada: string;
}

export interface FlujoProyectado {
  saldoActual: number;
  totalCuentasPorCobrar: number;
  totalCuentasPorPagar: number;
  costoProyectadoAlimento30Dias: number;
  flujoNetoProyectado30Dias: number;
  detalle: DetalleFlujoProyectado[];
}

export function useFinanzas(params?: { inicio?: string; fin?: string; categoria?: string }) {
  const queryParams = new URLSearchParams();
  if (params?.inicio) queryParams.append('inicio', params.inicio);
  if (params?.fin) queryParams.append('fin', params.fin);
  if (params?.categoria) queryParams.append('categoria', params.categoria);

  const queryStr = queryParams.toString() ? `?${queryParams.toString()}` : '';

  const flujoCaja = useQuery({
    queryKey: ['finanzas', 'flujo-caja', params],
    queryFn: () => api.get<FlujoCaja>(`/api/Finanzas/flujo-caja${queryStr}`),
  });

  const gastosPorCategoria = useQuery({
    queryKey: ['finanzas', 'gastos-por-categoria', params],
    queryFn: () => api.get<GastoPorCategoria[]>(`/api/Finanzas/gastos-por-categoria${queryStr}`),
  });

  const gastos = useQuery({
    queryKey: ['finanzas', 'gastos', params],
    queryFn: () => api.get<Gasto[]>(`/api/Finanzas/gastos${queryStr}`),
  });

  const flujoProyectado = useQuery({
    queryKey: ['finanzas', 'flujo-proyectado'],
    queryFn: () => api.get<FlujoProyectado>('/api/Finanzas/flujo-proyectado'),
  });

  const cuentasPorCobrar = useQuery({
    queryKey: ['finanzas', 'cuentas-por-cobrar'],
    queryFn: () => api.get<CuentaPorCobrar[]>('/api/Finanzas/cuentas-por-cobrar'),
  });

  return {
    flujoCaja: flujoCaja.data,
    isLoadingFlujo: flujoCaja.isLoading,
    gastosPorCategoria: gastosPorCategoria.data || [],
    isLoadingCategorias: gastosPorCategoria.isLoading,
    gastos: gastos.data || [],
    isLoadingGastos: gastos.isLoading,
    flujoProyectado: flujoProyectado.data,
    isLoadingProyectado: flujoProyectado.isLoading,
    cuentasPorCobrar: cuentasPorCobrar.data || [],
    isLoadingCxC: cuentasPorCobrar.isLoading,
  };
}
