import { useQuery } from '@tanstack/react-query';
import { api } from '@/lib/api';

export interface AlertaStockMinimo {
  productoNombre: string;
  stockActual: number;
  umbralMinimo: number;
}

export interface DashboardResumen {
  totalPollosVivos: number;
  mortalidadMesActual: number;
  stockAlimentoActual: number;
  requiereAlertaAlimento: boolean;
  diasAlimentoRestantes: number;
  saldoPorCobrarTotal: number;
  tareasSanitariasHoy: number;
  inversionTotalEnCurso: number;
  fcrPromedioEmpresa: number;
  alertasStockMinimo: AlertaStockMinimo[];
}

export interface ComparativaLote {
  loteId: string;
  fechaIngreso: string;
  cantidadInicial: number;
  mortalidadTotal: number;
  fcrFinal: number;
  totalVentas: number;
  totalGastos: number;
  utilidadNeta: number;
}

export interface ComparativaGalpon {
  galponId: string;
  nombre: string;
  totalLotes: number;
  promedioMortalidad: number;
  promedioFCR: number;
  utilidadTotalAcumulada: number;
}

export interface ProyeccionSacrificio {
  fechaEstimada: string;
  pesoEstimado: number;
  diasRestantes: number;
  fcrEstimado: number;
}

export function useDashboard() {
  const resumen = useQuery({
    queryKey: ['dashboard', 'resumen'],
    queryFn: () => api.get<DashboardResumen>('/api/Dashboard/resumen'),
  });

  const comparativaLotes = useQuery({
    queryKey: ['dashboard', 'comparativa-lotes'],
    queryFn: () => api.get<ComparativaLote[]>('/api/Dashboard/comparativa-lotes'),
  });

  const comparativaGalpones = useQuery({
    queryKey: ['dashboard', 'comparativa-galpones'],
    queryFn: () => api.get<ComparativaGalpon[]>('/api/Dashboard/comparativa-galpones'),
  });

  return {
    resumen: resumen.data,
    isLoadingResumen: resumen.isLoading,
    comparativaLotes: comparativaLotes.data || [],
    isLoadingLotes: comparativaLotes.isLoading,
    comparativaGalpones: comparativaGalpones.data || [],
    isLoadingGalpones: comparativaGalpones.isLoading,
  };
}

export function useProyeccionSacrificio(loteId?: string) {
  return useQuery({
    queryKey: ['dashboard', 'proyeccion', loteId],
    queryFn: () => api.get<ProyeccionSacrificio>(`/api/Dashboard/proyeccion-sacrificio/${loteId}`),
    enabled: !!loteId,
    retry: false, // Evitar reintentos si falla con 404
  });
}
