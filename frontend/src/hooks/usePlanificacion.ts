import { useQuery } from '@tanstack/react-query';
import { api } from '@/lib/api';

export interface SimulacionParams {
  cantidadPollos: number;
  pesoEsperadoPorPolloKg: number;
  precioAlimentoPorKg: number;
  precioVentaPorKg: number;
  fcrPersonalizado?: number;
}

export interface DetalleEtapa {
  etapa: string;
  diasInicio: number;
  diasFin: number;
  porcentajeConsumo: number;
  consumoKg: number;
  costoEstimado: number;
}

export interface SimulacionResultado {
  cantidadPollos: number;
  pesoEsperadoTotalKg: number;
  alimentoTotalKg: number;
  costoAlimentoTotal: number;
  ingresosProyectados: number;
  utilidadBrutaProyectada: number;
  detallesEtapas: DetalleEtapa[];
}

export function useSimulacion(params: SimulacionParams, enabled = false) {
  const queryParams = new URLSearchParams({
    CantidadPollos: params.cantidadPollos.toString(),
    PesoEsperadoPorPolloKg: params.pesoEsperadoPorPolloKg.toString(),
    PrecioAlimentoPorKg: params.precioAlimentoPorKg.toString(),
    PrecioVentaPorKg: params.precioVentaPorKg.toString(),
  });

  if (params.fcrPersonalizado) {
    queryParams.append('FcrPersonalizado', params.fcrPersonalizado.toString());
  }

  return useQuery({
    queryKey: ['planificacion', 'simulacion', params],
    queryFn: () => api.get<SimulacionResultado>(`/api/Planificacion/simulacion?${queryParams.toString()}`),
    enabled: enabled && !!params.cantidadPollos && params.cantidadPollos > 0,
  });
}
