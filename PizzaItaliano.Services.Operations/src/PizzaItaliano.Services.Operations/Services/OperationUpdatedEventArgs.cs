using PizzaItaliano.Services.Operations.DTO;
using System;

namespace PizzaItaliano.Services.Operations.Services
{
    public class OperationUpdatedEventArgs : EventArgs
    {
        public OperationDto Operation { get; }

        public OperationUpdatedEventArgs(OperationDto operation)
        {
            Operation = operation;
        }
    }
}