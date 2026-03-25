namespace OrdersApp.Domain.Orders
{
    public class Order
    {
        public int Id { get; set; }
        public string NumeroPedido { get; set; }
        public string Cliente { get; set; }
        public string Estado { get; set; }
        public decimal Total { get; set; }
        public DateTime Fecha { get; set; }
    }
}
