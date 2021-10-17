using Convey.WebApi.Exceptions;
using PizzaItaliano.Services.Payments.Application.Exceptions;
using PizzaItaliano.Services.Payments.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Payments.Infrastructure.Exceptions
{
    internal sealed class ExceptionToResponseMapper : IExceptionToResponseMapper
    {
        public ExceptionResponse Map(Exception exception)
            => exception switch
            {
                DomainException ex => new ExceptionResponse(new { code = ex.Code, reason = ex.Message }, System.Net.HttpStatusCode.BadRequest),
                AppException ex => new ExceptionResponse(new { code = ex.Code, reason = ex.Message }, System.Net.HttpStatusCode.BadRequest),
                _ => new ExceptionResponse(new { code = "error", reason = "There was an error" }, System.Net.HttpStatusCode.InternalServerError)
            };
    }
}
