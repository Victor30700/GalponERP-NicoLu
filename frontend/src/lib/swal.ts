import Swal from 'sweetalert2';

// Configuración base para asegurar visibilidad
const swalConfig = {
  background: 'var(--card)',
  color: 'var(--foreground)',
  backdrop: `rgba(0,0,0,0.4)`,
  customClass: {
    popup: 'rounded-[2.5rem] border border-border shadow-2xl bg-card',
    title: 'font-black text-foreground uppercase tracking-tight',
    htmlContainer: 'text-muted-foreground font-medium',
    confirmButton: 'rounded-2xl font-black px-8 py-4 text-black uppercase text-xs tracking-[0.2em] transition-all hover:scale-105 active:scale-95 shadow-lg',
    cancelButton: 'rounded-2xl font-bold px-8 py-4 text-muted-foreground uppercase text-xs tracking-widest hover:bg-muted transition-all active:scale-95'
  },
  buttonsStyling: false
};

export const confirmAction = async (title: string, text: string, icon: 'warning' | 'info' | 'question' = 'question') => {
  return await Swal.fire({
    ...swalConfig,
    title,
    text,
    icon,
    showCancelButton: true,
    confirmButtonText: 'Confirmar',
    cancelButtonText: 'Cancelar',
    customClass: {
      ...swalConfig.customClass,
      confirmButton: swalConfig.customClass.confirmButton + ' bg-[#eafe00] shadow-[#eafe00]/20',
      cancelButton: swalConfig.customClass.cancelButton + ' bg-muted/50'
    }
  });
};

export const promptAction = async (title: string, placeholder: string) => {
  return await Swal.fire({
    ...swalConfig,
    title,
    input: 'textarea',
    inputPlaceholder: placeholder,
    showCancelButton: true,
    confirmButtonText: 'Enviar',
    cancelButtonText: 'Cancelar',
    customClass: {
      ...swalConfig.customClass,
      input: 'bg-muted/50 border-border text-foreground rounded-2xl focus:ring-primary/50 mx-4 my-2 p-4 font-medium',
      confirmButton: swalConfig.customClass.confirmButton + ' bg-[#eafe00] shadow-[#eafe00]/20',
      cancelButton: swalConfig.customClass.cancelButton + ' bg-muted/50'
    }
  });
};

export const confirmDestructiveAction = async (title: string, text: string) => {
  return await Swal.fire({
    ...swalConfig,
    title,
    text,
    icon: 'warning',
    showCancelButton: true,
    confirmButtonText: 'Sí, eliminar',
    cancelButtonText: 'Cancelar',
    customClass: {
      ...swalConfig.customClass,
      confirmButton: swalConfig.customClass.confirmButton + ' bg-red-500 text-white shadow-red-500/20',
      cancelButton: swalConfig.customClass.cancelButton + ' bg-muted/50'
    }
  });
};

export const showSuccessAlert = (title: string, text?: string) => {
  Swal.fire({
    ...swalConfig,
    title,
    text,
    icon: 'success',
    timer: 2000,
    showConfirmButton: false
  });
};

