import { useCallback, useEffect, useMemo, useState, type ReactNode } from 'react'
import { useNavigate } from 'react-router-dom'
import { login as loginRequest } from '../api/auth'
import { clearToken, getToken } from '../api/authStorage'
import { ApiError } from '../api/client'
import type { LoginRequest } from '../types/api'
import { AuthContext } from './auth-context'

export function AuthProvider({ children }: { children: ReactNode }) {
  const navigate = useNavigate()
  const [token, setTokenState] = useState<string | null>(() => getToken())

  useEffect(() => {
    const onLogout = () => {
      setTokenState(null)
      navigate('/login', { replace: true })
    }
    window.addEventListener('auth:logout', onLogout)
    return () => window.removeEventListener('auth:logout', onLogout)
  }, [navigate])

  const login = useCallback(async (credentials: LoginRequest) => {
    try {
      const res = await loginRequest(credentials)
      setTokenState(res.token)
      navigate('/pedidos', { replace: true })
    } catch (e) {
      if (e instanceof ApiError && e.status === 401) {
        throw new Error('Email o contraseña incorrectos.')
      }
      if (e instanceof ApiError && e.problem?.detail) {
        throw new Error(e.problem.detail)
      }
      throw e
    }
  }, [navigate])

  const logout = useCallback(() => {
    clearToken()
    setTokenState(null)
    navigate('/login', { replace: true })
  }, [navigate])

  const value = useMemo(
    () => ({
      token,
      isAuthenticated: Boolean(token),
      login,
      logout,
    }),
    [token, login, logout],
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}
