using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Application.Common.Exceptions
{
    public class RedirectionException : ApiException
    {
        public string Location { get; init; }

        /// <summary>
        /// RedirectionException Constructor
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        public RedirectionException(string code, string message, string location) : base(HttpStatusCode.Redirect, code, message)
        {
            this.Location = location;
        }

        /// <summary>
        /// RedirectionException Constructor
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public RedirectionException(string code, string message, Exception innerException, string location) : base(HttpStatusCode.Redirect, code, message, innerException)
        {
            this.Location = location;
        }
    }
}
