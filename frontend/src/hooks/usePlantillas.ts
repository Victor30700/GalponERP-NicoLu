import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { api } from '@/lib/api';

export interface TareaPlantilla {
  id: string;
  diaProgramado: number;
  descripcion: string;
  tipoAccion: string;
  productoId?: string;
  dosis?: string;
}

export interface PlantillaSanitaria {
  id: string;
  nombre: string;
  descripcion: string;
  tareas: TareaPlantilla[];
}

export interface CrearPlantillaRequest {
  nombre: string;
  descripcion: string;
  tareas: Omit<TareaPlantilla, 'id'>[];
}

export function usePlantillas() {
  const queryClient = useQueryClient();

  const plantillas = useQuery({
    queryKey: ['plantillas'],
    queryFn: () => api.get<PlantillaSanitaria[]>('/api/Plantillas'),
  });

  const crearPlantilla = useMutation({
    mutationFn: (data: CrearPlantillaRequest) => api.post('/api/Plantillas', data),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['plantillas'] }),
  });

  return {
    plantillas: plantillas.data || [],
    isLoading: plantillas.isLoading,
    crearPlantilla,
    refresh: () => plantillas.refetch(),
  };
}
