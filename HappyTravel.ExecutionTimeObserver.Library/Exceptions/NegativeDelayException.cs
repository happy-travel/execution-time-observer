using System;

namespace HappyTravel.ExecutionTimeObserver.Exceptions
{
    public class NegativeDelayException : Exception
    {
        public NegativeDelayException(string message) : base(message)
        {
        }
    }
}