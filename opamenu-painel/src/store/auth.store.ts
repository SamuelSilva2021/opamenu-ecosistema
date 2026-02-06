import { create } from "zustand";
import { persist } from "zustand/middleware";
import type { UserInfo } from "@/features/auth/types";

interface AuthState {
  accessToken: string | null;
  refreshToken: string | null;
  user: UserInfo | null;
  isAuthenticated: boolean;
  requiresPayment: boolean;
  setAccessToken: (token: string, refreshToken: string, requiresPayment?: boolean) => void;
  setUser: (user: UserInfo) => void;
  logout: () => void;
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      accessToken: null,
      refreshToken: null,
      user: null,
      isAuthenticated: false,
      requiresPayment: false,
      setAccessToken: (token, refreshToken, requiresPayment = false) => set({ accessToken: token, refreshToken, isAuthenticated: !!token, requiresPayment }),
      setUser: (user) => set({ user }),
      logout: () => set({ accessToken: null, refreshToken: null, user: null, isAuthenticated: false, requiresPayment: false }),
    }),
    {
      name: "auth-storage",
    }
  )
);