namespace OrdersApp.Api.Contracts
{
    public record OrderResponse(
        int Id,
        string Cliente,
        string Estado,
        DateTime Fecha,
        string NumeroPedido,
        decimal Total);
}
