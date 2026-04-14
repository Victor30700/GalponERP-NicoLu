'use client';

import Swal from 'sweetalert2';

export function useSwal() {
  const toast = (title: string, icon: 'success' | 'error' | 'warning' | 'info' = 'success') => {
    const Toast = Swal.mixin({
      toast: true,
      position: 'top-end',
      showConfirmButton: false,
      timer: 3000,
      timerProgressBar: true,
      background: '#1e293b',
      color: '#f8fafc',
      didOpen: (toast) => {
        toast.addEventListener('mouseenter', Swal.stopTimer);
        toast.addEventListener('mouseleave', Swal.resumeTimer);
      }
    });

    Toast.fire({
      icon,
      title
    });
  };

  const confirm = async (title: string, text: string, icon: 'warning' | 'info' | 'question' = 'warning') => {
    const result = await Swal.fire({
      title,
      text,
      icon,
      showCancelButton: true,
      confirmButtonColor: '#eafe00', // primary
      cancelButtonColor: '#334155',  // slate-700
      confirmButtonText: 'Confirmar',
      cancelButtonText: 'Cancelar',
      background: '#0f172a', // slate-950
      color: '#f8fafc', // slate-50
      customClass: {
        popup: 'rounded-[2rem] border border-white/10 shadow-2xl backdrop-blur-xl',
        title: 'text-xl font-black uppercase tracking-tight',
        htmlContainer: 'text-slate-400 font-medium',
        confirmButton: 'rounded-xl font-black px-6 py-3 text-black uppercase text-xs tracking-widest transition-transform active:scale-95',
        cancelButton: 'rounded-xl font-bold px-6 py-3 text-slate-300 uppercase text-xs tracking-widest transition-transform active:scale-95'
      }
    });
    return result.isConfirmed;
  };

  const alert = async ({ title, text, icon = 'info' }: { title: string, text: string, icon?: 'success' | 'error' | 'warning' | 'info' }) => {
    await Swal.fire({
      title,
      text,
      icon,
      confirmButtonColor: '#eafe00',
      background: '#0f172a',
      color: '#f8fafc',
      customClass: {
        popup: 'rounded-[2rem] border border-white/10 shadow-2xl backdrop-blur-xl',
        title: 'text-xl font-black uppercase tracking-tight',
        htmlContainer: 'text-slate-400 font-medium',
        confirmButton: 'rounded-xl font-black px-8 py-3 text-black uppercase text-xs tracking-widest transition-transform active:scale-95',
      }
    });
  };

  return { toast, confirm, alert };
}
