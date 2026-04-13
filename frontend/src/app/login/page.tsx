import { LoginBackground } from '@/components/auth/LoginBackground'
import { LoginForm } from '@/components/auth/LoginForm'

export default function LoginPage() {
  return (
    <main className="relative min-h-screen w-full flex items-center justify-center p-4">
      <LoginBackground />
      <LoginForm />
      
      {/* Footer minimalista */}
      <div className="absolute bottom-6 text-slate-500 text-xs font-medium tracking-wider uppercase z-10">
        © 2026 Pollos NicoLu • Gestión Inteligente
      </div>
    </main>
  )
}
