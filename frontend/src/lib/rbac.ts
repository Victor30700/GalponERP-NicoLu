export enum UserRole {
  Empleado = 0,
  SubAdmin = 1,
  Admin = 2
}

/**
 * Define los permisos por defecto para las rutas que no tienen roles especificados
 * Por defecto, si una ruta no tiene roles asignados en navigation.ts, se asume que es pública para todos los autenticados.
 */
export const hasAccess = (pathname: string, userRole: number, navigationSections: any[]): boolean => {
  // El Dashboard "/" siempre es accesible
  if (pathname === '/') return true;

  // Encontrar el item de navegación que coincida con el pathname
  const allItems = navigationSections.flatMap(section => section.items);
  const currentItem = allItems.find(item => item.href === pathname);

  // Si el item no existe en la navegación (ej. subrutas o páginas no listadas)
  // podrías implementar una lógica más compleja aquí. Por ahora, si no tiene roles definidos, es libre.
  if (!currentItem || !currentItem.roles) {
    return true;
  }

  // Verificar si el rol del usuario está en la lista de roles permitidos
  return currentItem.roles.includes(userRole);
};
