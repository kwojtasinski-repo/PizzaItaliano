using Convey.MessageBrokers.RabbitMQ;
using PizzaItaliano.Services.Products.Application.Commands;
using PizzaItaliano.Services.Products.Application.Events.Rejected;
using PizzaItaliano.Services.Products.Application.Exceptions;
using PizzaItaliano.Services.Products.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Products.Infrastructure.Exceptions
{
    internal sealed class ExceptionToMessageMapper : IExceptionToMessageMapper
    {
        public object Map(Exception exception, object message) // mapowanie wyjatkow na message (wiadomosci sa wysylane do RabbitMQ)
            => exception switch
            { 
                // App Exceptions
                Application.Exceptions.InvalidProductCostException ex => message switch
                { 
                    UpdateProduct command => new UpdateProductRejected(command.ProductId, ex.Message, ex.Code),
                    _ => null // rzucony tylko w kontekscie update jesli w innych przypadkach nic nie zwracaj (jesli pojawi sie wymog a mozliwe ze tak to wystarczy dopisac inne commandy)
                },
                CannotDeleteProductException ex => new DeleteProductRejected(ex.ProductId, ex.Message, ex.Code), // wyjatek poleci tylko na delete
                ProductAlreadyExistsException ex => new AddProductRejected(ex.ProductId, ex.Message, ex.Code), // wyjatek rzucony tylko podczas dodawania produktu
                ProductNotFoundException ex => message switch
                {
                    UpdateProduct command => new UpdateProductRejected(command.ProductId, ex.Message, ex.Code),
                    DeleteProduct command => new DeleteProductRejected(command.ProductId, ex.Message, ex.Code),
                    _ => null
                },

                // Domain Exceptions
                Core.Exceptions.InvalidProductCostException ex => new AddProductRejected(Guid.Empty, ex.Message, ex.Code),
                ProductNameCannotBeEmptyException ex => new AddProductRejected(Guid.Empty, ex.Message, ex.Code),

                // other unforeseen exceptions 
                _ => null
            };
    }
}
