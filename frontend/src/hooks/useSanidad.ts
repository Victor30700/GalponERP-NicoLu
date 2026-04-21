import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { api } from '@/lib/api';

export interface BienestarRequest {
  loteId: string;
  fecha: string;
  temperatura: number;
  humedad: number;
  consumoAgua: number;
  lecturaMedidor?: number;
  observaciones: string;
}

export interface BienestarItem {
  id: string;
  loteId: string;
  fecha: string;
  temperatura: number | null;
  humedad: number | null;
  consumoAgua: number | null;
  lecturaMedidor: number | null;
  observaciones: string | null;
}

export function useSanidad(loteId?: string) {
  const queryClient = useQueryClient();

  const historialBienestar = useQuery({
    queryKey: ['sanidad', 'bienestar', 'lote', loteId],
    queryFn: () => api.get<BienestarItem[]>(`/api/Sanidad/lote/${loteId}/bienestar`),
    enabled: !!loteId,
  });

  const registrarBienestar = useMutation({
    mutationFn: (data: BienestarRequest) => api.post<{ registroId: string }>('/api/Sanidad/bienestar', data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['sanidad'] });
      queryClient.invalidateQueries({ queryKey: ['lotes'] });
      if (loteId) {
        queryClient.invalidateQueries({ queryKey: ['sanidad', 'bienestar', 'lote', loteId] });
      }
    },
  });

  const eliminarBienestar = useMutation({
    mutationFn: (id: string) => api.delete(`/api/Sanidad/bienestar/${id}`),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['sanidad'] });
      if (loteId) {
        queryClient.invalidateQueries({ queryKey: ['sanidad', 'bienestar', 'lote', loteId] });
      }
    },
  });

  const actualizarBienestar = useMutation({
    mutationFn: (data: any) => api.put(`/api/Sanidad/bienestar/${data.id}`, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['sanidad'] });
      if (loteId) {
        queryClient.invalidateQueries({ queryKey: ['sanidad', 'bienestar', 'lote', loteId] });
      }
    },
  });

  return {
    historialBienestar: historialBienestar.data || [],
    isLoadingHistorial: historialBienestar.isLoading,
    registrarBienestar,
    eliminarBienestar,
    actualizarBienestar,
    descargarReporteBienestar: (registroId: string) => {
      const baseUrl = process.env.NEXT_PUBLIC_API_URL || '';
      window.open(`${baseUrl}/api/Sanidad/reportes/bienestar/${registroId}`, '_blank');
    }
  };
}
