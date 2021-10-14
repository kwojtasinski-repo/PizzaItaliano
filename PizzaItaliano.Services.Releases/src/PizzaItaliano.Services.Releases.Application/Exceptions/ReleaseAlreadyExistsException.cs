using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Releases.Application.Exceptions
{
    public class ReleaseAlreadyExistsException : AppException
    {
        public override string Code => "release_already_exists";
        public Guid Id { get; }

        public ReleaseAlreadyExistsException(Guid id) : base($"Release with id: {id} already exists")
        {
            Id = id;
        }
    }
}
