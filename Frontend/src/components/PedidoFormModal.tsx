import { useState, type FormEvent } from 'react'
import type { Pedido, PedidoPayload } from '../types/api'
import { datetimeLocalToApi, isoToDatetimeLocal } from '../utils/date'

type Props = {
  open: boolean
  mode: 'create' | 'edit'
  initial?: Pedido | null
  loading: boolean
  serverError?: string | null
  onSubmit: (payload: PedidoPayload) => Promise<void>
  onClose: () => void
}

function buildInitialState(mode: 'create' | 'edit', initial?: Pedido | null) {
  if (mode === 'edit' && initial) {
    return {
      cliente: initial.cliente,
      estado: initial.estado,
      fechaLocal: isoToDatetimeLocal(initial.fecha),
      numeroPedido: initial.numeroPedido,
      totalStr: String(initial.total),
    }
  }
  return {
    cliente: '',
    estado: 'Registrado',
    fechaLocal: '',
    numeroPedido: '',
    totalStr: '',
  }
}

export function PedidoFormModal({
  open,
  mode,
  initial,
  loading,
  serverError,
  onSubmit,
  onClose,
}: Props) {
  const [cliente, setCliente] = useState(
    () => buildInitialState(mode, initial).cliente,
  )
  const [estado, setEstado] = useState(
    () => buildInitialState(mode, initial).estado,
  )
  const [fechaLocal, setFechaLocal] = useState(
    () => buildInitialState(mode, initial).fechaLocal,
  )
  const [numeroPedido, setNumeroPedido] = useState(
    () => buildInitialState(mode, initial).numeroPedido,
  )
  const [totalStr, setTotalStr] = useState(
    () => buildInitialState(mode, initial).totalStr,
  )
  const [formError, setFormError] = useState<string | null>(null)

  if (!open) return null

  function validate(): PedidoPayload | null {
    const c = cliente.trim()
    const e = estado.trim()
    const np = numeroPedido.trim()
    if (!c || !e || !np) {
      setFormError('Completa cliente, estado y número de pedido.')
      return null
    }
    if (!fechaLocal) {
      setFormError('Indica la fecha.')
      return null
    }
    const total = Number(totalStr.replace(',', '.'))
    if (!Number.isFinite(total) || total <= 0) {
      setFormError('El total debe ser un número mayor que 0.')
      return null
    }
    return {
      cliente: c,
      estado: e,
      fecha: datetimeLocalToApi(fechaLocal),
      numeroPedido: np,
      total,
    }
  }

  async function handleSubmit(ev: FormEvent) {
    ev.preventDefault()
    setFormError(null)
    const payload = validate()
    if (!payload) return
    await onSubmit(payload)
  }

  return (
    <div className="fixed inset-0 z-40 flex items-center justify-center p-4">
      <button
        type="button"
        className="absolute inset-0 bg-slate-900/40"
        aria-label="Cerrar"
        onClick={onClose}
      />
      <div
        role="dialog"
        aria-modal="true"
        aria-labelledby="pedido-form-title"
        className="relative z-10 max-h-[90dvh] w-full max-w-lg overflow-y-auto rounded-xl border border-slate-200 bg-white p-6 shadow-xl"
      >
        <h2
          id="pedido-form-title"
          className="text-lg font-semibold text-slate-900"
        >
          {mode === 'create' ? 'Nuevo pedido' : 'Editar pedido'}
        </h2>

        <form className="mt-4 space-y-4" onSubmit={handleSubmit}>
          <div>
            <label htmlFor="cliente" className="text-sm font-medium text-slate-700">
              Cliente
            </label>
            <input
              id="cliente"
              value={cliente}
              onChange={(e) => setCliente(e.target.value)}
              className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 outline-none focus:ring-2 focus:ring-violet-500"
              disabled={loading}
              required
            />
          </div>
          <div>
            <label htmlFor="estado" className="text-sm font-medium text-slate-700">
              Estado
            </label>
            <input
              id="estado"
              value={estado}
              onChange={(e) => setEstado(e.target.value)}
              className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 outline-none focus:ring-2 focus:ring-violet-500"
              disabled={loading}
              required
            />
          </div>
          <div>
            <label htmlFor="fecha" className="text-sm font-medium text-slate-700">
              Fecha
            </label>
            <input
              id="fecha"
              type="datetime-local"
              value={fechaLocal}
              onChange={(e) => setFechaLocal(e.target.value)}
              className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 outline-none focus:ring-2 focus:ring-violet-500"
              disabled={loading}
              required
            />
          </div>
          <div>
            <label
              htmlFor="numeroPedido"
              className="text-sm font-medium text-slate-700"
            >
              Número de pedido
            </label>
            <input
              id="numeroPedido"
              value={numeroPedido}
              onChange={(e) => setNumeroPedido(e.target.value)}
              className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 outline-none focus:ring-2 focus:ring-violet-500"
              disabled={loading}
              required
            />
          </div>
          <div>
            <label htmlFor="total" className="text-sm font-medium text-slate-700">
              Total
            </label>
            <input
              id="total"
              type="text"
              inputMode="decimal"
              value={totalStr}
              onChange={(e) => setTotalStr(e.target.value)}
              className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 outline-none focus:ring-2 focus:ring-violet-500"
              disabled={loading}
              placeholder="0.00"
              required
            />
          </div>

          {formError && (
            <p role="alert" className="text-sm text-red-600">
              {formError}
            </p>
          )}
          {serverError && (
            <p role="alert" className="text-sm text-red-700">
              {serverError}
            </p>
          )}

          <div className="flex justify-end gap-3 pt-2">
            <button
              type="button"
              onClick={onClose}
              disabled={loading}
              className="rounded-lg border border-slate-300 px-4 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50 disabled:opacity-50"
            >
              Cancelar
            </button>
            <button
              type="submit"
              disabled={loading}
              className="inline-flex items-center rounded-lg bg-violet-600 px-4 py-2 text-sm font-medium text-white hover:bg-violet-700 disabled:cursor-not-allowed disabled:opacity-60"
            >
              {loading && (
                <span
                  className="mr-2 inline-block size-4 animate-spin rounded-full border-2 border-white border-t-transparent"
                  aria-hidden
                />
              )}
              {mode === 'create' ? 'Crear' : 'Guardar'}
            </button>
          </div>
        </form>
      </div>
    </div>
  )
}
