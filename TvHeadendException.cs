using System;
using System.Runtime.Serialization;

namespace TvHeadendRestApiClientLibrary
{
    public class TvHeadendException : Exception
    {
        public TvHeadendException() { }
        public TvHeadendException(string message) : base(message) { }
        public TvHeadendException(string message, Exception inner)
                : base(message, inner) { }
        protected TvHeadendException(SerializationInfo info,
                StreamingContext context) : base(info, context) { }
    }
    
}
