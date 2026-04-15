'use client';

import Swal from 'sweetalert2';

export function useSwal() {
  const commonConfig = {
    background: 'var(--card)',
    color: 'var(--foreground)',
    buttonsStyling: false,
    customClass: {
      popup: 'rounded-[2.5rem] border border-border shadow-2xl bg-card',
      title: 'text-xl font-black uppercase tracking-tight text-foreground',
      htmlContainer: 'text-muted-foreground font-medium',
      confirmButton: 'rounded-xl font-black px-6 py-3 text-black uppercase text-xs tracking-widest transition-transform active:scale-95 bg-[#eafe00] shadow-lg shadow-[#eafe00]/20 mx-2',
      cancelButton: 'rounded-xl font-bold px-6 py-3 text-muted-foreground uppercase text-xs tracking-widest transition-transform active:scale-95 bg-muted/50 mx-2'
    }
  };

  const toast = (title: string, icon: 'success' | 'error' | 'warning' | 'info' = 'success') => {
    const Toast = Swal.mixin({
      toast: true,
      position: 'top-end',
      showConfirmButton: false,
      timer: 3000,
      timerProgressBar: true,
      background: 'var(--card)',
      color: 'var(--foreground)',
      customClass: {
        popup: 'rounded-2xl border border-border shadow-xl bg-card'
      },
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
      ...commonConfig,
      title,
      text,
      icon,
      showCancelButton: true,
      confirmButtonText: 'Confirmar',
      cancelButtonText: 'Cancelar'
    });
    return result.isConfirmed;
  };

  const alert = async ({ title, text, icon = 'info' }: { title: string, text: string, icon?: 'success' | 'error' | 'warning' | 'info' }) => {
    await Swal.fire({
      ...commonConfig,
      title,
      text,
      icon,
      confirmButtonText: 'Aceptar'
    });
  };

  return { toast, confirm, alert };
}
