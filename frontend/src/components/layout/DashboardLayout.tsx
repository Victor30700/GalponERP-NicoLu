'use client'

import { Sidebar } from './Sidebar'
import { TopNavbar } from './TopNavbar'
import { BottomNav } from './BottomNav'
import { useAuth } from '@/context/AuthContext'
import { useRouter, usePathname } from 'next/navigation'
import { useEffect } from 'react'
import { hasAccess } from '@/lib/rbac'
import { navigationSections } from '@/config/navigation'

export function DashboardLayout({ children }: { children: React.ReactNode }) {
  const { profile, loading } = useAuth()
  const router = useRouter()
  const pathname = usePathname()

  useEffect(() => {
    if (!loading) {
      if (!profile) {
        router.push('/login')
      } else {
        // Verificar permisos de ruta (Guardián)
        const userRole = Number(profile.rol)
        if (!hasAccess(pathname, userRole, navigationSections)) {
          console.warn(`Acceso denegado a ${pathname} para el rol ${userRole}`)
          router.push('/')
        }
      }
    }
  }, [profile, loading, router, pathname])

  if (loading) {
    return (
      <div className="h-screen w-full flex items-center justify-center bg-background">
        <div className="w-10 h-10 border-4 border-primary/20 border-t-primary rounded-full animate-spin" />
      </div>
    )
  }

  if (!profile) return null

  return (
    <div className="flex min-h-screen bg-background text-foreground">
      <Sidebar />
      <div className="flex-1 flex flex-col min-w-0">
        <TopNavbar />
        <main className="flex-1 p-6 md:pb-6 pb-24 overflow-x-hidden">
          {children}
        </main>
        <BottomNav />
      </div>
    </div>
  )
}
