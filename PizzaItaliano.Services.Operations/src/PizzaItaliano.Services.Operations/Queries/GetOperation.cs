using Convey.CQRS.Queries;
using PizzaItaliano.Services.Operations.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Operations.Queries
{
    public class GetOperation : IQuery<OperationDto>
    {
        public string OperationId { get; set; }
    }
}
