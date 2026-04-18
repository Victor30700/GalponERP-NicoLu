"use client";

import { useState } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import { 
  Receipt, Plus, Search, Filter, 
  Trash2, Edit, Calendar, DollarSign, 
  Warehouse, Bird, Save, X, Tag
} from 'lucide-react';
import { useGastos, GastoRequest } from '@/hooks/useGastos';
import { useQuery } from '@tanstack/react-query';
import { api } from '@/lib/api';
import { UniversalGrid } from '@/components/shared/UniversalGrid';
import { confirmDestructiveAction } from '@/lib/swal';
import { cn } from '@/lib/utils';
import { useAuth } from '@/context/AuthContext';
import { UserRole } from '@/lib/rbac';

export default function GastosPage() {
  const { profile } = useAuth();
  const userRole = profile?.rol !== undefined ? Number(profile.rol) : null;
  const isEmpleado = userRole === UserRole.Empleado;

  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingGasto, setEditingGasto] = useState<any>(null);
  const { gastos, isLoading, createGasto, updateGasto, deleteGasto } = useGastos();

  // Queries para selects
  const { data: galpones = [] } = useQuery({
    queryKey: ['galpones-select'],
    queryFn: () => api.get<any[]>('/api/Galpones')
  });

  const { data: lotes = [] } = useQuery({
    queryKey: ['lotes-activos-select'],
    queryFn: () => api.get<any[]>('/api/Lotes?soloActivos=true')
  });

  const [formData, setFormData] = useState<GastoRequest>({
    galponId: '',
    loteId: '',
    descripcion: '',
    monto: 0,
    fecha: new Date().toISOString().split('T')[0],
    tipoGasto: ''
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (editingGasto) {
      updateGasto.mutate({ id: editingGasto.id, data: formData }, {
        onSuccess: () => closeModal()
      });
    } else {
      createGasto.mutate(formData, {
        onSuccess: () => closeModal()
      });
    }
  };

  const openModal = (gasto?: any) => {
    if (gasto) {
      setEditingGasto(gasto);
      const montoVal = typeof gasto.monto === 'object' ? gasto.monto.monto : gasto.monto;
      setFormData({
        galponId: gasto.galponId,
        loteId: gasto.loteId,
        descripcion: gasto.descripcion,
        monto: montoVal,
        fecha: gasto.fecha.split('T')[0],
        tipoGasto: gasto.tipoGasto
      });
    } else {
      setEditingGasto(null);
      setFormData({
        galponId: '',
        loteId: '',
        descripcion: '',
        monto: 0,
        fecha: new Date().toISOString().split('T')[0],
        tipoGasto: ''
      });
    }
    setIsModalOpen(true);
  };

  const closeModal = () => {
    setIsModalOpen(false);
    setEditingGasto(null);
  };

  const handleDelete = async (id: string) => {
    const result = await confirmDestructiveAction('¿Eliminar gasto?', 'Esta acción no se puede deshacer.');
    if (result.isConfirmed) {
      deleteGasto.mutate(id);
    }
  };

  return (
    <div className="space-y-8">
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
        <div>
          <h1 className="text-3xl font-black text-foreground flex items-center gap-3">
            <Receipt size={32} className="text-primary" />
            Control de Gastos
          </h1>
          <p className="text-muted-foreground mt-1">Registra egresos operativos, insumos y mantenimiento.</p>
        </div>

        {!isEmpleado && (
          <button 
            onClick={() => openModal()}
            className="flex items-center gap-2 px-6 py-4 bg-primary text-black font-black rounded-2xl text-xs uppercase tracking-widest hover:scale-105 transition-all shadow-lg shadow-primary/20"
          >
            <Plus size={18} /> Nuevo Gasto
          </button>
        )}
      </div>

      <UniversalGrid
        title="Historial de Egresos"
        items={gastos}
        isLoading={isLoading}
        searchPlaceholder="Buscar por descripción o tipo..."
        onEdit={isEmpleado ? undefined : (item) => openModal(item)}
        onDelete={isEmpleado ? undefined : (item) => handleDelete(item.id)}
        columns={[
          { 
            header: 'Descripción', 
            accessor: (item) => (
              <div className="flex flex-col">
                <span className="font-bold text-foreground">{item.descripcion}</span>
                <span className="text-[10px] text-muted-foreground uppercase font-black">{item.tipoGasto}</span>
              </div>
            ) 
          },
          { 
            header: 'Monto', 
            accessor: (item) => {
              const val = typeof item.monto === 'object' ? item.monto.monto : item.monto;
              return <span className="font-black text-foreground">Bs. {val.toLocaleString()}</span>;
            }
          },
          { header: 'Fecha', accessor: (item) => new Date(item.fecha).toLocaleDateString() },
          { 
            header: 'Asignación', 
            accessor: (item) => (
              <div className="flex flex-col text-[10px] uppercase font-bold text-muted-foreground">
                <span>Galpón: {galpones.find((g: any) => g.id === item.galponId)?.nombre || 'N/A'}</span>
                <span>Lote: {lotes.find((l: any) => l.id === item.loteId)?.nombre || lotes.find((l: any) => l.id === item.loteId)?.nombreLote || 'Gral.'}</span>
              </div>
            )
          }
        ]}
        renderMobileCard={(item) => {
          const val = typeof item.monto === 'object' ? item.monto.monto : item.monto;
          return (
            <div className="space-y-3">
              <div className="flex justify-between items-start">
                <div>
                  <h3 className="font-bold text-foreground">{item.descripcion}</h3>
                  <span className="px-2 py-0.5 bg-muted/50 rounded text-[8px] font-black text-primary uppercase tracking-widest">{item.tipoGasto}</span>
                </div>
                <span className="text-lg font-black text-foreground">Bs. {val.toLocaleString()}</span>
              </div>
              <div className="flex items-center gap-4 text-[10px] text-muted-foreground font-bold uppercase">
                <div className="flex items-center gap-1"><Calendar size={12} /> {new Date(item.fecha).toLocaleDateString()}</div>
                <div className="flex items-center gap-1"><Warehouse size={12} /> {galpones.find((g: any) => g.id === item.galponId)?.nombre}</div>
              </div>
            </div>
          );
        }}
      />

      {/* Modal de Registro/Edición */}
      <AnimatePresence>
        {isModalOpen && (
          <>
            <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} onClick={closeModal} className="fixed inset-0 bg-black/80 backdrop-blur-md z-[120]" />
            <motion.div initial={{ opacity: 0, scale: 0.95, y: 20 }} animate={{ opacity: 1, scale: 1, y: 0 }} exit={{ opacity: 0, scale: 0.95, y: 20 }} className="fixed inset-0 m-auto w-full max-w-lg h-fit glass z-[130] p-8 rounded-[2.5rem] border border-border shadow-2xl">
              <div className="flex items-center justify-between mb-8">
                <div>
                  <h2 className="text-2xl font-black text-foreground uppercase tracking-widest">{editingGasto ? 'Editar Gasto' : 'Nuevo Gasto'}</h2>
                  <p className="text-[10px] font-bold text-muted-foreground uppercase tracking-tighter mt-1">Complete los detalles del egreso</p>
                </div>
                <button onClick={closeModal} className="p-3 bg-muted/50 hover:bg-muted/50 rounded-2xl text-muted-foreground transition-all"><X size={24} /></button>
              </div>

              <form onSubmit={handleSubmit} className="space-y-5">
                <div className="space-y-2">
                  <label className="text-[10px] font-black text-muted-foreground uppercase tracking-widest ml-1">Descripción</label>

                  <input 
                    required 
                    type="text" 
                    value={formData.descripcion} 
                    onChange={(e) => setFormData({...formData, descripcion: e.target.value})}
                    placeholder="Ej: Mantenimiento techos, Compra viruta..."
                    className="w-full px-5 py-4 bg-muted/50 border border-border rounded-2xl text-foreground font-medium focus:outline-none focus:ring-2 focus:ring-primary/50"
                  />
                </div>

                <div className="grid grid-cols-2 gap-4">
                  <div className="space-y-2">
                    <label className="text-[10px] font-black text-muted-foreground uppercase tracking-widest ml-1">Monto (Bs.)</label>

                    <div className="relative">
                      <DollarSign className="absolute left-4 top-1/2 -translate-y-1/2 text-muted-foreground" size={18} />
                      <input 
                        required 
                        type="number" 
                        step="0.01" 
                        value={formData.monto} 
                        onChange={(e) => setFormData({...formData, monto: Number(e.target.value)})}
                        className="w-full pl-12 pr-5 py-4 bg-muted/50 border border-border rounded-2xl text-foreground font-bold"
                      />
                    </div>
                  </div>
                  <div className="space-y-2">
                    <label className="text-[10px] font-black text-muted-foreground uppercase tracking-widest ml-1">Fecha</label>

                    <input 
                      required 
                      type="date" 
                      value={formData.fecha} 
                      onChange={(e) => setFormData({...formData, fecha: e.target.value})}
                      className="w-full px-5 py-4 bg-muted/50 border border-border rounded-2xl text-foreground font-medium focus:ring-2 focus:ring-primary/50"
                    />
                  </div>
                </div>

                <div className="space-y-2">
                  <label className="text-[10px] font-black text-muted-foreground uppercase tracking-widest ml-1">Tipo de Gasto</label>

                  <div className="relative">
                    <Tag className="absolute left-4 top-1/2 -translate-y-1/2 text-muted-foreground" size={18} />
                    <input 
                      required 
                      type="text" 
                      value={formData.tipoGasto} 
                      onChange={(e) => setFormData({...formData, tipoGasto: e.target.value})}
                      placeholder="Ej: Mantenimiento, Insumos, Servicios"
                      className="w-full pl-12 pr-5 py-4 bg-muted/50 border border-border rounded-2xl text-foreground font-medium"
                    />
                  </div>
                </div>

                <div className="grid grid-cols-2 gap-4">
                  <div className="space-y-2">
                    <label className="text-[10px] font-black text-muted-foreground uppercase tracking-widest ml-1">Galpón</label>

                    <select 
                      required 
                      value={formData.galponId} 
                      onChange={(e) => setFormData({...formData, galponId: e.target.value})}
                      className="w-full px-5 py-4 bg-muted/50 border border-border rounded-2xl text-foreground appearance-none"
                    >
                      <option value="" className="bg-muted/50">Seleccionar...</option>
                      {galpones.map((g: any) => <option key={g.id} value={g.id} className="bg-muted/50">{g.nombre}</option>)}
                    </select>
                  </div>
                  <div className="space-y-2">
                    <label className="text-[10px] font-black text-muted-foreground uppercase tracking-widest ml-1">Lote</label>

                    <select 
                      value={formData.loteId} 
                      onChange={(e) => setFormData({...formData, loteId: e.target.value})}
                      className="w-full px-5 py-4 bg-muted/50 border border-border rounded-2xl text-foreground appearance-none"
                    >
                      <option value="" className="bg-muted/50">General / Ninguno</option>
                      {lotes.map((l: any) => <option key={l.id} value={l.id} className="bg-muted/50">{l.nombre || l.nombreLote}</option>)}
                    </select>
                  </div>
                </div>

                <button 
                  type="submit" 
                  disabled={createGasto.isPending || updateGasto.isPending}
                  className="w-full py-5 bg-primary text-black font-black rounded-2xl uppercase tracking-[0.2em] shadow-lg shadow-primary/20 hover:scale-[1.02] active:scale-[0.98] transition-all disabled:opacity-50 mt-4"
                >
                  <Save size={20} className="inline-block mr-2" />
                  {editingGasto ? 'Guardar Cambios' : 'Registrar Gasto'}
                </button>
              </form>
            </motion.div>
          </>
        )}
      </AnimatePresence>
    </div>
  );
}


