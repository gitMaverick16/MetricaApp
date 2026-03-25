export interface Pedido {
  id: number
  cliente: string
  estado: string
  fecha: string
  numeroPedido: string
  total: number
}

export type PedidoPayload = Omit<Pedido, 'id'>

export interface LoginResponse {
  token: string
  expiresIn: number
}

export interface LoginRequest {
  email: string
  password: string
}

export interface ProblemDetails {
  type?: string
  title?: string
  status?: number
  detail?: string
}
