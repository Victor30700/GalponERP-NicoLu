"use client";

import React, { createContext, useContext, useEffect, useState } from "react";
import { 
  onAuthStateChanged, 
  signOut as firebaseSignOut, 
  User
} from "firebase/auth";
import { auth } from "@/config/firebase";
import { api } from "@/lib/api";

interface UserProfile {
  id: string;
  nombre: string;
  apellidos: string;
  email: string;
  rol: string;
  telefono: string;
  isActive: boolean;
}

interface AuthResponse {
  idToken: string;
  refreshToken: string;
  email: string;
  expiresIn: number;
}

interface AuthContextType {
  user: User | null;
  profile: UserProfile | null;
  loading: boolean;
  token: string | null;
  login: (email: string, pass: string) => Promise<void>;
  logout: () => Promise<void>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider = ({ children }: { children: React.ReactNode }) => {
  const [user, setUser] = useState<User | null>(null);
  const [profile, setProfile] = useState<UserProfile | null>(null);
  const [token, setToken] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  const fetchProfile = async (retryCount = 0) => {
    try {
      console.log("Intentando obtener perfil del usuario...");
      const userProfile = await api.get<UserProfile>('/api/Usuarios/me');
      setProfile(userProfile);
      console.log("Perfil obtenido con éxito:", userProfile.email);
    } catch (error: any) {
      console.error(`Error fetching profile (intento ${retryCount + 1}):`, error.message);
      
      // Si falla con 401 y tenemos un refresh token, intentamos refrescar la sesión una vez
      if (error.message?.includes('401') && localStorage.getItem("refreshToken") && retryCount === 0) {
        console.log("Token probablemente expirado (401), intentando refrescar sesión...");
        const newToken = await refreshSession();
        if (newToken) {
          // Si el refresh fue exitoso, reintentamos UNA vez
          return fetchProfile(1);
        }
      }
      
      // Si llegamos aquí es que falló definitivamente o no era un 401
      setProfile(null);
      
      // Si no es un 401 (ej: 404), es probable que el usuario no exista en DB local
      if (error.message?.includes('404')) {
        console.warn("Usuario no encontrado en base de datos local.");
      }
    }
  };

  const refreshSession = async () => {
    try {
      const refreshToken = localStorage.getItem("refreshToken");
      if (!refreshToken) return;

      console.log("Refrescando sesión automáticamente...");
      const data = await api.post<AuthResponse>("/api/Auth/refresh-token", { refreshToken });
      
      localStorage.setItem("idToken", data.idToken);
      if (data.refreshToken) {
        localStorage.setItem("refreshToken", data.refreshToken);
      }
      setToken(data.idToken);
      console.log("Sesión refrescada con éxito");
      return data.idToken;
    } catch (error) {
      console.error("Error al refrescar sesión:", error);
      // No cerramos sesión automáticamente aquí para evitar bucles, 
      // pero el siguiente error 401 del API lo hará.
    }
  };

  useEffect(() => {
    let isMounted = true;

    const initAuth = async () => {
      setLoading(true);
      const savedToken = localStorage.getItem("idToken");
      
      if (savedToken) {
        setToken(savedToken);
        try {
          await fetchProfile();
        } catch (e) {
          console.error("Error inicializando perfil:", e);
        }
      }
      
      // Si no hay Firebase, terminamos la carga aquí
      if (!auth && isMounted) {
        setLoading(false);
      }
    };

    initAuth();

    // Configurar refresco automático cada 55 minutos
    const refreshInterval = setInterval(() => {
      if (localStorage.getItem("refreshToken")) {
        refreshSession();
      }
    }, 55 * 60 * 1000);

    let unsubscribe = () => {};

    if (auth) {
      unsubscribe = onAuthStateChanged(auth, async (fbUser) => {
        if (!isMounted) return;

        try {
          if (fbUser) {
            console.log("Firebase: Usuario detectado");
            setUser(fbUser);
            const t = await fbUser.getIdToken();
            setToken(t);
            localStorage.setItem("idToken", t);
            await fetchProfile();
          } else {
            console.log("Firebase: No hay sesión activa en SDK");
            setUser(null);
            
            // CRÍTICO: Si no hay usuario en Firebase pero SÍ hay un token manual, 
            // NO limpiamos el perfil ni el token. Esto permite el acceso manual.
            const hasManualToken = !!localStorage.getItem("idToken");
            if (!hasManualToken) {
              setProfile(null);
              setToken(null);
            } else if (!profile) {
              // Si hay token pero no hay perfil cargado aún, lo intentamos cargar
              await fetchProfile();
            }
          }
        } catch (error) {
          console.error("Error en onAuthStateChanged:", error);
        } finally {
          setLoading(false);
        }
      });
    } else {
      // Si no hay Firebase, dependemos totalmente de initAuth que ya se ejecutó
      // Solo nos aseguramos de que loading termine si initAuth no lo hizo
      setTimeout(() => {
        if (isMounted) setLoading(false);
      }, 500);
    }

    return () => {
      isMounted = false;
      unsubscribe();
      clearInterval(refreshInterval);
    };
  }, []);

  const login = async (email: string, password: string) => {
    try {
      console.log("AuthContext: Iniciando petición de login para", email);
      const data = await api.post<AuthResponse>("/api/Auth/login", { email, password });
      
      if (!data.idToken) {
        throw new Error("No se recibió el token de acceso");
      }

      localStorage.setItem("idToken", data.idToken);
      localStorage.setItem("refreshToken", data.refreshToken);
      setToken(data.idToken);
      
      console.log("AuthContext: Login exitoso, obteniendo perfil...");
      await fetchProfile();
      
    } catch (error: any) {
      console.error("AuthContext: Error en login:", error);
      throw new Error(error.message || "Credenciales incorrectas o error de servidor");
    }
  };

  const logout = async () => {
    localStorage.removeItem("idToken");
    localStorage.removeItem("refreshToken");
    setUser(null);
    setProfile(null);
    setToken(null);
    if (auth) await firebaseSignOut(auth);
    window.location.href = '/login';
  };

  return (
    <AuthContext.Provider value={{ user, profile, loading, token, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error("useAuth must be used within an AuthProvider");
  }
  return context;
};
