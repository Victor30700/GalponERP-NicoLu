import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { api } from '@/lib/api';

export interface ChatMessage {
  id: string;
  rol: string;
  contenido: string;
  fecha: string;
}

export interface Conversacion {
  id: string;
  titulo: string;
  fechaInicio: string;
  ultimoResumen: string | null;
  totalMensajes: number;
}

export interface ChatResponse {
  respuesta: string;
  conversacionId: string;
}

export interface VoiceResponse {
  transcripcion: string;
  respuestaTexto: string;
  respuestaAudioBase64: string;
  conversacionId: string;
}

const EMPTY_ARRAY: any[] = [];

export function useAgentes() {
  const queryClient = useQueryClient();

  const conversaciones = useQuery({
    queryKey: ['agentes', 'conversaciones'],
    queryFn: () => api.get<Conversacion[]>('/api/Agentes/conversaciones'),
  });

  const enviarMensaje = useMutation({
    mutationFn: (data: { mensaje: string; conversacionId?: string }) => 
      api.post<ChatResponse>('/api/Agentes/chat', data),
    onSuccess: (data) => {
      queryClient.invalidateQueries({ queryKey: ['agentes', 'conversaciones'] });
      if (data.conversacionId) {
        queryClient.invalidateQueries({ queryKey: ['agentes', 'conversacion', data.conversacionId] });
      }
    },
  });

  const uploadVoice = useMutation({
    mutationFn: async (data: { audio: Blob; conversacionId?: string }) => {
      const formData = new FormData();
      formData.append('Audio', data.audio, 'voice.webm');
      if (data.conversacionId) formData.append('ConversacionId', data.conversacionId);

      const response = await fetch(`${process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5167'}/api/voice/upload`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('idToken')}`
        },
        body: formData
      });

      if (!response.ok) throw new Error('Error al subir audio');
      return response.json() as Promise<VoiceResponse>;
    },
    onSuccess: (data) => {
      queryClient.invalidateQueries({ queryKey: ['agentes', 'conversaciones'] });
      if (data.conversacionId) {
        queryClient.invalidateQueries({ queryKey: ['agentes', 'conversacion', data.conversacionId] });
      }
    }
  });

  const eliminarConversacion = useMutation({
    mutationFn: (id: string) => api.delete(`/api/Agentes/conversaciones/${id}`),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['agentes', 'conversaciones'] }),
  });

  return {
    conversaciones: conversaciones.data || (EMPTY_ARRAY as Conversacion[]),
    isLoadingConversaciones: conversaciones.isLoading,
    enviarMensaje,
    uploadVoice,
    eliminarConversacion,
  };
}

export function useConversacion(id: string) {
  const historial = useQuery({
    queryKey: ['agentes', 'conversacion', id],
    queryFn: () => api.get<{ mensajes: ChatMessage[] }>(`/api/Agentes/conversaciones/${id}`),
    enabled: !!id,
  });

  return {
    mensajes: historial.data?.mensajes || (EMPTY_ARRAY as ChatMessage[]),
    isLoading: historial.isLoading,
  };
}
