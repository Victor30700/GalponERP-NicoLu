import React, { useState } from 'react';
import { FileDown, Loader2 } from 'lucide-react';
import { downloadBlob } from '@/lib/utils/download-helper';
import { toast } from 'sonner';

interface ReportButtonProps {
  url: string;
  fileName: string;
  label: string;
  className?: string;
}

export const ReportButton: React.FC<ReportButtonProps> = ({ url, fileName, label, className }) => {
  const [loading, setLoading] = useState(false);

  const handleDownload = async () => {
    setLoading(true);
    try {
      await downloadBlob(url, fileName);
      toast.success('Reporte generado correctamente');
    } catch (error) {
      toast.error('No se pudo generar el reporte. Verifique su conexión.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <button
      onClick={handleDownload}
      disabled={loading}
      className={`flex items-center gap-2 px-3 py-2 rounded-md transition-all 
        ${loading ? 'bg-muted text-muted-foreground' : 'bg-primary/10 text-primary hover:bg-primary/20'} 
        ${className}`}
    >
      {loading ? (
        <Loader2 className="h-4 w-4 animate-spin" />
      ) : (
        <FileDown className="h-4 w-4" />
      )}
      <span className="text-sm font-medium">{label}</span>
    </button>
  );
};
