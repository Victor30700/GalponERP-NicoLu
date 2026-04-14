import { useState } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import { X, Calendar as CalendarIcon, ClipboardCheck, Package } from 'lucide-react';
import { useCalendarioSanitario, TipoActividad } from '@/hooks/useCalendarioSanitario';
import { useCatalogos } from '@/hooks/useCatalogos';

interface ManualActivityModalProps {
  isOpen: boolean;
  onClose: () => void;
  loteId: string;
}

export function ManualActivityModal({ isOpen, onClose, loteId }: ManualActivityModalProps) {
  const { agregarActividadManual } = useCalendarioSanitario(loteId);
  const { productos } = useCatalogos();
  const [formData, setFormData] = useState({
    descripcion: '',
    tipo: TipoActividad.Sanidad,
    fechaProgramada: new Date().toISOString().split('T')[0],
    productoId: ''
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    agregarActividadManual.mutate({
      loteId,
      descripcion: formData.descripcion,
      tipo: formData.tipo,
      fechaProgramada: new Date(formData.fechaProgramada).toISOString(),
      productoId: formData.productoId || undefined
    }, {
      onSuccess: () => {
        onClose();
        setFormData({
          descripcion: '',
          tipo: TipoActividad.Sanidad,
          fechaProgramada: new Date().toISOString().split('T')[0],
          productoId: ''
        });
      }
    });
  };

  return (
    <AnimatePresence>
      {isOpen && (
        <>
          <motion.div
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            exit={{ opacity: 0 }}
            onClick={onClose}
            className="fixed inset-0 bg-black/80 backdrop-blur-md z-[120]"
          />
          <motion.div
            initial={{ opacity: 0, scale: 0.95, y: 20 }}
            animate={{ opacity: 1, scale: 1, y: 0 }}
            exit={{ opacity: 0, scale: 0.95, y: 20 }}
            className="fixed inset-0 m-auto w-full max-w-lg h-fit glass-dark z-[130] p-8 rounded-[2.5rem] border border-white/10"
          >
            <div className="flex items-center justify-between mb-8">
              <div>
                <h2 className="text-2xl font-black text-white uppercase tracking-widest">Actividad Manual</h2>
                <p className="text-xs font-bold text-slate-500 uppercase tracking-tighter mt-1">Programar tarea extraordinaria</p>
              </div>
              <button
                onClick={onClose}
                className="p-3 bg-white/5 hover:bg-white/10 rounded-2xl text-slate-500 transition-all"
              >
                <X size={24} />
              </button>
            </div>

            <form onSubmit={handleSubmit} className="space-y-6">
              <div className="space-y-2">
                <label className="text-[10px] font-black text-slate-500 uppercase tracking-widest ml-1">Descripción de la Tarea</label>
                <div className="relative">
                  <ClipboardCheck className="absolute left-4 top-1/2 -translate-y-1/2 text-slate-500" size={18} />
                  <input
                    required
                    type="text"
                    value={formData.descripcion}
                    onChange={(e) => setFormData({ ...formData, descripcion: e.target.value })}
                    placeholder="Ej: Vacunación extra, Limpieza profunda..."
                    className="w-full pl-12 pr-6 py-4 bg-white/5 border border-white/10 rounded-2xl text-white font-medium focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all"
                  />
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div className="space-y-2">
                  <label className="text-[10px] font-black text-slate-500 uppercase tracking-widest ml-1">Tipo</label>
                  <select
                    value={formData.tipo}
                    onChange={(e) => setFormData({ ...formData, tipo: Number(e.target.value) })}
                    className="w-full px-5 py-4 bg-white/5 border border-white/10 rounded-2xl text-white font-medium focus:outline-none focus:ring-2 focus:ring-primary/50 appearance-none"
                  >
                    <option value={TipoActividad.Sanidad} className="bg-slate-900">Sanidad</option>
                    <option value={TipoActividad.Vacuna} className="bg-slate-900">Vacuna</option>
                    <option value={TipoActividad.Tratamiento} className="bg-slate-900">Tratamiento</option>
                    <option value={TipoActividad.Control} className="bg-slate-900">Control</option>
                    <option value={TipoActividad.Limpieza} className="bg-slate-900">Limpieza</option>
                    <option value={TipoActividad.Otro} className="bg-slate-900">Otro</option>
                  </select>
                </div>

                <div className="space-y-2">
                  <label className="text-[10px] font-black text-slate-500 uppercase tracking-widest ml-1">Fecha Programada</label>
                  <div className="relative">
                    <CalendarIcon className="absolute left-4 top-1/2 -translate-y-1/2 text-slate-500" size={18} />
                    <input
                      required
                      type="date"
                      value={formData.fechaProgramada}
                      onChange={(e) => setFormData({ ...formData, fechaProgramada: e.target.value })}
                      className="w-full pl-12 pr-6 py-4 bg-white/5 border border-white/10 rounded-2xl text-white font-medium focus:outline-none focus:ring-2 focus:ring-primary/50"
                    />
                  </div>
                </div>
              </div>

              <div className="space-y-2">
                <label className="text-[10px] font-black text-slate-500 uppercase tracking-widest ml-1">Producto Relacionado (Opcional)</label>
                <div className="relative">
                  <Package className="absolute left-4 top-1/2 -translate-y-1/2 text-slate-500" size={18} />
                  <select
                    value={formData.productoId}
                    onChange={(e) => setFormData({ ...formData, productoId: e.target.value })}
                    className="w-full pl-12 pr-6 py-4 bg-white/5 border border-white/10 rounded-2xl text-white font-medium focus:outline-none focus:ring-2 focus:ring-primary/50 appearance-none"
                  >
                    <option value="" className="bg-slate-900">Ningún producto</option>
                    {productos.map((p) => (
                      <option key={p.id} value={p.id} className="bg-slate-900">
                        {p.nombre} ({p.categoriaNombre})
                      </option>
                    ))}
                  </select>
                </div>
              </div>

              <button
                type="submit"
                disabled={agregarActividadManual.isPending}
                className="w-full py-5 bg-primary text-black font-black rounded-[1.5rem] uppercase tracking-[0.2em] shadow-lg shadow-primary/20 hover:scale-[1.02] active:scale-[0.98] transition-all disabled:opacity-50 mt-4"
              >
                {agregarActividadManual.isPending ? 'Programando...' : 'Programar Actividad'}
              </button>
            </form>
          </motion.div>
        </>
      )}
    </AnimatePresence>
  );
}
