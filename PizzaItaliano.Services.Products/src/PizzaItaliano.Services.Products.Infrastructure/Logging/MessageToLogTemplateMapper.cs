using Convey.Logging.CQRS;
using PizzaItaliano.Services.Products.Application.Commands;
using PizzaItaliano.Services.Products.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Products.Infrastructure.Logging
{
    internal sealed class MessageToLogTemplateMapper : IMessageToLogTemplateMapper
    {
        public HandlerLogTemplate Map<TMessage>(TMessage message) where TMessage : class
        {
            var key = message.GetType();

            return Templates.TryGetValue(key, out var template) ? template : null;
        }

        private static readonly IReadOnlyDictionary<Type, HandlerLogTemplate> Templates = new Dictionary<Type, HandlerLogTemplate>()
        {
            [typeof(AddProduct)] = new HandlerLogTemplate
            {
                Before = "Adding an product with id: {ProductId}",
                After = "Added an product with id: {ProductId}",
                OnError = new Dictionary<Type, string> // w zaleznosci w jakim kontekscie poleci wyjatek
                {
                    [typeof(ProductAlreadyExistsException)] = "Product with id: {ProductId} already exists"
                }
            },
            [typeof(DeleteProduct)] = new HandlerLogTemplate
            {
                Before = "Deleting an product with id: {ProductId}",
                After = "Deleted an product with id: {ProductId}",
                OnError = new Dictionary<Type, string> // w zaleznosci w jakim kontekscie poleci wyjatek
                {
                    [typeof(ProductNotFoundException)] = "Product with id: {ProductId} not found",
                    [typeof(CannotDeleteProductException)] = "Cannot delete product with id: {ProductId}"
                }
            },
            [typeof(UpdateProduct)] = new HandlerLogTemplate
            {
                Before = "Updating an product with id: {ProductId}",
                After = "Updated an product with id: {ProductId}",
                OnError = new Dictionary<Type, string> // w zaleznosci w jakim kontekscie poleci wyjatek
                {
                    [typeof(InvalidUpdateProductException)] = "Product with id: {ProductId} has invalid parameters",
                    [typeof(InvalidProductCostException)] = "Product with id: {ProductId} has invalid cost",
                    [typeof(ProductNotFoundException)] = "Product with id: {ProductId} not found"
                }
            }
        };
    }
}
