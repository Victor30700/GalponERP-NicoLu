'use client'

import { Toaster } from 'sonner'
import { useTheme } from '@/context/ThemeContext'

export function ToasterWrapper() {
  const { theme, mounted } = useTheme()

  if (!mounted) return null

  return (
    <Toaster 
      theme={theme} 
      position="top-right" 
      toastOptions={{
        style: {
          background: theme === 'dark' ? '#0f172a' : '#ffffff',
          border: theme === 'dark' ? '1px solid rgba(255, 255, 255, 0.1)' : '1px solid rgba(0, 0, 0, 0.05)',
          color: theme === 'dark' ? '#f1f5f9' : '#1e293b',
        },
      }}
    />
  )
}
