import { api } from '../api';

/**
 * Descarga un archivo binario (Blob) desde una URL protegida.
 * @param url URL del endpoint que retorna el archivo.
 * @param fileName Nombre sugerido para el archivo descargado.
 */
export const downloadBlob = async (url: string, fileName: string) => {
  try {
    const blob = await api.getBlob(url);
    
    // Crear un objeto URL para el blob
    const blobUrl = window.URL.createObjectURL(blob);
    
    // Crear un link temporal y simular click
    const link = document.createElement('a');
    link.href = blobUrl;
    link.setAttribute('download', fileName);
    document.body.appendChild(link);
    link.click();
    
    // Limpieza
    document.body.removeChild(link);
    window.URL.revokeObjectURL(blobUrl);
  } catch (error) {
    console.error('Error al descargar el archivo:', error);
    throw error;
  }
};
