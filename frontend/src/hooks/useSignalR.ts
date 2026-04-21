import { useEffect, useState } from 'react';
import * as signalR from '@microsoft/signalr';
import { toast } from 'sonner';

export function useSignalR() {
  const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
  const [notifications, setNotifications] = useState<any[]>([]);

  useEffect(() => {
    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5167'}/notificationHub`, {
        accessTokenFactory: () => localStorage.getItem('idToken') || '',
        skipNegotiation: false,
        transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.LongPolling
      })
      .withAutomaticReconnect()
      .build();

    setConnection(newConnection);
  }, []);

  useEffect(() => {
    if (connection) {
      connection.start()
        .then(() => {
          console.log('Connected to SignalR Hub');

          connection.on('ReceiveNotification', (user, message) => {
            console.log('Notification received:', { user, message });

            // Mostrar Toast visual
            toast(user, {
              description: message,
              duration: 8000,
            });

            setNotifications(prev => [...prev, { user, message, date: new Date() }]);
          });
        })
        .catch(error => console.error('SignalR Connection Error: ', error));
    }

    return () => {
      if (connection) {
        connection.off('ReceiveNotification');
      }
    };
  }, [connection]);

  return { connection, notifications };
}
