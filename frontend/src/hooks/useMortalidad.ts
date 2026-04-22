import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { api } from '@/lib/api';

export interface MortalidadItem {
  id: string;
  loteId: string;
  fecha: string;
  cantidadBajas: number;
  causa: string;
  version?: string;
}

export interface RegistrarMortalidadForm {
  loteId: string;
  cantidad: number;
  causa: string;
  fecha: string;
}

export interface ActualizarMortalidadForm {
  id: string;
  cantidad: number;
  causa: string;
  fecha: string;
  version?: string;
}

export function useMortalidad(loteId?: string) {
  const queryClient = useQueryClient();

  const mortalidad = useQuery({
    queryKey: ['mortalidad', 'lote', loteId],
    queryFn: () => api.get<MortalidadItem[]>(`/api/Mortalidad/lote/${loteId}`),
    enabled: !!loteId,
  });

  const registrarMortalidad = useMutation({
    mutationFn: ({ data, idempotencyKey }: { data: RegistrarMortalidadForm; idempotencyKey?: string }) => 
      api.post('/api/Mortalidad', data, idempotencyKey),
    onMutate: async ({ data: newMortalidad }) => {
      // Cancelar cualquier refetch en curso para que no sobreescriba nuestro estado optimista
      await queryClient.cancelQueries({ queryKey: ['mortalidad', 'lote', loteId] });

      // Capturar el estado anterior del caché
      const previousMortalidad = queryClient.getQueryData<MortalidadItem[]>(['mortalidad', 'lote', loteId]);

      // Actualizar el caché de forma optimista
      if (previousMortalidad) {
        queryClient.setQueryData<MortalidadItem[]>(['mortalidad', 'lote', loteId], [
          ...previousMortalidad,
          {
            id: 'temp-id-' + Date.now(),
            loteId: newMortalidad.loteId,
            fecha: newMortalidad.fecha,
            cantidadBajas: newMortalidad.cantidad,
            causa: newMortalidad.causa,
          } as MortalidadItem
        ]);
      }

      // Devolver el contexto con el estado anterior para el rollback
      return { previousMortalidad };
    },
    onError: (err, variables, context) => {
      // Si la mutación falla, revertir al estado anterior
      if (context?.previousMortalidad) {
        queryClient.setQueryData(['mortalidad', 'lote', loteId], context.previousMortalidad);
      }
    },
    onSettled: () => {
      // Invalidar consultas para asegurar que los datos estén sincronizados con el servidor
      queryClient.invalidateQueries({ queryKey: ['mortalidad', 'lote', loteId] });
      queryClient.invalidateQueries({ queryKey: ['lote', loteId] });
    },
  });

  const actualizarMortalidad = useMutation({
    mutationFn: ({ data, idempotencyKey }: { data: ActualizarMortalidadForm; idempotencyKey?: string }) => 
      api.put(`/api/Mortalidad/${data.id}`, data, idempotencyKey),
    onMutate: async ({ data: updatedItem }) => {
      // Cancelar cualquier refetch en curso
      await queryClient.cancelQueries({ queryKey: ['mortalidad', 'lote', loteId] });

      // Capturar el estado anterior del caché
      const previousMortalidad = queryClient.getQueryData<MortalidadItem[]>(['mortalidad', 'lote', loteId]);

      // Actualizar el caché de forma optimista
      if (previousMortalidad) {
        queryClient.setQueryData<MortalidadItem[]>(
          ['mortalidad', 'lote', loteId],
          previousMortalidad.map((item) =>
            item.id === updatedItem.id
              ? { ...item, cantidadBajas: updatedItem.cantidad, causa: updatedItem.causa, fecha: updatedItem.fecha }
              : item
          )
        );
      }

      // Devolver el contexto con el estado anterior para el rollback
      return { previousMortalidad };
    },
    onError: (err, variables, context) => {
      if (context?.previousMortalidad) {
        queryClient.setQueryData(['mortalidad', 'lote', loteId], context.previousMortalidad);
      }
    },
    onSettled: () => {
      queryClient.invalidateQueries({ queryKey: ['mortalidad', 'lote', loteId] });
      queryClient.invalidateQueries({ queryKey: ['lote', loteId] });
    },
  });

  const eliminarMortalidad = useMutation({
    mutationFn: (id: string) => api.delete(`/api/Mortalidad/${id}`),
    onMutate: async (idToRemove) => {
      // Cancelar cualquier refetch en curso
      await queryClient.cancelQueries({ queryKey: ['mortalidad', 'lote', loteId] });

      // Capturar el estado anterior del caché
      const previousMortalidad = queryClient.getQueryData<MortalidadItem[]>(['mortalidad', 'lote', loteId]);

      // Actualizar el caché de forma optimista
      if (previousMortalidad) {
        queryClient.setQueryData<MortalidadItem[]>(
          ['mortalidad', 'lote', loteId],
          previousMortalidad.filter((item) => item.id !== idToRemove)
        );
      }

      // Devolver el contexto con el estado anterior para el rollback
      return { previousMortalidad };
    },
    onError: (err, idToRemove, context) => {
      if (context?.previousMortalidad) {
        queryClient.setQueryData(['mortalidad', 'lote', loteId], context.previousMortalidad);
      }
    },
    onSettled: () => {
      queryClient.invalidateQueries({ queryKey: ['mortalidad', 'lote', loteId] });
      queryClient.invalidateQueries({ queryKey: ['lote', loteId] });
    },
  });

  return {
    mortalidad: mortalidad.data || [],
    isLoading: mortalidad.isLoading,
    registrarMortalidad,
    actualizarMortalidad,
    eliminarMortalidad,
  };
}
