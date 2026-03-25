using Microsoft.Extensions.Logging.Abstractions;
using OrdersApp.Application.Common.Interfaces;
using OrdersApp.Application.Orders.Commands.CreateOrder;
using OrdersApp.Domain.Orders;
using Xunit;

namespace OrdersApp.UnitTests.Application;

public sealed class CreateOrderCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenNumeroPedidoAlreadyExists_ShouldReturnConflict()
    {
        var repo = new FakeOrdersRepository
        {
            ExistsByNumeroPedidoResult = true
        };
        var handler = new CreateOrderCommandHandler(repo, NullLogger<CreateOrderCommandHandler>.Instance);
        var command = new CreateOrderCommand("Cliente", "Registrado", DateTime.UtcNow, "PED-001", 120m);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal("Ya existe un pedido con ese número.", result.FirstError.Description);
        Assert.False(repo.AddOrderCalled);
    }

    [Fact]
    public async Task Handle_WhenTotalIsInvalid_ShouldReturnValidationError()
    {
        var repo = new FakeOrdersRepository();
        var handler = new CreateOrderCommandHandler(repo, NullLogger<CreateOrderCommandHandler>.Instance);
        var command = new CreateOrderCommand("Cliente", "Registrado", DateTime.UtcNow, "PED-001", 0m);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal("El total debe ser mayor que cero.", result.FirstError.Description);
        Assert.False(repo.AddOrderCalled);
    }

    private sealed class FakeOrdersRepository : IOrdersRepository
    {
        public bool ExistsByNumeroPedidoResult { get; set; }
        public bool AddOrderCalled { get; private set; }

        public Task AddOrderAsync(Order order, CancellationToken cancellationToken = default)
        {
            AddOrderCalled = true;
            return Task.CompletedTask;
        }

        public Task<bool> ExistsByNumeroPedidoAsync(string numeroPedido, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(ExistsByNumeroPedidoResult);
        }

        public Task<bool> ExistsByNumeroPedidoExcludingIdAsync(
            string numeroPedido,
            int excludeOrderId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }

        public Task<List<Order>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new List<Order>());
        }

        public Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<Order?>(null);
        }

        public Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
