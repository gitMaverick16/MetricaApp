namespace OrdersApp.Domain.Common
{
    public sealed class OrderDomainException : Exception
    {
        public OrderDomainException(string message) : base(message)
        {
        }
    }
}
