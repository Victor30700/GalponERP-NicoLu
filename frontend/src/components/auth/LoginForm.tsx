'use client'

import { useState } from 'react'
import { motion } from 'framer-motion'
import { useAuth } from '@/context/AuthContext'
import { Loader2, LogIn, AlertCircle, Mail, Lock } from 'lucide-react'

export function LoginForm() {
  const { login } = useAuth()
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState<string | null>(null)
  const [isLoading, setIsLoading] = useState(false)

  const handleManualSubmit = async (e?: React.FormEvent) => {
    if (e) {
      e.preventDefault()
      e.stopPropagation()
    }

    console.log("BOTON CLICKED - Iniciando login manual");
    
    if (!email || !password) {
      setError("Por favor rellena todos los campos");
      return;
    }

    setIsLoading(true)
    setError(null)

    try {
      console.log("Llamando a login con:", email);
      await login(email, password)
      console.log("Login exitoso, redirigiendo...");
      
      // Redirección forzada
      window.location.replace('/')
    } catch (err: any) {
      console.error("Error capturado en el componente:", err);
      setError(err.message || 'Error de conexión con el servidor');
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <motion.div
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      className="w-full max-w-md p-8 glass-dark rounded-3xl shadow-2xl relative z-10 border border-white/10"
    >
      <div className="flex flex-col items-center mb-8">
        <div className="w-16 h-16 bg-primary/20 rounded-2xl flex items-center justify-center mb-4 border border-primary/30">
          <LogIn className="text-primary" size={32} />
        </div>
        <h1 className="text-3xl font-black text-white tracking-tighter">GALPON<span className="text-primary text-4xl">.</span>ERP</h1>
        <p className="text-slate-500 text-sm font-bold uppercase tracking-widest mt-1">Acceso de Operador</p>
      </div>

      <div className="space-y-5">
        <div className="space-y-2">
          <label className="text-[10px] font-black text-slate-500 uppercase tracking-widest ml-1">Email Profesional</label>
          <div className="relative">
            <Mail className="absolute left-4 top-1/2 -translate-y-1/2 text-slate-600" size={18} />
            <input
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              className="w-full pl-12 pr-4 py-4 bg-slate-900/50 border border-white/5 rounded-2xl text-white focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all placeholder:text-slate-700 font-medium"
              placeholder="admin@galponerp.com"
            />
          </div>
        </div>

        <div className="space-y-2">
          <label className="text-[10px] font-black text-slate-500 uppercase tracking-widest ml-1">Contraseña de Seguridad</label>
          <div className="relative">
            <Lock className="absolute left-4 top-1/2 -translate-y-1/2 text-slate-600" size={18} />
            <input
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              className="w-full pl-12 pr-4 py-4 bg-slate-900/50 border border-white/5 rounded-2xl text-white focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all placeholder:text-slate-700 font-medium"
              placeholder="••••••••"
            />
          </div>
        </div>

        {error && (
          <motion.div 
            initial={{ opacity: 0, scale: 0.95 }} 
            animate={{ opacity: 1, scale: 1 }}
            className="p-4 bg-red-500/10 border border-red-500/20 rounded-2xl text-red-400 text-xs font-bold flex items-center gap-3"
          >
            <AlertCircle size={18} className="shrink-0" />
            {error}
          </motion.div>
        )}

        <button
          onClick={() => handleManualSubmit()}
          disabled={isLoading}
          type="button"
          className="w-full py-4 bg-primary hover:bg-primary/90 text-primary-foreground font-black rounded-2xl transition-all flex items-center justify-center gap-3 disabled:opacity-50 shadow-xl shadow-primary/20 group"
        >
          {isLoading ? (
            <>
              <Loader2 className="animate-spin" size={20} />
              <span className="uppercase tracking-widest text-xs">Verificando Credenciales...</span>
            </>
          ) : (
            <>
              <span className="uppercase tracking-widest text-xs">Entrar al Sistema</span>
              <LogIn size={18} className="group-hover:translate-x-1 transition-transform" />
            </>
          )}
        </button>
      </div>
      
      <div className="mt-8 pt-6 border-t border-white/5 text-center">
        <p className="text-[9px] text-slate-600 font-bold uppercase tracking-[0.2em]">
          Conectado a: {process.env.NEXT_PUBLIC_API_URL?.replace('https://', '')}
        </p>
      </div>
    </motion.div>
  )
}
