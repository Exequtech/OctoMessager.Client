using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoMessager.client.Exceptions
{
    public class OctoMessengerException : Exception
    {
        public System.Net.HttpStatusCode? StatusCode { get; }

        public OctoMessengerException(string message) : base(message) { }

        public OctoMessengerException(string message, Exception innerException)
            : base(message, innerException) { }

        public OctoMessengerException(string message, System.Net.HttpStatusCode statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
