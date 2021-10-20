using Convey.CQRS.Commands;
using Convey.CQRS.Events;
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

        public OrderStateModifiedHandler(IOrderServiceClient orderServiceClient, ICommandHandler<AddPayment> addPaymentCommandHandler)
        {
            _orderServiceClient = orderServiceClient;
            _addPaymentCommandHandler = addPaymentCommandHandler;
        }

        public async Task HandleAsync(OrderStateModified @event)
        {
            var orderStateBefore = @event.OrderStatusBeforeChange;
            var orderStateAfter = @event.OrderStatusAfterChange;

            if (orderStateBefore != OrderStatus.New || orderStateAfter != OrderStatus.Ready)
            {
                return;
            }

            var order = await _orderServiceClient.GetAsync(@event.OrderId);
            if (order is null)
            {
                return;
            }

            await _addPaymentCommandHandler.HandleAsync(new AddPayment { PaymentId = Guid.NewGuid(), OrderId = @event.OrderId, Cost = order.Cost });
        }
    }
}
