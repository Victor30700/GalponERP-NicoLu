import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { api } from '@/lib/api';

export interface StockProducto {
  id: string; // Map from productoId
  productoId: string;
  nombreProducto: string;
  tipoProducto: string;
  stockActual: number;
  stockActualKg: number;
  unidadMedida: string;
}

export interface ValoracionInventario {
  valorTotalEmpresa: number;
  detalles: Array<{
    categoria: string;
    valor: number;
    porcentaje: number;
  }>;
}

export interface ProyeccionInventario {
  id: string; // Map from productoId
  productoId: string;
  nombreProducto: string;
  stockActual: number;
  consumoDiarioEstimado: number;
  diasRestantes: number;
  fechaAgotamientoEstimada: string;
}

export interface KardexItem {
  fecha: string;
  tipo: string;
  referencia: string;
  entrada: number;
  salida: number;
  saldo: number;
  costoUnitario: number;
}

export interface Movimiento {
  id: string;
  productoId: string;
  nombreProducto: string;
  loteId: string | null;
  cantidad: number;
  tipo: string;
  fecha: string;
  justificacion: string | null;
}

export interface Compra {
  id: string;
  proveedorId: string;
  proveedorNombre: string;
  fecha: string;
  total: number;
  totalPagado: number;
  saldoPendiente: number;
  estadoPago: string;
  nota: string | null;
}

export interface PagoCompra {
  id: string;
  monto: number;
  fechaPago: string;
  metodoPago: string;
}

export interface NivelesAlimento {
  stockActualAlimento: number;
  consumoDiarioGlobal: number;
  diasRestantes: number;
  requiereAlerta: boolean;
}

export interface CompraFormValues {
  productoId: string;
  cantidad: number;
  costoTotalCompra: number;
  proveedorId: string;
  montoPagado: number;
  nota?: string;
}

export interface AjusteFormValues {
  productoId: string;
  loteId?: string;
  cantidad: number;
  tipo: number; // 0 para entrada, 1 para salida? Segun implementar.md
  fecha: string;
  justificacion: string;
}

export interface PagoFormValues {
  monto: number;
  fechaPago: string;
  metodoPago: number;
}

export interface ConciliacionItem {
  productoId: string;
  cantidadFisica: number;
  nota?: string;
}

export interface ConciliacionFormValues {
  items: ConciliacionItem[];
}

export function useInventario() {
  const queryClient = useQueryClient();

  const stock = useQuery({
    queryKey: ['inventario', 'stock'],
    queryFn: async () => {
      const data = await api.get<StockProducto[]>('/api/Inventario/stock');
      return data.map(item => ({ ...item, id: item.id || item.productoId }));
    },
  });

  const valoracion = useQuery({
    queryKey: ['inventario', 'valoracion'],
    queryFn: () => api.get<ValoracionInventario>('/api/Inventario/valoracion'),
  });

  const proyecciones = useQuery({
    queryKey: ['inventario', 'proyecciones'],
    queryFn: async () => {
      const data = await api.get<ProyeccionInventario[]>('/api/Inventario/proyecciones');
      return data.map(item => ({ ...item, id: item.id || item.productoId }));
    },
  });

  const movimientos = useQuery({
    queryKey: ['inventario', 'movimientos'],
    queryFn: () => api.get<Movimiento[]>('/api/Inventario/movimientos'),
  });

  const compras = useQuery({
    queryKey: ['inventario', 'compras'],
    queryFn: () => api.get<Compra[]>('/api/Inventario/compras'),
  });

  const nivelesAlimento = useQuery({
    queryKey: ['inventario', 'niveles-alimento'],
    queryFn: () => api.get<NivelesAlimento>('/api/Inventario/niveles-alimento'),
  });

  const registrarCompra = useMutation({
    mutationFn: (data: CompraFormValues) => api.post('/api/Inventario/compras', data),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['inventario'] }),
  });

  const registrarAjuste = useMutation({
    mutationFn: (data: any) => api.put('/api/Inventario/ajuste', data),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['inventario'] }),
  });

  const registrarConsumoDiario = useMutation({
    mutationFn: (data: any) => api.post('/api/Inventario/consumo-diario', data),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['inventario'] }),
  });

  const realizarConciliacion = useMutation({
    mutationFn: (data: ConciliacionFormValues) => api.post('/api/Inventario/conciliacion', data),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['inventario'] }),
  });

  return {
    stock: stock.data || [],
    isLoadingStock: stock.isLoading,
    valoracion: valoracion.data,
    isLoadingValoracion: valoracion.isLoading,
    proyecciones: proyecciones.data || [],
    isLoadingProyecciones: proyecciones.isLoading,
    movimientos: movimientos.data || [],
    isLoadingMovimientos: movimientos.isLoading,
    compras: compras.data || [],
    isLoadingCompras: compras.isLoading,
    nivelesAlimento: nivelesAlimento.data,
    isLoadingNiveles: nivelesAlimento.isLoading,
    registrarCompra,
    registrarAjuste,
    registrarConsumoDiario,
    realizarConciliacion,
  };
}

export function useProductoDetalle(productoId: string) {
  const stock = useQuery({
    queryKey: ['inventario', 'producto', productoId, 'stock'],
    queryFn: () => api.get<StockProducto>(`/api/Inventario/productos/${productoId}/stock`),
    enabled: !!productoId,
  });

  const kardex = useQuery({
    queryKey: ['inventario', 'producto', productoId, 'kardex'],
    queryFn: () => api.get<KardexItem[]>(`/api/Inventario/productos/${productoId}/kardex`),
    enabled: !!productoId,
  });

  const movimientos = useQuery({
    queryKey: ['inventario', 'producto', productoId, 'movimientos'],
    queryFn: () => api.get<Movimiento[]>(`/api/Inventario/productos/${productoId}/movimientos`),
    enabled: !!productoId,
  });

  return {
    stock: stock.data,
    isLoadingStock: stock.isLoading,
    kardex: kardex.data || [],
    isLoadingKardex: kardex.isLoading,
    movimientos: movimientos.data || [],
    isLoadingMovimientos: movimientos.isLoading,
  };
}

export function useCompraPagos(compraId: string) {
  const queryClient = useQueryClient();

  const pagos = useQuery({
    queryKey: ['inventario', 'compra', compraId, 'pagos'],
    queryFn: () => api.get<PagoCompra[]>(`/api/Inventario/compras/${compraId}/pagos`),
    enabled: !!compraId,
  });

  const registrarPago = useMutation({
    mutationFn: (data: PagoFormValues) => api.post(`/api/Inventario/compras/${compraId}/pagos`, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['inventario', 'compra', compraId, 'pagos'] });
      queryClient.invalidateQueries({ queryKey: ['inventario', 'compras'] });
    },
  });

  const eliminarPago = useMutation({
    mutationFn: (pagoId: string) => api.delete(`/api/Inventario/compras/${compraId}/pagos/${pagoId}`),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['inventario', 'compra', compraId, 'pagos'] });
      queryClient.invalidateQueries({ queryKey: ['inventario', 'compras'] });
    },
  });

  return {
    pagos: pagos.data || [],
    isLoadingPagos: pagos.isLoading,
    registrarPago,
    eliminarPago,
  };
}

export function useMovimientosLote(loteId: string) {
  return useQuery({
    queryKey: ['inventario', 'lote', loteId, 'movimientos'],
    queryFn: () => api.get<Movimiento[]>(`/api/Inventario/lote/${loteId}/movimientos`),
    enabled: !!loteId,
  });
}
