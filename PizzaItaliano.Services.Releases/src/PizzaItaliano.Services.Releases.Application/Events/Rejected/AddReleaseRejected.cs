using Convey.CQRS.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Releases.Application.Events.Rejected
{
    [Contract]
    public class AddReleaseRejected : IRejectedEvent
    {
        public Guid ReleaseId { get; }
        public string Reason { get; }
        public string Code { get; }

        public AddReleaseRejected(Guid releaseId, string reason, string code)
        {
            ReleaseId = releaseId;
            Reason = reason;
            Code = code;
        }
    }
}
