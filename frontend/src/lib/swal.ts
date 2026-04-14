import Swal from 'sweetalert2';

export const confirmAction = async (title: string, text: string, icon: 'warning' | 'info' | 'question' = 'question') => {
  return await Swal.fire({
    title,
    text,
    icon,
    showCancelButton: true,
    confirmButtonColor: '#eafe00', // primary
    cancelButtonColor: '#1e293b',  // slate-800
    confirmButtonText: 'Confirmar',
    cancelButtonText: 'Cancelar',
    background: 'hsl(var(--background))',
    color: 'hsl(var(--foreground))',
    customClass: {
      popup: 'rounded-3xl border border-border shadow-2xl',
      title: 'font-bold text-foreground',
      confirmButton: 'rounded-xl font-black px-6 py-2.5 text-black uppercase text-xs tracking-widest',
      cancelButton: 'rounded-xl font-bold px-6 py-2.5 text-slate-400 uppercase text-xs tracking-widest'
    }
  });
};

export const promptAction = async (title: string, placeholder: string) => {
  return await Swal.fire({
    title,
    input: 'textarea',
    inputPlaceholder: placeholder,
    showCancelButton: true,
    confirmButtonColor: '#eafe00',
    cancelButtonColor: '#1e293b',
    confirmButtonText: 'Enviar',
    cancelButtonText: 'Cancelar',
    background: 'hsl(var(--background))',
    color: 'hsl(var(--foreground))',
    customClass: {
      popup: 'rounded-3xl border border-border shadow-2xl',
      input: 'bg-muted/50 border-border text-foreground rounded-xl focus:ring-primary/50',
      confirmButton: 'rounded-xl font-black px-6 py-2.5 text-black uppercase text-xs tracking-widest',
      cancelButton: 'rounded-xl font-bold px-6 py-2.5 text-slate-400 uppercase text-xs tracking-widest'
    }
  });
};

export const confirmDestructiveAction = async (title: string, text: string) => {
  return await Swal.fire({
    title,
    text,
    icon: 'warning',
    showCancelButton: true,
    confirmButtonColor: '#ef4444', // red-500
    cancelButtonColor: '#1e293b',  // slate-800
    confirmButtonText: 'Sí, eliminar',
    cancelButtonText: 'Cancelar',
    background: 'hsl(var(--background))',
    color: 'hsl(var(--foreground))',
    customClass: {
      popup: 'rounded-3xl border border-border shadow-2xl',
      title: 'font-bold text-foreground',
      confirmButton: 'rounded-xl font-bold px-6 py-2.5',
      cancelButton: 'rounded-xl font-bold px-6 py-2.5'
    }
  });
};

export const showSuccessAlert = (title: string, text?: string) => {
  Swal.fire({
    title,
    text,
    icon: 'success',
    timer: 2000,
    showConfirmButton: false,
    background: 'hsl(var(--background))',
    color: 'hsl(var(--foreground))',
    customClass: {
      popup: 'rounded-3xl border border-border shadow-2xl',
      title: 'text-foreground'
    }
  });
};

