using System;

namespace HappyTravel.ExecutionTimeObserver.Library.Exceptions
{
    public class NegativeDelayException : Exception
    {
        public NegativeDelayException(string message) : base(message)
        {
        }
    }
}