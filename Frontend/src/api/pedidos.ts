import type { Pedido, PedidoPayload } from '../types/api'
import { apiFetch } from './client'

export function fetchPedidos(): Promise<Pedido[]> {
  return apiFetch<Pedido[]>('/api/pedidos')
}

export function fetchPedido(id: number): Promise<Pedido> {
  return apiFetch<Pedido>(`/api/pedidos/${id}`)
}

export function createPedido(payload: PedidoPayload): Promise<Pedido> {
  return apiFetch<Pedido>('/api/pedidos', {
    method: 'POST',
    body: JSON.stringify(payload),
  })
}

export function updatePedido(
  id: number,
  payload: PedidoPayload,
): Promise<Pedido> {
  return apiFetch<Pedido>(`/api/pedidos/${id}`, {
    method: 'PUT',
    body: JSON.stringify(payload),
  })
}

export function deletePedido(id: number): Promise<void> {
  return apiFetch<void>(`/api/pedidos/${id}`, {
    method: 'DELETE',
  })
}
