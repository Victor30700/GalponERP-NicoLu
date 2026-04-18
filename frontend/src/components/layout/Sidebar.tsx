'use client'

import { useState } from 'react'
import Link from 'next/link'
import { usePathname } from 'next/navigation'
import { motion } from 'framer-motion'
import { cn } from '@/lib/utils'
import { navigationSections } from '@/config/navigation'
import { ChevronLeft, ChevronRight, LogOut } from 'lucide-react'
import { useAuth } from '@/context/AuthContext'

export function Sidebar() {
  const [isCollapsed, setIsCollapsed] = useState(false)
  const pathname = usePathname()
  const { logout, profile } = useAuth()

  const userRole = profile?.rol !== undefined ? Number(profile.rol) : null

  const filteredSections = navigationSections.map(section => ({
    ...section,
    items: section.items.filter(item => 
      !item.roles || (userRole !== null && item.roles.includes(userRole))
    )
  })).filter(section => section.items.length > 0)

  return (
    <motion.aside
      animate={{ width: isCollapsed ? 80 : 260 }}
      className="hidden md:flex flex-col h-screen glass sticky top-0 left-0 z-40 border-r border-border shadow-xl"
    >
      <div className="p-6 flex items-center justify-between">
        {!isCollapsed && (
          <motion.h1 
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            className="text-xl font-black text-foreground tracking-tighter"
          >
            GALPON<span className="text-primary">ERP</span>
          </motion.h1>
        )}
        <button
          onClick={() => setIsCollapsed(!isCollapsed)}
          className="p-2 hover:bg-muted/50 rounded-lg text-muted-foreground transition-colors"
        >
          {isCollapsed ? <ChevronRight size={20} /> : <ChevronLeft size={20} />}
        </button>
      </div>

      <nav className="flex-1 px-3 space-y-6 overflow-y-auto no-scrollbar pb-10">
        {filteredSections.map((section) => (
          <div key={section.title} className="space-y-1">
            {!isCollapsed && (
              <h3 className="px-4 text-[10px] font-black text-muted-foreground uppercase tracking-[0.2em] mb-2 opacity-50">
                {section.title}
              </h3>
            )}
            
            <div className="space-y-1">
              {section.items.map((item) => {
                const isActive = pathname === item.href
                return (
                  <Link
                    key={item.name}
                    href={item.href}
                    className={cn(
                      "flex items-center gap-3 px-3 py-2.5 rounded-xl transition-all group relative",
                      isActive 
                        ? "bg-primary text-primary-foreground font-bold shadow-lg shadow-primary/20" 
                        : "text-muted-foreground hover:text-foreground hover:bg-muted/50"
                    )}
                  >
                    <item.icon size={20} className={cn(isActive ? "" : "group-hover:text-primary transition-colors")} />
                    {!isCollapsed && (
                      <motion.span
                        initial={{ opacity: 0, x: -10 }}
                        animate={{ opacity: 1, x: 0 }}
                        className="text-sm"
                      >
                        {item.name}
                      </motion.span>
                    )}
                    {isActive && isCollapsed && (
                      <div className="absolute left-0 w-1 h-6 bg-primary-foreground rounded-r-full" />
                    )}
                  </Link>
                )
              })}
            </div>
            {isCollapsed && <div className="mx-4 border-t border-border/50 my-4" />}
          </div>
        ))}
      </nav>

      <div className="p-4 border-t border-border bg-muted/20">
        <button
          onClick={logout}
          className={cn(
            "w-full flex items-center gap-3 px-3 py-3 rounded-xl text-muted-foreground hover:text-red-400 hover:bg-red-500/10 transition-all font-bold text-xs uppercase tracking-widest",
            isCollapsed && "justify-center"
          )}
        >
          <LogOut size={20} />
          {!isCollapsed && <span>Cerrar Sesión</span>}
        </button>
      </div>
    </motion.aside>
  )
}
