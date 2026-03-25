import { Navigate, Route, Routes } from 'react-router-dom'
import { ProtectedRoute } from './components/ProtectedRoute'
import { LoginPage } from './pages/LoginPage'
import { PedidosPage } from './pages/PedidosPage'

export default function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route
        path="/pedidos"
        element={
          <ProtectedRoute>
            <PedidosPage />
          </ProtectedRoute>
        }
      />
      <Route path="/" element={<Navigate to="/pedidos" replace />} />
      <Route path="*" element={<Navigate to="/pedidos" replace />} />
    </Routes>
  )
}
