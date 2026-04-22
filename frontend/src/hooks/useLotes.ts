import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { api } from '@/lib/api';

export interface Lote {
  id: string;
  nombre: string;
  nombreLote?: string;
  galponId: string;
  galponNombre: string;
  nombreGalpon?: string;
  fechaInicio: string;
  fechaIngreso: string;
  fechaCierre?: string;
  cantidadInicial: number;
  cantidadActual: number;
  avesVivas: number;
  mortalidadAcumulada: number;
  pollosVendidos: number;
  estado: 'Activo' | 'Cerrado' | 'Cancelado';
  fcrActual: number;
  mortalidadPorcentaje: number;
  viabilidad: number;
  costoPorAve: number;
  pesoPromedioActual?: number;
  costoUnitarioPollito: number;
  edadSemanas?: number;
  fechaFinRetiro?: string;
  version?: string;
}

export interface LoteDetalle extends Lote {
  diasEnProduccion: number;
  totalConsumoAlimento: number;
  totalBajas: number;
  costoTotalAcumulado?: number;
  ultimaTemperatura?: number;
  ultimaHumedad?: number;
}

export interface CrearLoteRequest {
  nombre: string;
  galponId: string;
  fechaIngreso: string;
  cantidadInicial: number;
  costoUnitarioPollito: number;
  plantillaSanitariaId?: string;
}

export interface ActualizarLoteRequest {
  id: string;
  nombre: string;
  galponId: string;
  fechaIngreso: string;
  cantidadInicial: number;
  costoUnitarioPollito: number;
  version?: string;
}

export interface CerrarLoteRequest {
  loteId: string;
  fechaCierre: string;
  precioVentaPromedio: number;
  observaciones?: string;
}

export function useLotes(filters?: { soloActivos?: boolean; busqueda?: string; mes?: number | ''; anio?: number | '' }) {
  const queryClient = useQueryClient();

  const queryParams = new URLSearchParams();
  if (filters?.soloActivos !== undefined) queryParams.append('soloActivos', filters.soloActivos.toString());
  if (filters?.busqueda) queryParams.append('busqueda', filters.busqueda);
  if (filters?.mes) queryParams.append('mes', filters.mes.toString());
  if (filters?.anio) queryParams.append('anio', filters.anio.toString());

  const lotes = useQuery({
    queryKey: ['lotes', filters],
    queryFn: () => api.get<Lote[]>(`/api/Lotes?${queryParams.toString()}`),
  });

  const crearLote = useMutation({
    mutationFn: ({ data, idempotencyKey }: { data: CrearLoteRequest; idempotencyKey?: string }) => 
      api.post('/api/Lotes', data, idempotencyKey),
    onMutate: async ({ data: newLote }) => {
      await queryClient.cancelQueries({ queryKey: ['lotes'] });
      const previousLotes = queryClient.getQueryData<Lote[]>(['lotes']);

      if (previousLotes) {
        queryClient.setQueryData<Lote[]>(['lotes'], [
          {
            id: 'temp-' + Date.now(),
            nombre: newLote.nombre,
            galponId: newLote.galponId,
            galponNombre: 'Cargando...',
            fechaInicio: newLote.fechaIngreso,
            fechaIngreso: newLote.fechaIngreso,
            cantidadInicial: newLote.cantidadInicial,
            cantidadActual: newLote.cantidadInicial,
            avesVivas: newLote.cantidadInicial,
            mortalidadAcumulada: 0,
            pollosVendidos: 0,
            estado: 'Activo',
            fcrActual: 0,
            mortalidadPorcentaje: 0,
            viabilidad: 100,
            costoPorAve: newLote.costoUnitarioPollito,
            costoUnitarioPollito: newLote.costoUnitarioPollito
          } as Lote,
          ...previousLotes
        ]);
      }
      return { previousLotes };
    },
    onError: (err, variables, context) => {
      if (context?.previousLotes) {
        queryClient.setQueryData(['lotes'], context.previousLotes);
      }
    },
    onSettled: () => {
      queryClient.invalidateQueries({ queryKey: ['lotes'] });
    },
  });

  return {
    lotes: lotes.data || [],
    isLoading: lotes.isLoading,
    isFetching: lotes.isFetching,
    crearLote,
    refresh: () => lotes.refetch(),
  };
}

