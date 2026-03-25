using OrdersApp.Domain.Common;

namespace OrdersApp.Domain.Orders
{
    public sealed class Order
    {
        public int Id { get; private set; }
        public string NumeroPedido { get; private set; } = null!;
        public string Cliente { get; private set; } = null!;
        public string Estado { get; private set; } = null!;
        public decimal Total { get; private set; }
        public DateTime Fecha { get; private set; }
        public bool IsDeleted { get; private set; }
        public DateTime? DeletedAt { get; private set; }

        private Order()
        {
        }

        public static Order Create(
            string numeroPedido,
            string cliente,
            string estado,
            DateTime fecha,
            decimal total)
        {
            ValidateTotal(total);
            ValidateNumeroPedido(numeroPedido);
            ArgumentException.ThrowIfNullOrWhiteSpace(cliente);
            ArgumentException.ThrowIfNullOrWhiteSpace(estado);

            return new Order
            {
                NumeroPedido = numeroPedido.Trim(),
                Cliente = cliente.Trim(),
                Estado = estado.Trim(),
                Fecha = fecha,
                Total = total,
                IsDeleted = false
            };
        }

        public void MarkAsDeleted()
        {
            if (IsDeleted)
            {
                return;
            }

            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
        }

        public void UpdateDetails(
            string numeroPedido,
            string cliente,
            string estado,
            DateTime fecha,
            decimal total)
        {
            ValidateTotal(total);
            ValidateNumeroPedido(numeroPedido);
            ArgumentException.ThrowIfNullOrWhiteSpace(cliente);
            ArgumentException.ThrowIfNullOrWhiteSpace(estado);

            NumeroPedido = numeroPedido.Trim();
            Cliente = cliente.Trim();
            Estado = estado.Trim();
            Fecha = fecha;
            Total = total;
        }

        private static void ValidateTotal(decimal total)
        {
            if (total <= 0)
            {
                throw new OrderDomainException("El total debe ser mayor que cero.");
            }
        }

        private static void ValidateNumeroPedido(string numeroPedido)
        {
            if (string.IsNullOrWhiteSpace(numeroPedido))
            {
                throw new OrderDomainException("El número de pedido es obligatorio.");
            }
        }
    }
}
