import { 
  LayoutDashboard, 
  Bird, 
  Warehouse, 
  Package, 
  Users, 
  ShoppingCart, 
  Receipt,
  DollarSign, 
  Settings,
  Tag,
  ShieldCheck,
  ClipboardList,
  MessageSquare,
  Truck,
  Ruler,
  FileText,
  HelpCircle
} from 'lucide-react'
import { UserRole } from '@/lib/rbac'

export interface NavigationItem {
  name: string
  href: string
  icon: any
  roles?: UserRole[]
}

export interface NavigationSection {
  title: string
  items: NavigationItem[]
}

export const navigationSections: NavigationSection[] = [
  {
    title: 'General',
    items: [
      { name: 'Dashboard', href: '/', icon: LayoutDashboard },
      { name: 'Chat con IA', href: '/chat', icon: MessageSquare },
    ]
  },
  {
    title: 'Producción',
    items: [
      { name: 'Lotes', href: '/lotes', icon: Bird },
      { name: 'Sanidad', href: '/sanidad', icon: ShieldCheck },
      { name: 'Galpones', href: '/galpones', icon: Warehouse },
      { name: 'Planificación', href: '/planificacion', icon: ClipboardList },
    ]
  },
  {
    title: 'Inventario y Catálogos',
    items: [
      { name: 'Inventario', href: '/inventario', icon: Package },
      { name: 'Productos', href: '/productos', icon: Package },
      { name: 'Categorías', href: '/categorias', icon: Tag },
      { name: 'Unidades', href: '/unidades-medida', icon: Ruler },
      { name: 'Proveedores', href: '/proveedores', icon: Truck },
    ]
  },
  {
    title: 'Finanzas y Ventas',
    items: [
      { name: 'Ventas', href: '/ventas', icon: ShoppingCart },
      { name: 'Gastos', href: '/gastos', icon: Receipt },
      { 
        name: 'Finanzas', 
        href: '/finanzas', 
        icon: DollarSign, 
        roles: [UserRole.Admin, UserRole.SubAdmin] 
      },
      { name: 'Clientes', href: '/clientes', icon: Users },
    ]
  },
  {
    title: 'Administración',
    items: [
      { 
        name: 'Usuarios', 
        href: '/usuarios', 
        icon: Users, 
        roles: [UserRole.Admin, UserRole.SubAdmin] 
      },
      { 
        name: 'Auditoría', 
        href: '/auditoria', 
        icon: FileText, 
        roles: [UserRole.Admin] 
      },
      { name: 'Plantillas', href: '/plantillas', icon: ClipboardList },
      { 
        name: 'Configuración', 
        href: '/configuracion', 
        icon: Settings, 
        roles: [UserRole.Admin] 
      },
    ]
  },
  {
    title: 'Soporte',
    items: [
      { name: 'Ayuda', href: '/ayuda', icon: HelpCircle },
    ]
  }
]

// Mantener compatibilidad con componentes que usen navigationItems plano
export const navigationItems = navigationSections.flatMap(section => section.items)
