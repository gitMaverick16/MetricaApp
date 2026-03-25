using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrdersApp.Api.Contracts;
using OrdersApp.Application.Orders.Commands.CreateOrder;
using OrdersApp.Application.Orders.Commands.DeleteOrder;
using OrdersApp.Application.Orders.Commands.ModifyOrder;
using OrdersApp.Application.Orders.Queries.GetAllOrders;
using OrdersApp.Application.Orders.Queries.GetOrder;

namespace OrdersApp.Api.Controllers
{
    [Route("api/pedidos")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost()]
        public async Task<IActionResult> CreateOrder(CreateOrderRequest request)
        {
            var command = new CreateOrderCommand(
                request.Cliente,
                request.Estado,
                request.Fecha,
                request.NumeroPedido,
                request.Total);
            var createOrderResult = await _mediator.Send(command);

            return createOrderResult.MatchFirst(
                id => Ok(new OrderResponse(
                    id, 
                    request.Cliente,
                    request.Estado,
                    request.Fecha,
                    request.NumeroPedido,
                    request.Total)),
                error => Problem()
                );
        }

        [HttpDelete("{orderId:int}")]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            var command = new DeleteOrderCommand(orderId);
            var deleteOrderResult = await _mediator.Send(command);
            return deleteOrderResult.Match<IActionResult>(
                _ => NoContent(),
                errors => Problem(errors.FirstOrDefault().Description)
                );
        }

        [HttpPut("{orderId:int}")]
        public async Task<IActionResult> ModifyPermission(int orderId, ModifyOrderRequest request)
        {
            var command = new ModifyOrderCommand(
                request.Cliente,
                request.Estado,
                request.Fecha,
                request.NumeroPedido,
                orderId,
                request.Total
            );

            var modifyOrderResult = await _mediator.Send(command);

            return modifyOrderResult.MatchFirst(
                order => Ok(new OrderResponse(
                    order.Id,
                    request.Cliente,
                    request.Estado,
                    request.Fecha,
                    request.NumeroPedido,
                    request.Total)),
                errors => Problem(errors.Description));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var query = new GetOrderQuery(id);

            var getOrderResult = await _mediator.Send(query);

            return getOrderResult.MatchFirst(
                permission => Ok(new OrderResponse(
                    getOrderResult.Value.Id,
                    getOrderResult.Value.Cliente,
                    getOrderResult.Value.Estado,
                    getOrderResult.Value.Fecha,
                    getOrderResult.Value.NumeroPedido,
                    getOrderResult.Value.Total)),
                error => Problem());
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var query = new GetAllOrdersQuery();

            var getAllOrdersResult = await _mediator.Send(query);

            return getAllOrdersResult.Match(
                orders => Ok(
                    orders.ConvertAll(
                        order => new OrderResponse(
                            order.Id,
                            order.Cliente,
                            order.Estado,
                            order.Fecha,
                            order.NumeroPedido,
                            order.Total))),
                _ => Problem());
        }
    }
}