export function useLote(id: string) {
  const queryClient = useQueryClient();

  const detalle = useQuery({
    queryKey: ['lote', id],
    queryFn: () => api.get<LoteDetalle>(`/api/Lotes/${id}`),
    enabled: !!id,
  });

  const actualizarLote = useMutation({
    mutationFn: ({ data, idempotencyKey }: { data: ActualizarLoteRequest; idempotencyKey?: string }) => 
      api.put(`/api/Lotes/${id}`, data, idempotencyKey),
    onMutate: async ({ data: updatedLote }) => {
      await queryClient.cancelQueries({ queryKey: ['lote', id] });
      await queryClient.cancelQueries({ queryKey: ['lotes'] });

      const previousLote = queryClient.getQueryData<LoteDetalle>(['lote', id]);
      const previousLotes = queryClient.getQueryData<Lote[]>(['lotes']);

      if (previousLote) {
        queryClient.setQueryData<LoteDetalle>(['lote', id], {
          ...previousLote,
          ...updatedLote
        });
      }

      if (previousLotes) {
        queryClient.setQueryData<Lote[]>(
          ['lotes'],
          previousLotes.map(l => l.id === id ? { ...l, ...updatedLote } : l)
        );
      }

      return { previousLote, previousLotes };
    },
    onError: (err, variables, context) => {
      if (context?.previousLote) {
        queryClient.setQueryData(['lote', id], context.previousLote);
      }
      if (context?.previousLotes) {
        queryClient.setQueryData(['lotes'], context.previousLotes);
      }
    },
    onSettled: () => {
      queryClient.invalidateQueries({ queryKey: ['lotes'] });
      queryClient.invalidateQueries({ queryKey: ['lote', id] });
    },
  });

  const eliminarLote = useMutation({
    mutationFn: () => api.delete(`/api/Lotes/${id}`),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['lotes'] }),
  });

  const cerrarLote = useMutation({
    mutationFn: (data: CerrarLoteRequest) => api.post(`/api/Lotes/${id}/cerrar`, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['lotes'] });
      queryClient.invalidateQueries({ queryKey: ['lote', id] });
    },
  });

  const reabrirLote = useMutation({
    mutationFn: () => api.put(`/api/Lotes/${id}/reabrir`, {}),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['lotes'] });
      queryClient.invalidateQueries({ queryKey: ['lote', id] });
    },
  });

  const cancelarLote = useMutation({
    mutationFn: (justificacion: string) => api.post(`/api/Lotes/${id}/cancelar`, { justificacion }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['lotes'] });
      queryClient.invalidateQueries({ queryKey: ['lote', id] });
    },
  });

  const trasladarLote = useMutation({
    mutationFn: (nuevoGalponId: string) => api.post(`/api/Lotes/${id}/trasladar`, { nuevoGalponId }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['lotes'] });
      queryClient.invalidateQueries({ queryKey: ['lote', id] });
    },
  });

  const rendimientoVivo = useQuery({
    queryKey: ['lote', id, 'rendimiento'],
    queryFn: () => api.get<any>(`/api/Lotes/${id}/rendimiento-vivo`),
    enabled: !!id,
  });

  const descargarReportePdf = async () => {
    const response = await fetch(`${process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5167'}/api/Lotes/${id}/reporte-cierre-pdf`, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('idToken')}`
      }
    });
    if (!response.ok) throw new Error('No se pudo descargar el reporte');
    const blob = await response.blob();
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `Reporte_Lote_${id}.pdf`;
    document.body.appendChild(a);
    a.click();
    window.URL.revokeObjectURL(url);
  };

  return {
    lote: detalle.data,
    isLoading: detalle.isLoading,
    rendimientoVivo: rendimientoVivo.data,
    actualizarLote,
    eliminarLote,
    cerrarLote,
    reabrirLote,
    cancelarLote,
    trasladarLote,
    descargarReportePdf
  };
}
