import Swal from 'sweetalert2';

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
    background: '#0f172a', // slate-950
    color: '#f8fafc', // slate-50
    customClass: {
      popup: 'rounded-3xl border border-white/10 shadow-2xl',
      title: 'font-bold',
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
    background: '#0f172a',
    color: '#f8fafc',
    customClass: {
      popup: 'rounded-3xl border border-white/10 shadow-2xl',
    }
  });
};
