import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { api } from '@/lib/api';
import { toast } from 'sonner';

export enum EstadoCalendario {
  Pendiente = 0,
  Aplicado = 1,
  Reprogramado = 2,
  Cancelado = 3
}

export enum TipoActividad {
  Otro = 0,
  Vacuna = 1,
  Tratamiento = 2,
  Control = 3,
  Limpieza = 4,
  Sanidad = 5
}

export interface ActividadCalendario {
  id: string;
  loteId: string;
  diaDeAplicacion: number;
  descripcionTratamiento: string;
  productoIdRecomendado: string | null;
  estado: EstadoCalendario;
  tipo: TipoActividad;
  esManual: boolean;
  justificacion: string | null;
  isActive: boolean;
  fechaCreacion: string;
  fechaProgramada?: string; // Para manuales o reprogramadas
}

export interface AplicarActividadRequest {
  cantidadConsumida?: number;
}

export interface ActividadManualRequest {
  loteId: string;
  tipo: TipoActividad;
  fechaProgramada: string;
  descripcion: string;
  productoId?: string;
}

export interface ReprogramarActividadRequest {
  nuevaFecha: string;
  justificacion: string;
}

export function useCalendarioSanitario(loteId?: string) {
  const queryClient = useQueryClient();

  const { data: calendario = [], isLoading } = useQuery({
    queryKey: ['calendario-sanitario', loteId],
    queryFn: () => api.get<ActividadCalendario[]>(`/api/CalendarioSanitario/${loteId}`),
    enabled: !!loteId,
  });

  const aplicarActividad = useMutation({
    mutationFn: ({ id, data }: { id: string, data: AplicarActividadRequest }) => 
      api.patch(`/api/CalendarioSanitario/${id}/aplicar`, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['calendario-sanitario', loteId] });
      toast.success('Actividad aplicada correctamente');
    },
    onError: (error: any) => {
      toast.error(`Error al aplicar actividad: ${error.message}`);
    }
  });

  const agregarActividadManual = useMutation({
    mutationFn: (data: ActividadManualRequest) => 
      api.post<{ actividadId: string }>('/api/CalendarioSanitario/actividad-manual', data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['calendario-sanitario', loteId] });
      toast.success('Actividad manual agregada');
    },
    onError: (error: any) => {
      toast.error(`Error al agregar actividad: ${error.message}`);
    }
  });

  const reprogramarActividad = useMutation({
    mutationFn: ({ id, data }: { id: string, data: ReprogramarActividadRequest }) => 
      api.put(`/api/CalendarioSanitario/${id}/reprogramar`, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['calendario-sanitario', loteId] });
      toast.success('Actividad reprogramada');
    },
    onError: (error: any) => {
      toast.error(`Error al reprogramar actividad: ${error.message}`);
    }
  });

  return {
    calendario,
    isLoading,
    aplicarActividad,
    agregarActividadManual,
    reprogramarActividad
  };
}
