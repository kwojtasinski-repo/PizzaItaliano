using Convey.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Operations.Infrastructure
{
    internal sealed class ExceptionToResponseMapper : IExceptionToResponseMapper
    {
        public ExceptionResponse Map(Exception exception)
            => exception switch
            {
                _ => new ExceptionResponse(new { code = "error", reason = "There was an error." },
                    HttpStatusCode.BadRequest)
            };
    }
}
