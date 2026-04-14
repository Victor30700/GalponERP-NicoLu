import { useState } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import { X, Bird, Droplets, ClipboardCheck, AlertCircle, Plus } from 'lucide-react';
import { useQuery } from '@tanstack/react-query';
import { api } from '@/lib/api';
import { QuickRecordModal } from './QuickRecordModal';
import { ManualActivityModal } from './ManualActivityModal';

interface SanitaryEventModalProps {
  isOpen: boolean;
  onClose: () => void;
}

export function SanitaryEventModal({ isOpen, onClose }: SanitaryEventModalProps) {
  const [selectedLoteId, setSelectedLoteId] = useState('');
  const [activeModal, setActiveModal] = useState<'mortality' | 'water' | 'manual' | null>(null);

  const { data: lotes = [] } = useQuery({
    queryKey: ['lotes-activos-select'],
    queryFn: () => api.get<any[]>('/api/Lotes?soloActivos=true'),
    enabled: isOpen
  });

  const selectedLote = lotes.find(l => l.id === selectedLoteId);

  const handleOpenModal = (type: 'mortality' | 'water' | 'manual') => {
    if (!selectedLoteId) return;
    setActiveModal(type);
  };

  return (
    <>
      <AnimatePresence>
        {isOpen && !activeModal && (
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
              className="fixed inset-0 m-auto w-full max-w-2xl h-fit glass-dark z-[130] p-8 rounded-[2.5rem] border border-white/10"
            >
              <div className="flex items-center justify-between mb-8">
                <div>
                  <h2 className="text-2xl font-black text-white uppercase tracking-widest">Registrar Evento Sanitario</h2>
                  <p className="text-xs font-bold text-slate-500 uppercase tracking-tighter mt-1">Seleccione un lote y el tipo de registro</p>
                </div>
                <button onClick={onClose} className="p-3 bg-white/5 hover:bg-white/10 rounded-2xl text-slate-500 transition-all">
                  <X size={24} />
                </button>
              </div>

              <div className="space-y-8">
                <div className="space-y-2">
                  <label className="text-[10px] font-black text-slate-500 uppercase tracking-widest ml-1">Seleccionar Lote Activo</label>
                  <select
                    value={selectedLoteId}
                    onChange={(e) => setSelectedLoteId(e.target.value)}
                    className="w-full px-6 py-4 bg-white/5 border border-white/10 rounded-2xl text-white font-bold focus:outline-none focus:ring-2 focus:ring-primary/50 appearance-none"
                  >
                    <option value="" className="bg-slate-900">Elegir un lote...</option>
                    {lotes.map((l) => (
                      <option key={l.id} value={l.id} className="bg-slate-900">
                        {l.nombreLote} - {l.galponNombre} ({l.cantidadAves} aves)
                      </option>
                    ))}
                  </select>
                </div>

                <div className={`grid grid-cols-1 md:grid-cols-3 gap-4 transition-all ${!selectedLoteId ? 'opacity-20 pointer-events-none' : ''}`}>
                  <button
                    onClick={() => handleOpenModal('mortality')}
                    className="p-6 glass-dark rounded-3xl border border-white/5 hover:border-red-500/50 transition-all group text-left"
                  >
                    <div className="w-12 h-12 rounded-2xl bg-red-500/10 text-red-500 flex items-center justify-center mb-4 group-hover:scale-110 transition-transform">
                      <AlertCircle size={24} />
                    </div>
                    <h4 className="text-white font-black text-xs uppercase tracking-widest">Mortalidad</h4>
                    <p className="text-[10px] text-slate-500 mt-1 font-bold">Registro de bajas diarias.</p>
                  </button>

                  <button
                    onClick={() => handleOpenModal('water')}
                    className="p-6 glass-dark rounded-3xl border border-white/5 hover:border-amber-500/50 transition-all group text-left"
                  >
                    <div className="w-12 h-12 rounded-2xl bg-amber-500/10 text-amber-500 flex items-center justify-center mb-4 group-hover:scale-110 transition-transform">
                      <Droplets size={24} />
                    </div>
                    <h4 className="text-white font-black text-xs uppercase tracking-widest">Bienestar</h4>
                    <p className="text-[10px] text-slate-500 mt-1 font-bold">Consumo de agua y clima.</p>
                  </button>

                  <button
                    onClick={() => handleOpenModal('manual')}
                    className="p-6 glass-dark rounded-3xl border border-white/5 hover:border-primary/50 transition-all group text-left"
                  >
                    <div className="w-12 h-12 rounded-2xl bg-primary/10 text-primary flex items-center justify-center mb-4 group-hover:scale-110 transition-transform">
                      <ClipboardCheck size={24} />
                    </div>
                    <h4 className="text-white font-black text-xs uppercase tracking-widest">Actividad</h4>
                    <p className="text-[10px] text-slate-500 mt-1 font-bold">Vacunas y tratamientos.</p>
                  </button>
                </div>
              </div>
            </motion.div>
          </>
        )}
      </AnimatePresence>

      {activeModal === 'mortality' && (
        <QuickRecordModal
          isOpen={true}
          onClose={() => { setActiveModal(null); onClose(); }}
          loteId={selectedLoteId}
          type="mortality"
          lote={selectedLote}
        />
      )}

      {activeModal === 'water' && (
        <QuickRecordModal
          isOpen={true}
          onClose={() => { setActiveModal(null); onClose(); }}
          loteId={selectedLoteId}
          type="water"
          lote={selectedLote}
        />
      )}

      {activeModal === 'manual' && (
        <ManualActivityModal
          isOpen={true}
          onClose={() => { setActiveModal(null); onClose(); }}
          loteId={selectedLoteId}
        />
      )}
    </>
  );
}
