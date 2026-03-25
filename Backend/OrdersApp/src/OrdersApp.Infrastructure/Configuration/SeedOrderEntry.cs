namespace OrdersApp.Infrastructure.Configuration
{
    public sealed class SeedOrderEntry
    {
        public string NumeroPedido { get; init; } = string.Empty;
        public string Cliente { get; init; } = string.Empty;
        public string Estado { get; init; } = string.Empty;
        public DateTime Fecha { get; init; }
        public decimal Total { get; init; }
    }
}
