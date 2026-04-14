'use client'

import { useState, useEffect, useRef } from 'react'
import { useAuth } from '@/context/AuthContext'
import { useRouter } from 'next/navigation'
import { motion, AnimatePresence } from 'framer-motion'
import { 
  Send, 
  Mic, 
  MicOff, 
  Bot, 
  User, 
  Volume2, 
  VolumeX,
  Trash2,
  Sparkles,
  MessageSquare,
  Plus,
  History,
  Menu,
  X,
  Loader2
} from 'lucide-react'
import { api } from '@/lib/api'
import { cn } from '@/lib/utils'
import { toast } from 'sonner'
import { confirmDestructiveAction } from '@/lib/swal'

interface Message {
  id: string
  role: 'user' | 'assistant'
  content: string
  timestamp: Date
  isVoice?: boolean
}

interface Conversacion {
  id: string
  titulo: string
  fechaInicio: string
  totalMensajes: number
}

export default function ChatPage() {
  const { profile, user, loading } = useAuth()
  const router = useRouter()
  
  // State
  const [conversaciones, setConversaciones] = useState<Conversacion[]>([])
  const [conversacionId, setConversacionId] = useState<string | null>(null)
  const [messages, setMessages] = useState<Message[]>([])
  const [input, setInput] = useState('')
  const [isTyping, setIsTyping] = useState(false)
  const [isRecording, setIsRecording] = useState(false)
  const [isAudioEnabled, setIsAudioEnabled] = useState(true)
  const [isSidebarOpen, setIsSidebarOpen] = useState(false)
  const [isLoadingConversations, setIsLoadingConversations] = useState(true)
  const [isLoadingMessages, setIsLoadingMessages] = useState(false)
  
  const messagesEndRef = useRef<HTMLDivElement>(null)
  const mediaRecorderRef = useRef<MediaRecorder | null>(null)
  const audioChunksRef = useRef<Blob[]>([])

  // Load conversations on mount
  useEffect(() => {
    if (profile) {
      fetchConversaciones()
    }
  }, [profile])

  // Scroll to bottom when messages change
  useEffect(() => {
    scrollToBottom()
  }, [messages, isTyping])

  const fetchConversaciones = async () => {
    try {
      setIsLoadingConversations(true)
      const data = await api.get<Conversacion[]>('/api/Agentes/conversaciones')
      setConversaciones(data)
    } catch (error) {
      console.error("Error fetching conversations:", error)
    } finally {
      setIsLoadingConversations(false)
    }
  }

  const loadConversacion = async (id: string) => {
    try {
      setIsLoadingMessages(true)
      setConversacionId(id)
      const data = await api.get<any>(`/api/Agentes/conversaciones/${id}`)
      
      const formattedMessages: Message[] = data.mensajes.map((m: any) => ({
        id: m.id,
        role: m.rol === 'user' ? 'user' : 'assistant',
        content: m.contenido,
        timestamp: new Date(m.fecha)
      }))
      
      setMessages(formattedMessages)
      if (window.innerWidth < 768) setIsSidebarOpen(false)
    } catch (error) {
      toast.error("No se pudo cargar la conversación")
    } finally {
      setIsLoadingMessages(false)
    }
  }

  const deleteConversacion = async (id: string, e: React.MouseEvent) => {
    e.stopPropagation()
    
    const result = await confirmDestructiveAction(
      '¿Eliminar conversación?',
      'Esta acción no se puede deshacer y se borrará todo el historial.'
    );

    if (result.isConfirmed) {
      try {
        await api.delete(`/api/Agentes/conversaciones/${id}`)
        setConversaciones(prev => prev.filter(c => c.id !== id))
        if (conversacionId === id) {
          setConversacionId(null)
          setMessages([])
        }
        toast.success("Conversación eliminada")
      } catch (error) {
        toast.error("Error al eliminar")
      }
    }
  }

  const startNewChat = () => {
    setConversacionId(null)
    setMessages([])
    if (window.innerWidth < 768) setIsSidebarOpen(false)
  }

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
      
      if (!conversacionId) {
        setConversacionId(response.conversacionId)
        fetchConversaciones() // Refresh list to show new chat
      }
    } catch (error: any) {
      toast.error("Error al conectar con el asistente")
    } finally {
      setIsTyping(false)
    }
  }

  // Voice handling
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
      const idToken = await user?.getIdToken()
      const response = await fetch(`${process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5167'}/api/voice/upload`, {
        method: 'POST',
        headers: { 'Authorization': `Bearer ${idToken}` },
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
      if (!conversacionId) {
        setConversacionId(data.conversacionId)
        fetchConversaciones()
      }

      if (isAudioEnabled && data.respuestaAudioBase64) {
        const audio = new Audio(`data:audio/wav;base64,${data.respuestaAudioBase64}`)
        audio.play()
      }
    } catch (error) {
      toast.error("Error al procesar mensaje de voz")
    } finally {
      setIsTyping(false)
    }
  }

  if (loading) return null

  return (
    <div className="flex h-[calc(100vh-8rem)] md:h-[calc(100vh-6rem)] glass border border-border rounded-3xl overflow-hidden shadow-2xl relative">
      
      {/* Sidebar - Desktop List / Mobile Slide-over */}
      <aside className={cn(
        "absolute inset-y-0 left-0 z-40 w-72 bg-muted/50/80 backdrop-blur-xl border-r border-border transition-transform duration-300 md:relative md:translate-x-0",
        isSidebarOpen ? "translate-x-0" : "-translate-x-full"
      )}>
        <div className="flex flex-col h-full">
          <div className="p-4 border-b border-border">
            <button 
              onClick={startNewChat}
              className="w-full py-3 bg-indigo-600 hover:bg-indigo-700 text-white rounded-xl font-bold flex items-center justify-center gap-2 transition-all shadow-lg shadow-indigo-500/20"
            >
              <Plus size={18} /> Nuevo Chat
            </button>
          </div>

          <div className="flex-1 overflow-y-auto p-3 space-y-2 no-scrollbar">
            <h3 className="px-3 text-[10px] font-black text-muted-foreground uppercase tracking-widest mb-2">Historial</h3>
            
            {isLoadingConversations ? (
              <div className="flex justify-center p-8"><Loader2 className="animate-spin text-muted-foreground" /></div>
            ) : conversaciones.length === 0 ? (
              <p className="px-3 text-xs text-slate-600 italic">No hay chats previos</p>
            ) : (
              conversaciones.map(c => (
                <div 
                  key={c.id}
                  onClick={() => loadConversacion(c.id)}
                  className={cn(
                    "group relative p-3 rounded-xl cursor-pointer transition-all flex items-start gap-3",
                    conversacionId === c.id ? "bg-muted/50 text-foreground" : "text-muted-foreground hover:bg-muted/50 hover:text-slate-200"
                  )}
                >
                  <MessageSquare size={16} className="mt-1 flex-shrink-0" />
                  <div className="flex-1 min-w-0">
                    <p className="text-xs font-bold truncate">{c.titulo || `Chat ${new Date(c.fechaInicio).toLocaleDateString()}`}</p>
                    <p className="text-[10px] opacity-40">{c.totalMensajes} mensajes</p>
                  </div>
                  <button 
                    onClick={(e) => deleteConversacion(c.id, e)}
                    className="opacity-0 group-hover:opacity-100 p-1.5 hover:text-red-400 transition-opacity"
                  >
                    <Trash2 size={14} />
                  </button>
                </div>
              ))
            )}
          </div>

          <div className="p-4 border-t border-border text-[10px] text-slate-600 font-bold uppercase tracking-tighter text-center">
            GalponERP Asistente IA v1.0
          </div>
        </div>
      </aside>

      {/* Main Chat View */}
      <div className="flex-1 flex flex-col min-w-0 bg-muted/50 relative">
        
        {/* Header */}
        <header className="px-6 py-4 border-b border-border bg-muted/50 flex items-center justify-between">
          <div className="flex items-center gap-3">
            <button 
              onClick={() => setIsSidebarOpen(!isSidebarOpen)}
              className="md:hidden p-2 text-muted-foreground hover:text-foreground"
            >
              {isSidebarOpen ? <X size={20} /> : <Menu size={20} />}
            </button>
            <div className="w-10 h-10 rounded-2xl bg-indigo-500/20 flex items-center justify-center text-indigo-400">
              <Bot size={24} />
            </div>
            <div>
              <h1 className="text-lg font-bold text-foreground leading-tight">Asistente IA</h1>
              <div className="flex items-center gap-1.5">
                <span className="w-1.5 h-1.5 rounded-full bg-emerald-500 animate-pulse" />
                <span className="text-[10px] text-muted-foreground font-bold uppercase tracking-widest">En Línea</span>
              </div>
            </div>
          </div>

          <div className="flex items-center gap-2">
            <button 
              onClick={() => setIsAudioEnabled(!isAudioEnabled)}
              className={cn(
                "p-2.5 rounded-xl border transition-all",
                isAudioEnabled ? "bg-indigo-500/10 border-indigo-500/20 text-indigo-400" : "bg-muted/50 border-border text-muted-foreground"
              )}
              title="Voz a texto"
            >
              {isAudioEnabled ? <Volume2 size={20} /> : <VolumeX size={20} />}
            </button>
          </div>
        </header>

        {/* Messages */}
        <main className="flex-1 overflow-y-auto p-6 space-y-6 scrollbar-thin scrollbar-thumb-white/10">
          {isLoadingMessages ? (
            <div className="h-full flex items-center justify-center"><Loader2 className="animate-spin text-indigo-500" size={32} /></div>
          ) : messages.length === 0 ? (
            <div className="h-full flex flex-col items-center justify-center text-center space-y-4 opacity-40">
              <div className="w-20 h-20 rounded-3xl bg-muted/50 flex items-center justify-center">
                <Sparkles size={40} className="text-indigo-400" />
              </div>
              <div>
                <p className="text-xl font-bold text-foreground">¿En qué puedo ayudarte hoy?</p>
                <p className="text-sm text-muted-foreground max-w-xs mx-auto">
                  Soy tu asistente experto en avicultura. Pregúntame sobre tus lotes, inventario o finanzas.
                </p>
              </div>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-2 w-full max-w-md pt-4">
                {["¿Estado del lote activo?", "Registrar baja de 5 pollos", "Consultar stock de alimento", "Resumen de ventas ayer"].map((hint, i) => (
                  <button 
                    key={i}
                    onClick={() => handleSend(hint)}
                    className="px-4 py-3 bg-muted/50 border border-border rounded-xl text-[10px] font-bold uppercase tracking-wider text-slate-300 hover:bg-muted/50 transition-all text-left"
                  >
                    {hint}
                  </button>
                ))}
              </div>
            </div>
          ) : (
            messages.map((msg) => (
              <motion.div
                key={msg.id}
                initial={{ opacity: 0, y: 10 }}
                animate={{ opacity: 1, y: 0 }}
                className={cn("flex w-full", msg.role === 'user' ? "justify-end" : "justify-start")}
              >
                <div className={cn("flex gap-3 max-w-[90%] md:max-w-[75%]", msg.role === 'user' ? "flex-row-reverse" : "flex-row")}>
                  <div className={cn(
                    "w-8 h-8 rounded-lg flex-shrink-0 flex items-center justify-center mt-1",
                    msg.role === 'user' ? "bg-slate-700 text-slate-300" : "bg-indigo-600 text-white"
                  )}>
                    {msg.role === 'user' ? <User size={16} /> : <Bot size={16} />}
                  </div>
                  <div className={cn(
                    "p-4 rounded-2xl shadow-xl",
                    msg.role === 'user' 
                      ? "bg-slate-800 text-slate-200 rounded-tr-none border border-border" 
                      : "bg-indigo-600/90 text-white rounded-tl-none border border-border"
                  )}>
                    <p className="text-sm leading-relaxed whitespace-pre-wrap">{msg.content}</p>
                    <span className="text-[9px] opacity-40 mt-2 block text-right">
                      {msg.timestamp.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
                    </span>
                  </div>
                </div>
              </motion.div>
            ))
          )}

          {isTyping && (
            <div className="flex justify-start">
              <div className="flex gap-3">
                <div className="w-8 h-8 rounded-lg bg-indigo-600 text-white flex items-center justify-center"><Bot size={16} /></div>
                <div className="bg-muted/50 border border-border p-4 rounded-2xl rounded-tl-none flex items-center gap-2">
                  <span className="w-1.5 h-1.5 rounded-full bg-indigo-400 animate-bounce" style={{ animationDelay: '-0.3s' }} />
                  <span className="w-1.5 h-1.5 rounded-full bg-indigo-400 animate-bounce" style={{ animationDelay: '-0.15s' }} />
                  <span className="w-1.5 h-1.5 rounded-full bg-indigo-400 animate-bounce" />
                </div>
              </div>
            </div>
          )}
          <div ref={messagesEndRef} />
        </main>

        {/* Input */}
        <footer className="p-6 bg-muted/50/20 border-t border-border">
          <div className="max-w-4xl mx-auto flex gap-3 items-end">
            <div className="flex-1 relative">
              <input
                type="text"
                value={input}
                onChange={(e) => setInput(e.target.value)}
                onKeyPress={(e) => e.key === 'Enter' && handleSend()}
                placeholder={isRecording ? "Grabando..." : "Escribe tu consulta aquí..."}
                disabled={isRecording}
                className="w-full bg-muted/50 border border-border rounded-2xl pl-4 pr-12 py-4 text-sm text-foreground focus:ring-2 focus:ring-indigo-500/50 transition-all outline-none"
              />
              <button 
                onClick={() => handleSend()}
                disabled={!input.trim() || isTyping}
                className="absolute right-2 top-1/2 -translate-y-1/2 p-2 text-indigo-400 hover:text-indigo-300 transition-all"
              >
                <Send size={24} />
              </button>
            </div>
            
            <button
              onMouseDown={startRecording}
              onMouseUp={stopRecording}
              className={cn(
                "w-14 h-14 rounded-2xl flex items-center justify-center transition-all",
                isRecording ? "bg-red-500 animate-pulse" : "bg-indigo-600 hover:bg-indigo-700 text-white"
              )}
            >
              <Mic size={24} />
            </button>
          </div>
        </footer>
      </div>
    </div>
  )
}



