import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { api } from '@/lib/api';

export interface ConfiguracionSistema {
  id: string;
  nombreEmpresa: string;
  ruc: string;
  direccion: string;
  telefono: string;
  correoContacto: string;
  moneda: string;
  ivaPorcentaje: number;
  umbralAlertaStockBajo: number;
  diasProyeccionStock: number;
}

export function useConfiguracion() {
  const queryClient = useQueryClient();

  const configuracion = useQuery({
    queryKey: ['configuracion'],
    queryFn: () => api.get<ConfiguracionSistema>('/api/Configuracion'),
  });

  const actualizarConfiguracion = useMutation({
    mutationFn: (data: ConfiguracionSistema) => api.post('/api/Configuracion', data),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['configuracion'] }),
  });

  return {
    configuracion: configuracion.data,
    isLoading: configuracion.isLoading,
    actualizarConfiguracion,
  };
}
