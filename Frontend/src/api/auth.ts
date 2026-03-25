import type { LoginRequest, LoginResponse } from '../types/api'
import { apiFetch } from './client'
import { setToken } from './authStorage'

export async function login(credentials: LoginRequest): Promise<LoginResponse> {
  const data = await apiFetch<LoginResponse>('/auth/login', {
    method: 'POST',
    body: JSON.stringify(credentials),
    skipAuth: true,
  })
  setToken(data.token)
  return data
}
