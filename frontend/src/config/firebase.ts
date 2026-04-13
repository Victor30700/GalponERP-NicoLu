import { initializeApp, getApps, getApp } from "firebase/app";
import { getAuth } from "firebase/auth";

const firebaseConfig = {
  apiKey: process.env.NEXT_PUBLIC_FIREBASE_API_KEY,
  authDomain: process.env.NEXT_PUBLIC_FIREBASE_AUTH_DOMAIN,
  projectId: process.env.NEXT_PUBLIC_FIREBASE_PROJECT_ID,
  storageBucket: process.env.NEXT_PUBLIC_FIREBASE_STORAGE_BUCKET,
  messagingSenderId: process.env.NEXT_PUBLIC_FIREBASE_MESSAGING_SENDER_ID,
  appId: process.env.NEXT_PUBLIC_FIREBASE_APP_ID,
};

// Inicialización segura
let app;
let auth: any = null;

try {
  if (firebaseConfig.apiKey && firebaseConfig.apiKey !== "OBTENER_DE_CONSOLA_FIREBASE") {
    app = getApps().length > 0 ? getApp() : initializeApp(firebaseConfig);
    auth = getAuth(app);
    console.log("Firebase inicializado correctamente");
  } else {
    console.warn("Firebase no se inicializó: Claves ausentes o por defecto.");
  }
} catch (error) {
  console.error("Error al inicializar Firebase:", error);
}

export { auth };
