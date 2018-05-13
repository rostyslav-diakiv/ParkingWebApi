using System;
using System.Runtime.Serialization;

using Newtonsoft.Json.Linq;

namespace Parking.BLL.Dtos
{
    [Serializable]
    public class HttpStatusCodeException : Exception
    {
        public int StatusCode { get; set; }
        public string ContentType { get; set; } = @"application/json";//@text/plain

        public HttpStatusCodeException(int statusCode)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCodeException(int statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCodeException(int statusCode, Exception inner) : this(statusCode, inner.ToString()) { }

        public HttpStatusCodeException(int statusCode, JObject errorObject) : this(statusCode, errorObject.ToString())
        {
        }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected HttpStatusCodeException(SerializationInfo info, StreamingContext context)
        {
        }
    }
}
