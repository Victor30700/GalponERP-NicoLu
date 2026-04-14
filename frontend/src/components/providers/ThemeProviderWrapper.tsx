'use client'

import { ThemeProvider, useTheme } from '@/context/ThemeContext'
import { useEffect, useState } from 'react'

function ThemeContent({ children }: { children: React.ReactNode }) {
  const { mounted } = useTheme()

  // Evitar saltos de hidratación mostrando un estado inicial coherente
  if (!mounted) {
    return (
      <div className="min-h-screen bg-background text-foreground">
        {children}
      </div>
    )
  }

  return <>{children}</>
}

export function ThemeProviderWrapper({ children }: { children: React.ReactNode }) {
  return (
    <ThemeProvider>
      <ThemeContent>{children}</ThemeContent>
    </ThemeProvider>
  )
}
