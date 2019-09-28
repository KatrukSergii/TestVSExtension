using System;

namespace SampleInjector.Exceptions
{
    public class SampleInitException : Exception
    {
        public SampleInitException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
