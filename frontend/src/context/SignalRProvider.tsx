'use client';

import React, { createContext, useContext, useEffect, useState } from 'react';
import * as signalR from '@microsoft/signalr';
import { useQueryClient } from '@tanstack/react-query';
import { useAuth } from './AuthContext';
import { toast } from 'sonner';

interface SignalRContextType {
  connection: signalR.HubConnection | null;
  isConnected: boolean;
}

const SignalRContext = createContext<SignalRContextType>({
  connection: null,
  isConnected: false,
});

export const useSignalR = () => useContext(SignalRContext);

const BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5167';

export const SignalRProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
  const [isConnected, setIsConnected] = useState(false);
  const queryClient = useQueryClient();
  const { user } = useAuth();

  useEffect(() => {
    if (!user) {
      if (connection) {
        connection.stop();
        setConnection(null);
        setIsConnected(false);
      }
      return;
    }

    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${BASE_URL}/notificationHub`, {
        accessTokenFactory: () => localStorage.getItem('idToken') || '',
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Information)
      .build();

    setConnection(newConnection);
  }, [user]);

  useEffect(() => {
    if (connection) {
      connection
        .start()
        .then(() => {
          setIsConnected(true);
          console.log('SignalR Connected');

          connection.on('ReceiveNotification', (category: string, message: string) => {
            console.log(`Notification received [${category}]: ${message}`);
            
            // Mapeo de eventos a invalidaciones de caché de TanStack Query
            switch (category) {
              case 'Lote':
                queryClient.invalidateQueries({ queryKey: ['lotes'] });
                queryClient.invalidateQueries({ queryKey: ['lote'] });
                break;
              case 'Inventario':
                queryClient.invalidateQueries({ queryKey: ['inventario'] });
                queryClient.invalidateQueries({ queryKey: ['movimientos'] });
                break;
              case 'Finanzas':
                queryClient.invalidateQueries({ queryKey: ['ventas'] });
                queryClient.invalidateQueries({ queryKey: ['pagos'] });
                queryClient.invalidateQueries({ queryKey: ['dashboard'] });
                break;
              case 'Sistema':
                // Notificaciones generales del sistema (ej. Alertas)
                toast.info(message);
                break;
              default:
                break;
            }

            // Si el mensaje es un evento específico, podemos ser más granulares
            if (message === 'LoteActualizado') {
              toast.success('Datos del lote actualizados en tiempo real');
            }
          });
        })
        .catch((err) => console.log('SignalR Connection Error: ', err));

      return () => {
        connection.off('ReceiveNotification');
        connection.stop()
          .then(() => console.log('SignalR Disconnected'))
          .catch(err => console.error('Error stopping SignalR connection: ', err));
      };
    }
  }, [connection, queryClient]);

  return (
    <SignalRContext.Provider value={{ connection, isConnected }}>
      {children}
    </SignalRContext.Provider>
  );
};
