using PizzaItaliano.Services.Operations.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Operations.DTO
{
    public class OperationDto
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public OperationState State { get; set; }
        public string Code { get; set; }
        public string Reason { get; set; }
    }
}
