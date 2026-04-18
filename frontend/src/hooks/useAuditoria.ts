import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { api } from '@/lib/api';

export interface AuditoriaLog {
  id: string;
  usuarioId: string;
  usuarioNombre: string;
  accion: string;
  entidad: string;
  entidadNombre: string;
  entidadId: string;
  fecha: string;
  detalles: string;
  detallesJSON: string;
}

export function useAuditoria(filters?: { desde?: string; hasta?: string; usuarioId?: string; entidad?: string }) {
  const queryClient = useQueryClient();

  const queryParams = new URLSearchParams();
  if (filters?.desde) queryParams.append('desde', filters.desde);
  if (filters?.hasta) queryParams.append('hasta', filters.hasta);
  if (filters?.usuarioId) queryParams.append('usuarioId', filters.usuarioId);
  if (filters?.entidad) queryParams.append('entidad', filters.entidad);

  const logs = useQuery({
    queryKey: ['auditoria', 'logs', filters],
    queryFn: () => api.get<AuditoriaLog[]>(`/api/Auditoria/logs?${queryParams.toString()}`),
  });

  const restaurarEntidad = useMutation({
    mutationFn: ({ entidad, id }: { entidad: string; id: string }) => 
      api.patch(`/api/Auditoria/restaurar/${entidad}/${id}`, {}),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['auditoria'] });
      // Invalidar todas las queries para asegurar consistencia tras restauración
      queryClient.invalidateQueries();
    },
  });

  return {
    logs: logs.data || [],
    isLoading: logs.isLoading,
    restaurarEntidad,
  };
}
