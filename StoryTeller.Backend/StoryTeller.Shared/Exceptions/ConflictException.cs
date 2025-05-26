using System;
using System.Net;

namespace StoryTeller.Shared.Exceptions
{
    public class ConflictException : Exception
    {
        public HttpStatusCode StatusCode => HttpStatusCode.Conflict;

        public ConflictException(string message) : base(message)
        {
        }

        public ConflictException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
