using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Releases.Application.DTO
{
    public class ReleaseDto
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid OrderProductId { get; set; }
        public DateTime Date { get; set; }
    }
}
