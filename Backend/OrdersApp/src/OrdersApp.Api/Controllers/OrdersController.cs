using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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

        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderRequest request)
        {
            var command = new CreateOrderCommand(
                request.Cliente,
                request.Estado,
                request.Fecha,
                request.NumeroPedido,
                request.Total);
            var result = await _mediator.Send(command);

            if (result.IsError)
            {
                return ProblemFromError(result.FirstError);
            }

            return Ok(new OrderResponse(
                result.Value,
                request.Cliente,
                request.Estado,
                request.Fecha,
                request.NumeroPedido,
                request.Total));
        }

        [HttpDelete("{orderId:int}")]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            var command = new DeleteOrderCommand(orderId);
            var deleteOrderResult = await _mediator.Send(command);
            return deleteOrderResult.Match<IActionResult>(
                _ => NoContent(),
                errors => ProblemFromError(errors.FirstOrDefault()));
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
                request.Total);

            var modifyOrderResult = await _mediator.Send(command);

            if (modifyOrderResult.IsError)
            {
                return ProblemFromError(modifyOrderResult.FirstError);
            }

            var order = modifyOrderResult.Value;
            return Ok(new OrderResponse(
                order.Id,
                order.Cliente,
                order.Estado,
                order.Fecha,
                order.NumeroPedido,
                order.Total));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var query = new GetOrderQuery(id);

            var getOrderResult = await _mediator.Send(query);

            if (getOrderResult.IsError)
            {
                return ProblemFromError(getOrderResult.FirstError);
            }

            var order = getOrderResult.Value;
            return Ok(new OrderResponse(
                order.Id,
                order.Cliente,
                order.Estado,
                order.Fecha,
                order.NumeroPedido,
                order.Total));
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
                errors => ProblemFromError(errors.FirstOrDefault()));
        }

        private IActionResult ProblemFromError(Error error)
        {
            return error.Type switch
            {
                ErrorType.Validation => Problem(detail: error.Description, statusCode: 400),
                ErrorType.Conflict => Problem(detail: error.Description, statusCode: 409),
                ErrorType.NotFound => Problem(detail: error.Description, statusCode: 404),
                _ => Problem(detail: error.Description, statusCode: 500)
            };
        }
    }
}
