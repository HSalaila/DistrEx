﻿using System;

namespace DistrEx.Communication.Contracts.Events
{
    public class ErrorCallbackEventArgs : EventArgs
    {
        public ErrorCallbackEventArgs(Guid operationId, Exception error)
        {
            OperationId = operationId;
            Error = error;
        }

        public Guid OperationId { get; private set; }

        public Exception Error { get; private set; }
    }
}
