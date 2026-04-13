'use client'

import { useState, useEffect, useRef } from 'react'
import { useAuth } from '@/context/AuthContext'
import { useRouter } from 'next/navigation'
import { motion, AnimatePresence } from 'framer-motion'
import { 
  Send, 
  Mic, 
  MicOff, 
  ChevronLeft, 
  Bot, 
  User, 
  Loader2, 
  Volume2, 
  VolumeX,
  Trash2,
  Sparkles,
  MessageSquare
} from 'lucide-react'
import { api } from '@/lib/api'
import { cn } from '@/lib/utils'
import { toast } from 'sonner'

interface Message {
  id: string
  role: 'user' | 'assistant'
  content: string
  timestamp: Date
  isVoice?: boolean
}

export default function ChatPage() {
  const { user, loading } = useAuth()
  const router = useRouter()
  const [messages, setMessages] = useState<Message[]>([])
  const [input, setInput] = useState('')
  const [conversacionId, setConversacionId] = useState<string | null>(null)
  const [isTyping, setIsTyping] = useState(false)
  const [isRecording, setIsRecording] = useState(false)
  const [isAudioEnabled, setIsAudioEnabled] = useState(true)
  
  const messagesEndRef = useRef<HTMLDivElement>(null)
  const mediaRecorderRef = useRef<MediaRecorder | null>(null)
  const audioChunksRef = useRef<Blob[]>([])

  useEffect(() => {
    if (!loading && !user) router.push('/login')
  }, [user, loading, router])

  useEffect(() => {
    scrollToBottom()
  }, [messages, isTyping])

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' })
  }

  const handleSend = async (text?: string) => {
    const messageText = text || input
    if (!messageText.trim() || isTyping) return

    const userMsg: Message = {
      id: Date.now().toString(),
      role: 'user',
      content: messageText,
      timestamp: new Date()
    }

    setMessages(prev => [...prev, userMsg])
    setInput('')
    setIsTyping(true)

    try {
      const response = await api.post<{ respuesta: string, conversacionId: string }>('/api/Agentes/chat', {
        mensaje: messageText,
        conversacionId: conversacionId
      })

      const botMsg: Message = {
        id: (Date.now() + 1).toString(),
        role: 'assistant',
        content: response.respuesta,
        timestamp: new Date()
      }

      setMessages(prev => [...prev, botMsg])
      setConversacionId(response.conversacionId)
    } catch (error: any) {
      toast.error("Error al conectar con el asistente")
      console.error(error)
    } finally {
      setIsTyping(false)
    }
  }

  const startRecording = async () => {
    try {
      const stream = await navigator.mediaDevices.getUserMedia({ audio: true })
      const mediaRecorder = new MediaRecorder(stream)
      mediaRecorderRef.current = mediaRecorder
      audioChunksRef.current = []

      mediaRecorder.ondataavailable = (event) => {
        audioChunksRef.current.push(event.data)
      }

      mediaRecorder.onstop = async () => {
        const audioBlob = new Blob(audioChunksRef.current, { type: 'audio/wav' })
        await sendVoiceMessage(audioBlob)
        stream.getTracks().forEach(track => track.stop())
      }

      mediaRecorder.start()
      setIsRecording(true)
    } catch (err) {
      toast.error("No se pudo acceder al micrófono")
    }
  }

  const stopRecording = () => {
    if (mediaRecorderRef.current && isRecording) {
      mediaRecorderRef.current.stop()
      setIsRecording(false)
    }
  }

  const sendVoiceMessage = async (blob: Blob) => {
    setIsTyping(true)
    const formData = new FormData()
    formData.append('Audio', blob, 'recording.wav')
    if (conversacionId) formData.append('ConversacionId', conversacionId)

    try {
      // Usamos fetch directamente porque api.post de mi lib podría no manejar FormData bien si no está configurada
      const idToken = await user?.getIdToken()
      const response = await fetch(`${process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5167'}/api/voice/upload`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${idToken}`
        },
        body: formData
      })

      if (!response.ok) throw new Error("Error en el servidor de voz")

      const data = await response.json()
      
      const userMsg: Message = {
        id: Date.now().toString(),
        role: 'user',
        content: data.transcripcion,
        timestamp: new Date(),
        isVoice: true
      }

      const botMsg: Message = {
        id: (Date.now() + 1).toString(),
        role: 'assistant',
        content: data.respuestaTexto,
        timestamp: new Date()
      }

      setMessages(prev => [...prev, userMsg, botMsg])
      setConversacionId(data.conversacionId)

      if (isAudioEnabled && data.respuestaAudioBase64) {
        playAudio(data.respuestaAudioBase64)
      }
    } catch (error) {
      toast.error("Error al procesar mensaje de voz")
      console.error(error)
    } finally {
      setIsTyping(false)
    }
  }

  const playAudio = (base64: string) => {
    const audio = new Audio(`data:audio/wav;base64,${base64}`)
    audio.play()
  }

  const clearChat = () => {
    if (confirm("¿Limpiar historial de esta conversación?")) {
      setMessages([])
      setConversacionId(null)
    }
  }

  if (loading) return null

  return (
    <div className="flex flex-col h-[calc(100vh-64px)] md:h-[calc(100vh-2rem)] glass-dark border border-white/5 rounded-3xl overflow-hidden shadow-2xl relative">
      {/* Header */}
      <header className="px-6 py-4 border-b border-white/5 bg-white/5 flex items-center justify-between">
        <div className="flex items-center gap-3">
          <div className="w-10 h-10 rounded-2xl bg-indigo-500/20 flex items-center justify-center text-indigo-400">
            <Bot size={24} />
          </div>
          <div>
            <h1 className="text-lg font-bold text-white leading-tight">Asistente IA</h1>
            <div className="flex items-center gap-1.5">
              <span className="w-1.5 h-1.5 rounded-full bg-emerald-500 animate-pulse" />
              <span className="text-[10px] text-slate-400 font-bold uppercase tracking-widest">En Línea</span>
            </div>
          </div>
        </div>

        <div className="flex items-center gap-2">
          <button 
            onClick={() => setIsAudioEnabled(!isAudioEnabled)}
            className={cn(
              "p-2.5 rounded-xl border transition-all",
              isAudioEnabled ? "bg-indigo-500/10 border-indigo-500/20 text-indigo-400" : "bg-white/5 border-white/10 text-slate-500"
            )}
          >
            {isAudioEnabled ? <Volume2 size={20} /> : <VolumeX size={20} />}
          </button>
          <button 
            onClick={clearChat}
            className="p-2.5 bg-white/5 border border-white/10 rounded-xl text-slate-400 hover:text-red-400 hover:bg-red-400/10 transition-all"
          >
            <Trash2 size={20} />
          </button>
        </div>
      </header>

      {/* Chat Area */}
      <main className="flex-1 overflow-y-auto p-6 space-y-6 scrollbar-thin scrollbar-thumb-white/10">
        {messages.length === 0 && (
          <div className="h-full flex flex-col items-center justify-center text-center space-y-4 opacity-40">
            <div className="w-20 h-20 rounded-3xl bg-white/5 flex items-center justify-center">
              <Sparkles size={40} className="text-indigo-400" />
            </div>
            <div>
              <p className="text-xl font-bold text-white">¿En qué puedo ayudarte?</p>
              <p className="text-sm text-slate-400 max-w-xs mx-auto">
                Puedes registrar consumos, bajas, ventas o consultar el estado de tus lotes por voz o texto.
              </p>
            </div>
            <div className="grid grid-cols-1 gap-2 w-full max-w-xs pt-4">
              {["¿Cómo va el lote 102?", "Registrar baja de 5 pollos", "Consultar stock de alimento"].map((hint, i) => (
                <button 
                  key={i}
                  onClick={() => handleSend(hint)}
                  className="px-4 py-2 bg-white/5 border border-white/5 rounded-xl text-xs text-slate-300 hover:bg-white/10 transition-all text-left"
                >
                  {hint}
                </button>
              ))}
            </div>
          </div>
        )}

        {messages.map((msg, i) => (
          <motion.div
            key={msg.id}
            initial={{ opacity: 0, y: 10, scale: 0.95 }}
            animate={{ opacity: 1, y: 0, scale: 1 }}
            className={cn(
              "flex w-full",
              msg.role === 'user' ? "justify-end" : "justify-start"
            )}
          >
            <div className={cn(
              "flex gap-3 max-w-[85%] md:max-w-[70%]",
              msg.role === 'user' ? "flex-row-reverse" : "flex-row"
            )}>
              <div className={cn(
                "w-8 h-8 rounded-lg flex-shrink-0 flex items-center justify-center mt-1",
                msg.role === 'user' ? "bg-slate-700 text-slate-300" : "bg-indigo-600 text-white"
              )}>
                {msg.role === 'user' ? <User size={16} /> : <Bot size={16} />}
              </div>
              <div className={cn(
                "p-4 rounded-2xl shadow-lg relative",
                msg.role === 'user' 
                  ? "bg-slate-800 text-slate-200 rounded-tr-none" 
                  : "bg-indigo-600/90 backdrop-blur-sm text-white rounded-tl-none border border-white/10"
              )}>
                {msg.isVoice && (
                  <div className="flex items-center gap-1.5 mb-1 opacity-60">
                    <Mic size={10} />
                    <span className="text-[10px] font-bold uppercase tracking-widest">Dictado por voz</span>
                  </div>
                )}
                <p className="text-sm leading-relaxed whitespace-pre-wrap">{msg.content}</p>
                <span className="text-[9px] opacity-40 mt-2 block text-right font-medium">
                  {msg.timestamp.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
                </span>
              </div>
            </div>
          </motion.div>
        ))}

        {isTyping && (
          <div className="flex justify-start">
            <div className="flex gap-3">
              <div className="w-8 h-8 rounded-lg bg-indigo-600 text-white flex items-center justify-center">
                <Bot size={16} />
              </div>
              <div className="bg-white/5 border border-white/10 p-4 rounded-2xl rounded-tl-none flex items-center gap-2">
                <div className="flex gap-1">
                  <span className="w-1.5 h-1.5 rounded-full bg-indigo-400 animate-bounce [animation-delay:-0.3s]" />
                  <span className="w-1.5 h-1.5 rounded-full bg-indigo-400 animate-bounce [animation-delay:-0.15s]" />
                  <span className="w-1.5 h-1.5 rounded-full bg-indigo-400 animate-bounce" />
                </div>
              </div>
            </div>
          </div>
        )}
        <div ref={messagesEndRef} />
      </main>

      {/* Input Area */}
      <footer className="p-6 bg-white/5 border-t border-white/5">
        <div className="max-w-4xl mx-auto relative">
          <div className="flex gap-3 items-end">
            <div className="flex-1 relative group">
              <input
                type="text"
                value={input}
                onChange={(e) => setInput(e.target.value)}
                onKeyPress={(e) => e.key === 'Enter' && handleSend()}
                placeholder={isRecording ? "Escuchando..." : "Pregunta algo al asistente..."}
                disabled={isRecording}
                className="w-full bg-slate-900/80 border border-white/10 rounded-2xl pl-4 pr-12 py-4 text-white focus:outline-none focus:ring-2 focus:ring-indigo-500/50 transition-all placeholder:text-slate-600"
              />
              <button 
                onClick={() => handleSend()}
                disabled={!input.trim() || isTyping || isRecording}
                className="absolute right-2 top-1/2 -translate-y-1/2 p-2 text-indigo-400 hover:text-indigo-300 disabled:opacity-0 transition-all"
              >
                <Send size={24} />
              </button>
            </div>
            
            <button
              onMouseDown={startRecording}
              onMouseUp={stopRecording}
              onTouchStart={startRecording}
              onTouchEnd={stopRecording}
              className={cn(
                "w-14 h-14 rounded-2xl flex items-center justify-center transition-all shadow-xl",
                isRecording 
                  ? "bg-red-500 text-white animate-pulse scale-110" 
                  : "bg-indigo-600 hover:bg-indigo-700 text-white"
              )}
            >
              {isRecording ? <MicOff size={28} /> : <Mic size={28} />}
            </button>
          </div>
          
          {isRecording && (
            <motion.div 
              initial={{ opacity: 0, y: 10 }}
              animate={{ opacity: 1, y: 0 }}
              className="absolute -top-12 left-1/2 -translate-x-1/2 flex items-center gap-2 px-4 py-2 bg-red-500 rounded-full text-white text-xs font-bold shadow-lg"
            >
              <span className="w-2 h-2 rounded-full bg-white animate-ping" />
              GRABANDO AUDIO... SUELTA PARA ENVIAR
            </motion.div>
          )}
        </div>
      </main>
    </div>
  )
}
