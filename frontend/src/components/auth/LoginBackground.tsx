'use client'

import { motion, AnimatePresence } from 'framer-motion'
import { useEffect, useState } from 'react'

const backgrounds = [
  "https://images.unsplash.com/photo-1548550023-2bdb3c5beed7?q=80&w=1974&auto=format&fit=crop", // Pollos en granja
  "https://images.unsplash.com/photo-1516467508483-a7212febe31a?q=80&w=2073&auto=format&fit=crop", // Granja amanecer
  "https://images.unsplash.com/photo-1527672809634-04ed36500acd?q=80&w=1974&auto=format&fit=crop"  // Maíz/Granos
]

export function LoginBackground() {
  const [index, setIndex] = useState(0)

  useEffect(() => {
    const timer = setInterval(() => {
      setIndex((prev) => (prev + 1) % backgrounds.length)
    }, 8000)
    return () => clearInterval(timer)
  }, [])

  return (
    <div className="fixed inset-0 z-0 overflow-hidden bg-slate-950">
      <AnimatePresence mode="wait">
        <motion.div
          key={backgrounds[index]}
          initial={{ opacity: 0, scale: 1.1 }}
          animate={{ opacity: 1, scale: 1 }}
          exit={{ opacity: 0 }}
          transition={{ duration: 2 }}
          className="absolute inset-0"
        >
          <div 
            className="absolute inset-0 bg-cover bg-center bg-no-repeat"
            style={{ backgroundImage: `url(${backgrounds[index]})` }}
          />
          {/* Overlay cinemático */}
          <div className="absolute inset-0 bg-gradient-to-br from-slate-950/90 via-slate-950/60 to-indigo-950/40" />
          <div className="absolute inset-0 backdrop-blur-[2px]" />
        </motion.div>
      </AnimatePresence>
    </div>
  )
}
