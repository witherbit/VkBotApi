using System;
using System.Collections.Generic;
using System.Text;

namespace VkBotApi.Models
{
    public class RequestException : Exception
    {
        public RequestException() : base("Cannot send request.")
        {
        }
    }
}
