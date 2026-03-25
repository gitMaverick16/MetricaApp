namespace OrdersApp.Application.Common.Exceptions
{
    public sealed class DuplicateNumeroPedidoException : Exception
    {
        public DuplicateNumeroPedidoException(Exception innerException)
            : base("Ya existe un pedido con ese número.", innerException)
        {
        }
    }
}
