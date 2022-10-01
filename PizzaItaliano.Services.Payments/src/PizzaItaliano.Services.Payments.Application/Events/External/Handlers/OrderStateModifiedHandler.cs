using Convey.CQRS.Commands;
using Convey.CQRS.Events;
using Microsoft.Extensions.Logging;
using PizzaItaliano.Services.Payments.Application.Commands;
using PizzaItaliano.Services.Payments.Application.Services.Clients;
using PizzaItaliano.Services.Payments.Core.Entities;
using PizzaItaliano.Services.Payments.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Payments.Application.Events.External.Handlers
{
    public class OrderStateModifiedHandler : IEventHandler<OrderStateModified>
    {
        private readonly IOrderServiceClient _orderServiceClient;
        private readonly ICommandHandler<AddPayment> _addPaymentCommandHandler;
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILogger<OrderStateModifiedHandler> _logger;

        public OrderStateModifiedHandler(IOrderServiceClient orderServiceClient, ICommandHandler<AddPayment> addPaymentCommandHandler,
            IPaymentRepository paymentRepository, ILogger<OrderStateModifiedHandler> logger)
        {
            _orderServiceClient = orderServiceClient;
            _addPaymentCommandHandler = addPaymentCommandHandler;
            _paymentRepository = paymentRepository;
            _logger = logger;
        }

        public async Task HandleAsync(OrderStateModified @event)
        {
            var orderStateBefore = @event.OrderStatusBeforeChange;
            var orderStateAfter = @event.OrderStatusAfterChange;

            if (orderStateBefore == OrderStatus.New && orderStateAfter != OrderStatus.Ready)
            {
                await CreatePayment(@event);
                return;
            }

            if (orderStateBefore == OrderStatus.Ready && orderStateAfter != OrderStatus.New)
            {
                await DeletePayment(@event);
                return;
            }
        }

        private async Task CreatePayment(OrderStateModified @event)
        {
            var order = await _orderServiceClient.GetAsync(@event.OrderId);
            if (order is null)
            {
                return;
            }

            var existPayment = await _paymentRepository.GetByOrderIdAsync(@event.OrderId);

            if (existPayment != null)
            {
                await _paymentRepository.DeleteAsync(existPayment.Id);
            }

            await _addPaymentCommandHandler.HandleAsync(new AddPayment { PaymentId = Guid.NewGuid(), OrderId = @event.OrderId, Cost = order.Cost });
        }

        private async Task DeletePayment(OrderStateModified @event)
        {
            var existPayment = await _paymentRepository.GetByOrderIdAsync(@event.OrderId);

            if (existPayment == null)
            {
                _logger.LogError($"Payment for order with id: '{@event.OrderId}' was not found");
                return;
            }

            await _paymentRepository.DeleteAsync(existPayment.Id);
        }
    }
}
