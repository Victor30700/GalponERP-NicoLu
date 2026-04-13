import { 
  LayoutDashboard, 
  Bird, 
  Warehouse, 
  Package, 
  Users, 
  ShoppingCart, 
  DollarSign, 
  Settings,
  ShieldCheck,
  ClipboardList,
  MessageSquare
} from 'lucide-react'

export const navigationItems = [
  { name: 'Dashboard', href: '/', icon: LayoutDashboard },
  { name: 'Lotes', href: '/lotes', icon: Bird },
  { name: 'Chat con IA', href: '/chat', icon: MessageSquare },
  { name: 'Galpones', href: '/galpones', icon: Warehouse },
  { name: 'Inventario', href: '/inventario', icon: Package },
  { name: 'Ventas', href: '/ventas', icon: ShoppingCart },
  { name: 'Finanzas', href: '/finanzas', icon: DollarSign },
  { name: 'Sanidad', href: '/sanidad', icon: ShieldCheck },
  { name: 'Clientes', href: '/clientes', icon: Users },
  { name: 'Auditoría', href: '/auditoria', icon: ClipboardList },
  { name: 'Configuración', href: '/configuracion', icon: Settings },
]
