using OrdersApp.Domain.Common;
using OrdersApp.Domain.Orders;
using Xunit;

namespace OrdersApp.UnitTests.Domain;

public sealed class OrderTests
{
    [Fact]
    public void Create_WithValidData_ShouldTrimAndInitializeFields()
    {
        var fecha = new DateTime(2025, 1, 10, 10, 0, 0, DateTimeKind.Utc);

        var order = Order.Create(" PED-001 ", " Juan Perez ", " Registrado ", fecha, 250.75m);

        Assert.Equal("PED-001", order.NumeroPedido);
        Assert.Equal("Juan Perez", order.Cliente);
        Assert.Equal("Registrado", order.Estado);
        Assert.Equal(250.75m, order.Total);
        Assert.Equal(fecha, order.Fecha);
        Assert.False(order.IsDeleted);
        Assert.Null(order.DeletedAt);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Create_WithTotalLessOrEqualZero_ShouldThrowDomainException(decimal total)
    {
        var act = () => Order.Create("PED-001", "Juan Perez", "Registrado", DateTime.UtcNow, total);

        var ex = Assert.Throws<OrderDomainException>(act);
        Assert.Equal("El total debe ser mayor que cero.", ex.Message);
    }

    [Fact]
    public void MarkAsDeleted_ShouldSetDeletedFlags()
    {
        var order = Order.Create("PED-001", "Juan Perez", "Registrado", DateTime.UtcNow, 100m);

        order.MarkAsDeleted();

        Assert.True(order.IsDeleted);
        Assert.NotNull(order.DeletedAt);
    }
}
