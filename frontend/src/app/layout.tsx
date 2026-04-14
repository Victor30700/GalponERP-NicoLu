import type { Metadata } from "next";
import { Geist, Geist_Mono } from "next/font/google";
import "./globals.css";
import { AuthProvider } from "@/context/AuthContext";
import { ThemeProviderWrapper } from "@/components/providers/ThemeProviderWrapper";
import { QueryProvider } from "@/components/providers/QueryProvider";
import { ToasterWrapper } from "@/components/shared/ToasterWrapper";

const geistSans = Geist({
  variable: "--font-geist-sans",
  subsets: ["latin"],
});

const geistMono = Geist_Mono({
  variable: "--font-geist-mono",
  subsets: ["latin"],
});

export const metadata: Metadata = {
  title: "GalponERP - Pollos NicoLu",
  description: "Sistema de gestión avícola integral para producción de pollos",
  manifest: "/manifest.json",
  appleWebApp: {
    capable: true,
    statusBarStyle: "default",
    title: "GalponERP",
  },
  formatDetection: {
    telephone: false,
  },
  icons: {
    apple: "/window.svg",
  },
};

export const viewport = {
  themeColor: "#10b981",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html
      lang="es"
      className={`${geistSans.variable} ${geistMono.variable} h-full antialiased dark`}
      suppressHydrationWarning
    >
      <body className="min-h-full flex flex-col bg-background text-foreground transition-colors duration-300 dark" suppressHydrationWarning>
        <ThemeProviderWrapper>
          <AuthProvider>
            <QueryProvider>
              {children}
              <ToasterWrapper />
            </QueryProvider>
          </AuthProvider>
        </ThemeProviderWrapper>
      </body>
    </html>
  );
}
