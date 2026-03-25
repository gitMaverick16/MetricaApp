import { useCallback, useEffect, useState } from 'react'
import {
  createPedido,
  deletePedido,
  fetchPedidos,
  updatePedido,
} from '../api/pedidos'
import { ApiError } from '../api/client'
import { ConfirmDialog } from '../components/ConfirmDialog'
import { PedidoFormModal } from '../components/PedidoFormModal'
import { useAuth } from '../hooks/useAuth'
import type { Pedido, PedidoPayload } from '../types/api'

function formatMoney(n: number): string {
  return new Intl.NumberFormat('es-ES', {
    style: 'currency',
    currency: 'EUR',
  }).format(n)
}

function formatDate(iso: string): string {
  try {
    return new Intl.DateTimeFormat('es-ES', {
      dateStyle: 'short',
      timeStyle: 'short',
    }).format(new Date(iso))
  } catch {
    return iso
  }
}

export function PedidosPage() {
  const { logout } = useAuth()
  const [pedidos, setPedidos] = useState<Pedido[]>([])
  const [loading, setLoading] = useState(true)
  const [refreshing, setRefreshing] = useState(false)
  const [forbidden, setForbidden] = useState(false)
  const [listError, setListError] = useState<string | null>(null)

  const [formOpen, setFormOpen] = useState(false)
  const [formMode, setFormMode] = useState<'create' | 'edit'>('create')
  const [editing, setEditing] = useState<Pedido | null>(null)
  const [formLoading, setFormLoading] = useState(false)
  const [formApiError, setFormApiError] = useState<string | null>(null)

  const [deleteTarget, setDeleteTarget] = useState<Pedido | null>(null)
  const [deleteLoading, setDeleteLoading] = useState(false)

  const load = useCallback(async (isRefresh = false) => {
    if (isRefresh) setRefreshing(true)
    else setLoading(true)
    setListError(null)
    setForbidden(false)
    try {
      const data = await fetchPedidos()
      setPedidos(data)
    } catch (e) {
      if (e instanceof ApiError) {
        if (e.status === 403) {
          setForbidden(true)
          setPedidos([])
        } else {
          setListError(e.problem?.detail ?? e.message)
        }
      } else {
        setListError('No se pudo cargar el listado.')
      }
    } finally {
      setLoading(false)
      setRefreshing(false)
    }
  }, [])

  useEffect(() => {
    void load()
  }, [load])

  function openCreate() {
    setFormMode('create')
    setEditing(null)
    setFormApiError(null)
    setFormOpen(true)
  }

  function openEdit(p: Pedido) {
    setFormMode('edit')
    setEditing(p)
    setFormApiError(null)
    setFormOpen(true)
  }

  function mapApiError(e: unknown): string {
    if (e instanceof ApiError) {
      if (e.status === 409)
        return e.problem?.detail ?? 'Conflicto: número de pedido duplicado.'
      if (e.status === 400) return e.problem?.detail ?? 'Datos no válidos.'
      if (e.status === 403)
        return 'No tienes permiso para gestionar pedidos.'
      return e.problem?.detail ?? e.message
    }
    return 'Ha ocurrido un error.'
  }

  async function handleFormSubmit(payload: PedidoPayload) {
    setFormLoading(true)
    setFormApiError(null)
    try {
      if (formMode === 'create') {
        const created = await createPedido(payload)
        setPedidos((prev) => [created, ...prev])
      } else if (editing) {
        const updated = await updatePedido(editing.id, payload)
        setPedidos((prev) =>
          prev.map((p) => (p.id === updated.id ? updated : p)),
        )
      }
      setFormOpen(false)
    } catch (e) {
      setFormApiError(mapApiError(e))
    } finally {
      setFormLoading(false)
    }
  }

  async function handleDeleteConfirm() {
    if (!deleteTarget) return
    setDeleteLoading(true)
    try {
      await deletePedido(deleteTarget.id)
      setPedidos((prev) => prev.filter((p) => p.id !== deleteTarget.id))
      setDeleteTarget(null)
    } catch (e) {
      setListError(mapApiError(e))
    } finally {
      setDeleteLoading(false)
    }
  }

  return (
    <div className="min-h-dvh">
      <header className="border-b border-slate-200 bg-white">
        <div className="mx-auto flex max-w-6xl flex-wrap items-center justify-between gap-4 px-4 py-4">
          <div>
            <h1 className="text-xl font-semibold text-slate-900">Pedidos</h1>
            <p className="text-sm text-slate-600">
              Listado y gestión (requiere rol Admin en la API).
            </p>
          </div>
          <div className="flex flex-wrap items-center gap-2">
            <button
              type="button"
              onClick={() => void load(true)}
              disabled={loading || refreshing}
              className="rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50 disabled:opacity-50"
            >
              {refreshing ? 'Actualizando…' : 'Refrescar'}
            </button>
            <button
              type="button"
              onClick={openCreate}
              disabled={forbidden}
              className="rounded-lg bg-violet-600 px-3 py-2 text-sm font-medium text-white hover:bg-violet-700 disabled:cursor-not-allowed disabled:opacity-50"
            >
              Nuevo pedido
            </button>
            <button
              type="button"
              onClick={logout}
              className="rounded-lg border border-slate-300 px-3 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50"
            >
              Cerrar sesión
            </button>
          </div>
        </div>
      </header>

      <main className="mx-auto max-w-6xl px-4 py-8">
        {forbidden && (
          <div
            role="alert"
            className="mb-6 rounded-xl border border-amber-200 bg-amber-50 px-4 py-3 text-amber-900"
          >
            <p className="font-medium">No tienes permiso para gestionar pedidos.</p>
            <p className="mt-1 text-sm text-amber-800">
              Tu usuario no tiene rol Admin. Inicia sesión con una cuenta
              autorizada para usar esta sección.
            </p>
          </div>
        )}

        {listError && !forbidden && (
          <div
            role="alert"
            className="mb-6 rounded-xl border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-800"
          >
            {listError}
          </div>
        )}

        {loading ? (
          <div className="space-y-3" aria-busy="true" aria-label="Cargando pedidos">
            {[1, 2, 3, 4, 5].map((i) => (
              <div
                key={i}
                className="h-12 animate-pulse rounded-lg bg-slate-200/80"
              />
            ))}
          </div>
        ) : !forbidden && pedidos.length === 0 && !listError ? (
          <p className="rounded-xl border border-dashed border-slate-300 bg-white px-6 py-12 text-center text-slate-600">
            No hay pedidos. Crea uno nuevo con el botón «Nuevo pedido».
          </p>
        ) : !forbidden ? (
          <div className="overflow-x-auto rounded-xl border border-slate-200 bg-white shadow-sm">
            <table className="min-w-full text-left text-sm">
              <thead className="border-b border-slate-200 bg-slate-50 text-slate-700">
                <tr>
                  <th scope="col" className="px-4 py-3 font-medium">
                    Nº pedido
                  </th>
                  <th scope="col" className="px-4 py-3 font-medium">
                    Cliente
                  </th>
                  <th scope="col" className="px-4 py-3 font-medium">
                    Estado
                  </th>
                  <th scope="col" className="px-4 py-3 font-medium">
                    Fecha
                  </th>
                  <th scope="col" className="px-4 py-3 font-medium">
                    Total
                  </th>
                  <th scope="col" className="px-4 py-3 font-medium text-right">
                    Acciones
                  </th>
                </tr>
              </thead>
              <tbody>
                {pedidos.map((p) => (
                  <tr
                    key={p.id}
                    className="border-b border-slate-100 last:border-0"
                  >
                    <td className="px-4 py-3 font-mono text-slate-900">
                      {p.numeroPedido}
                    </td>
                    <td className="px-4 py-3 text-slate-800">{p.cliente}</td>
                    <td className="px-4 py-3 text-slate-700">{p.estado}</td>
                    <td className="px-4 py-3 text-slate-600">
                      {formatDate(p.fecha)}
                    </td>
                    <td className="px-4 py-3 text-slate-900">
                      {formatMoney(p.total)}
                    </td>
                    <td className="px-4 py-3 text-right">
                      <button
                        type="button"
                        onClick={() => openEdit(p)}
                        className="mr-2 rounded-md text-violet-700 hover:underline"
                      >
                        Editar
                      </button>
                      <button
                        type="button"
                        onClick={() => setDeleteTarget(p)}
                        className="rounded-md text-red-700 hover:underline"
                      >
                        Eliminar
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        ) : null}
      </main>

      <PedidoFormModal
        key={formOpen ? `${formMode}-${editing?.id ?? 'new'}` : 'closed'}
        open={formOpen}
        mode={formMode}
        initial={editing}
        loading={formLoading}
        serverError={formApiError}
        onClose={() => {
          if (!formLoading) {
            setFormOpen(false)
            setFormApiError(null)
          }
        }}
        onSubmit={async (payload) => {
          await handleFormSubmit(payload)
        }}
      />

      <ConfirmDialog
        open={Boolean(deleteTarget)}
        title="Eliminar pedido"
        danger
        loading={deleteLoading}
        confirmLabel="Eliminar"
        onCancel={() => !deleteLoading && setDeleteTarget(null)}
        onConfirm={() => void handleDeleteConfirm()}
      >
        {deleteTarget && (
          <>
            ¿Eliminar el pedido{' '}
            <strong className="font-mono">{deleteTarget.numeroPedido}</strong>?
            La eliminación es lógica en el servidor.
          </>
        )}
      </ConfirmDialog>
    </div>
  )
}
