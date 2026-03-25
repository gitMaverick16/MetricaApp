import type { ProblemDetails } from '../types/api'
import { clearToken, getToken } from './authStorage'

export class ApiError extends Error {
  readonly status: number
  readonly problem?: ProblemDetails

  constructor(status: number, problem?: ProblemDetails) {
    super(problem?.detail ?? problem?.title ?? `Error HTTP ${status}`)
    this.name = 'ApiError'
    this.status = status
    this.problem = problem
  }
}

function getBaseUrl(): string {
  const raw = import.meta.env.VITE_API_URL?.trim()
  if (!raw) {
    console.warn(
      'VITE_API_URL no está definida; usa la URL por defecto solo en desarrollo.',
    )
    return 'https://localhost:7203'
  }
  return raw.replace(/\/$/, '')
}

export async function apiFetch<T>(
  path: string,
  init: RequestInit & { skipAuth?: boolean } = {},
): Promise<T> {
  const { skipAuth, ...rest } = init
  const headers = new Headers(rest.headers)

  if (
    rest.body &&
    typeof rest.body === 'string' &&
    !headers.has('Content-Type')
  ) {
    headers.set('Content-Type', 'application/json')
  }

  if (!skipAuth) {
    const token = getToken()
    if (token) headers.set('Authorization', `Bearer ${token}`)
  }

  const url = `${getBaseUrl()}${path.startsWith('/') ? path : `/${path}`}`
  const res = await fetch(url, { ...rest, headers })

  const isLogin = path.includes('/auth/login')
  if (res.status === 401 && !isLogin) {
    clearToken()
    window.dispatchEvent(new Event('auth:logout'))
  }

  if (!res.ok) {
    let problem: ProblemDetails | undefined
    const ct = res.headers.get('content-type') ?? ''
    if (ct.includes('application/json')) {
      try {
        problem = (await res.json()) as ProblemDetails
      } catch {
        /* ignore */
      }
    }
    throw new ApiError(res.status, problem)
  }

  if (res.status === 204) return undefined as T
  return (await res.json()) as T
}
