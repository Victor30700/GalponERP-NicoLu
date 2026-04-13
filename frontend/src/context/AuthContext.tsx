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

  const fetchProfile = async () => {
    try {
      const userProfile = await api.get<UserProfile>('/api/Usuarios/me');
      setProfile(userProfile);
    } catch (error) {
      console.error("Error fetching profile:", error);
      setProfile(null);
    }
  };

  useEffect(() => {
    // Si auth no está disponible (por error de claves), cargamos desde localStorage
    const savedToken = localStorage.getItem("idToken");
    if (savedToken) {
      setToken(savedToken);
      fetchProfile();
    }

    let unsubscribe = () => {};

    if (auth) {
      unsubscribe = onAuthStateChanged(auth, async (fbUser) => {
        if (fbUser) {
          setUser(fbUser);
          const t = await fbUser.getIdToken();
          setToken(t);
          localStorage.setItem("idToken", t);
          fetchProfile();
        } else {
          setUser(null);
        }
        setLoading(false);
      });
    } else {
      setLoading(false);
    }

    return () => unsubscribe();
  }, []);

  const login = async (email: string, password: string) => {
    try {
      const data = await api.post<AuthResponse>("/api/Auth/login", { email, password });
      
      localStorage.setItem("idToken", data.idToken);
      localStorage.setItem("refreshToken", data.refreshToken);
      setToken(data.idToken);
      
      await fetchProfile();
      
    } catch (error: any) {
      throw new Error(error.message || "Credenciales incorrectas");
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
