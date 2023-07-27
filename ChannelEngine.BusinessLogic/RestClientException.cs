using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelEngine.BusinessLogic
{
    public class RestClientException : Exception
    {
        public RestClientException(string? message = null, Exception? innerException = null) : base(message, innerException) { }
        
    }
}
