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
  pesoUnitarioKg: number;
  fechaVencimientoProxima?: string;
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
  version?: string;
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
  metodoPago: number;
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

export interface AjusteInventario {
  id: string;
  productoId: string;
  nombreProducto: string;
  cantidad: number;
  tipo: string;
  fecha: string;
  justificacion: string | null;
  loteId: string | null;
  usuarioId: string;
}

export interface ReporteMovimiento {
  id: string;
  productoId: string;
  nombreProducto: string;
  categoriaProductoId: string | null;
  nombreCategoria: string;
  loteId: string | null;
  cantidad: number;
  tipo: string;
  fecha: string;
  justificacion: string | null;
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

  const ajustes = useQuery({
    queryKey: ['inventario', 'ajustes'],
    queryFn: () => api.get<AjusteInventario[]>('/api/Inventario/ajustes'),
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
    mutationFn: ({ data, idempotencyKey }: { data: CompraFormValues; idempotencyKey?: string }) => 
      api.post('/api/Inventario/compras', data, idempotencyKey),
    onMutate: async () => {
      await queryClient.cancelQueries({ queryKey: ['inventario'] });
      const previousData = queryClient.getQueryData(['inventario', 'compras']);
      return { previousData };
    },
    onError: (err, newData, context) => {
      if (context?.previousData) {
        queryClient.setQueryData(['inventario', 'compras'], context.previousData);
      }
    },
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['inventario'] }),
  });

  const registrarAjuste = useMutation({
    mutationFn: (data: any) => api.put('/api/Inventario/ajuste', data),
    onMutate: async (newAjuste) => {
      // Cancelar refetches en curso
      await queryClient.cancelQueries({ queryKey: ['inventario'] });

      // Capturar estados anteriores
      const previousStock = queryClient.getQueryData<StockProducto[]>(['inventario', 'stock']);
      const previousMovimientos = queryClient.getQueryData<Movimiento[]>(['inventario', 'movimientos']);
      const previousAjustes = queryClient.getQueryData<AjusteInventario[]>(['inventario', 'ajustes']);

      // Actualizar Stock
      if (previousStock) {
        queryClient.setQueryData<StockProducto[]>(['inventario', 'stock'], 
          previousStock.map(p => {
            if (p.productoId === newAjuste.productoId) {
              const diff = newAjuste.tipo === 0 ? newAjuste.cantidad : -newAjuste.cantidad;
              const newStockActual = p.stockActual + diff;
              return {
                ...p,
                stockActual: newStockActual,
                stockActualKg: newStockActual * (p.pesoUnitarioKg || 0)
              };
            }
            return p;
          })
        );
      }

      // Actualizar Movimientos
      if (previousMovimientos) {
        const product = previousStock?.find(p => p.productoId === newAjuste.productoId);
        queryClient.setQueryData<Movimiento[]>(['inventario', 'movimientos'], [
          {
            id: 'temp-' + Date.now(),
            productoId: newAjuste.productoId,
            nombreProducto: product?.nombreProducto || 'Producto',
            loteId: newAjuste.loteId || null,
            cantidad: newAjuste.cantidad,
            tipo: newAjuste.tipo === 0 ? 'Entrada' : 'Salida',
            fecha: newAjuste.fecha || new Date().toISOString(),
            justificacion: newAjuste.justificacion,
          } as Movimiento,
          ...previousMovimientos
        ]);
      }

      // Actualizar Ajustes
      if (previousAjustes) {
        const product = previousStock?.find(p => p.productoId === newAjuste.productoId);
        queryClient.setQueryData<AjusteInventario[]>(['inventario', 'ajustes'], [
          {
            id: 'temp-' + Date.now(),
            productoId: newAjuste.productoId,
            nombreProducto: product?.nombreProducto || 'Producto',
            cantidad: newAjuste.cantidad,
            tipo: newAjuste.tipo === 0 ? 'Entrada' : 'Salida',
            fecha: newAjuste.fecha || new Date().toISOString(),
            justificacion: newAjuste.justificacion,
            loteId: newAjuste.loteId || null,
            usuarioId: 'temp-user',
          } as AjusteInventario,
          ...previousAjustes
        ]);
      }

      return { previousStock, previousMovimientos, previousAjustes };
    },
    onError: (err, newAjuste, context) => {
      if (context?.previousStock) queryClient.setQueryData(['inventario', 'stock'], context.previousStock);
      if (context?.previousMovimientos) queryClient.setQueryData(['inventario', 'movimientos'], context.previousMovimientos);
      if (context?.previousAjustes) queryClient.setQueryData(['inventario', 'ajustes'], context.previousAjustes);
    },
    onSettled: () => {
      queryClient.invalidateQueries({ queryKey: ['inventario'] });
    },
  });

  const registrarConsumoDiario = useMutation({
    mutationFn: (data: any) => api.post('/api/Inventario/consumo-diario', data),
    onMutate: async (newConsumo) => {
      // Cancelar refetches en curso
      await queryClient.cancelQueries({ queryKey: ['inventario'] });

      // Capturar estados anteriores
      const previousStock = queryClient.getQueryData<StockProducto[]>(['inventario', 'stock']);
      const previousMovimientos = queryClient.getQueryData<Movimiento[]>(['inventario', 'movimientos']);

      // Actualizar Stock (Consumo es Salida)
      if (previousStock) {
        queryClient.setQueryData<StockProducto[]>(['inventario', 'stock'], 
          previousStock.map(p => {
            if (p.productoId === newConsumo.productoId) {
              const newStockActualKg = (p.stockActualKg || 0) - newConsumo.cantidad;
              return {
                ...p,
                stockActualKg: newStockActualKg,
                stockActual: p.pesoUnitarioKg > 0 ? newStockActualKg / p.pesoUnitarioKg : p.stockActual
              };
            }
            return p;
          })
        );
      }

      // Actualizar Movimientos
      if (previousMovimientos) {
        const product = previousStock?.find(p => p.productoId === newConsumo.productoId);
        queryClient.setQueryData<Movimiento[]>(['inventario', 'movimientos'], [
          {
            id: 'temp-' + Date.now(),
            productoId: newConsumo.productoId,
            nombreProducto: product?.nombreProducto || 'Alimento',
            loteId: newConsumo.loteId,
            cantidad: newConsumo.cantidad,
            tipo: 'Salida',
            fecha: newConsumo.fecha || new Date().toISOString(),
            justificacion: newConsumo.justificacion || 'Consumo diario',
          } as Movimiento,
          ...previousMovimientos
        ]);
      }

      return { previousStock, previousMovimientos };
    },
    onError: (err, newConsumo, context) => {
      if (context?.previousStock) queryClient.setQueryData(['inventario', 'stock'], context.previousStock);
      if (context?.previousMovimientos) queryClient.setQueryData(['inventario', 'movimientos'], context.previousMovimientos);
    },
    onSettled: () => {
      queryClient.invalidateQueries({ queryKey: ['inventario'] });
    },
  });

  const eliminarMovimiento = useMutation({
    mutationFn: (id: string) => api.delete(`/api/Inventario/movimiento/${id}`),
    onMutate: async () => {
      await queryClient.cancelQueries({ queryKey: ['inventario'] });
      const previousData = queryClient.getQueryData(['inventario', 'movimientos']);
      return { previousData };
    },
    onError: (err, id, context) => {
      if (context?.previousData) {
        queryClient.setQueryData(['inventario', 'movimientos'], context.previousData);
      }
    },
    onSuccess: () => {
        queryClient.invalidateQueries({ queryKey: ['inventario'] });
        queryClient.invalidateQueries({ queryKey: ['lote'] });
    },
  });

  const actualizarMovimiento = useMutation({
    mutationFn: (data: any) => api.put(`/api/Inventario/movimiento/${data.id}`, data),
    onMutate: async () => {
      await queryClient.cancelQueries({ queryKey: ['inventario'] });
      const previousData = queryClient.getQueryData(['inventario', 'movimientos']);
      return { previousData };
    },
    onError: (err: any, data, context) => {
        if (context?.previousData) {
            queryClient.setQueryData(['inventario', 'movimientos'], context.previousData);
        }
        if (err.message !== 'CONCURRENCY_ERROR') {
            // Manejado globalmente
        }
    },
    onSuccess: () => {
        queryClient.invalidateQueries({ queryKey: ['inventario'] });
        queryClient.invalidateQueries({ queryKey: ['lote'] });
    },
  });

  const realizarConciliacion = useMutation({
    mutationFn: (data: ConciliacionFormValues) => api.post('/api/Inventario/conciliacion', data),
    onMutate: async () => {
      await queryClient.cancelQueries({ queryKey: ['inventario'] });
      const previousData = queryClient.getQueryData(['inventario', 'stock']);
      return { previousData };
    },
    onError: (err, data, context) => {
      if (context?.previousData) {
        queryClient.setQueryData(['inventario', 'stock'], context.previousData);
      }
    },
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['inventario'] }),
  });

  const ajustarStock = useMutation({
    mutationFn: (data: { productoId: string; cantidadFisica: number; nota?: string }) => 
      api.post('/api/Inventario/ajustar-stock', data),
    onMutate: async () => {
      await queryClient.cancelQueries({ queryKey: ['inventario'] });
      const previousData = queryClient.getQueryData(['inventario', 'stock']);
      return { previousData };
    },
    onError: (err, data, context) => {
      if (context?.previousData) {
        queryClient.setQueryData(['inventario', 'stock'], context.previousData);
      }
    },
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
    ajustes: ajustes.data || [],
    isLoadingAjustes: ajustes.isLoading,
    compras: compras.data || [],
    isLoadingCompras: compras.isLoading,
    nivelesAlimento: nivelesAlimento.data,
    isLoadingNiveles: nivelesAlimento.isLoading,
    registrarCompra,
    registrarAjuste,
    registrarConsumoDiario,
    eliminarMovimiento,
    actualizarMovimiento,
    realizarConciliacion,
    ajustarStock,
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
    mutationFn: ({ data, idempotencyKey }: { data: PagoFormValues; idempotencyKey?: string }) => 
      api.post(`/api/Inventario/compras/${compraId}/pagos`, data, idempotencyKey),
    onMutate: async () => {
      await queryClient.cancelQueries({ queryKey: ['inventario', 'compra', compraId, 'pagos'] });
      const previousData = queryClient.getQueryData(['inventario', 'compra', compraId, 'pagos']);
      return { previousData };
    },
    onError: (err, newData, context) => {
      if (context?.previousData) {
        queryClient.setQueryData(['inventario', 'compra', compraId, 'pagos'], context.previousData);
      }
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['inventario', 'compra', compraId, 'pagos'] });
      queryClient.invalidateQueries({ queryKey: ['inventario', 'compras'] });
    },
  });

  const eliminarPago = useMutation({
    mutationFn: (pagoId: string) => api.delete(`/api/Inventario/compras/${compraId}/pagos/${pagoId}`),
    onMutate: async () => {
      await queryClient.cancelQueries({ queryKey: ['inventario', 'compra', compraId, 'pagos'] });
      const previousData = queryClient.getQueryData(['inventario', 'compra', compraId, 'pagos']);
      return { previousData };
    },
    onError: (err, id, context) => {
      if (context?.previousData) {
        queryClient.setQueryData(['inventario', 'compra', compraId, 'pagos'], context.previousData);
      }
    },
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
