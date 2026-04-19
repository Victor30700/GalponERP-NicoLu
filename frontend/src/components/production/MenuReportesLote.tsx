import React from 'react';
import { FileText, ChevronDown } from 'lucide-react';
import { ReportButton } from '../shared/ReportButton';

interface MenuReportesLoteProps {
  loteId: string;
}

export const MenuReportesLote: React.FC<MenuReportesLoteProps> = ({ loteId }) => {
  return (
    <div className="flex flex-col gap-3 p-4 bg-card rounded-lg border shadow-sm">
      <div className="flex items-center gap-2 mb-2">
        <FileText className="h-5 w-5 text-primary" />
        <h3 className="font-semibold text-lg">Formatos SAVCO</h3>
      </div>
      
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-3">
        <ReportButton 
          label="SAVCO-01 Ingreso" 
          url={`/api/lotes/${loteId}/reportes/ingreso`} 
          fileName={`SAVCO-01_Ingreso_${loteId}.pdf`}
        />
        <ReportButton 
          label="SAVCO-02 Mortalidad" 
          url={`/api/lotes/${loteId}/reportes/mortalidad`} 
          fileName={`SAVCO-02_Mortalidad_${loteId}.pdf`}
        />
        <ReportButton 
          label="SAVCO-03 Ficha Semanal" 
          url={`/api/lotes/${loteId}/reportes/semanal`} 
          fileName={`SAVCO-03_Semanal_${loteId}.pdf`}
        />
        <ReportButton 
          label="SAVCO-04 Consumo" 
          url={`/api/lotes/${loteId}/reportes/consumo`} 
          fileName={`SAVCO-04_Consumo_${loteId}.pdf`}
        />
        <ReportButton 
          label="SAVCO-05 Sanidad" 
          url={`/api/sanidad/reportes/calendario/${loteId}`} 
          fileName={`SAVCO-05_Sanidad_${loteId}.pdf`}
        />
        <ReportButton 
          label="SAVCO-09 Liquidación" 
          url={`/api/lotes/${loteId}/reportes/liquidacion`} 
          fileName={`SAVCO-09_Liquidacion_${loteId}.pdf`}
        />
      </div>
    </div>
  );
};
