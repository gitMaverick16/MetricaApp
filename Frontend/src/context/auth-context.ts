import { createContext } from 'react'
import type { LoginRequest } from '../types/api'

export type AuthContextValue = {
  token: string | null
  isAuthenticated: boolean
  login: (credentials: LoginRequest) => Promise<void>
  logout: () => void
}

export const AuthContext = createContext<AuthContextValue | null>(null)
