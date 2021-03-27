using System;
using System.Collections.Generic;
using System.Text;
using VkBotApi.Enums;

namespace VkBotApi.Models
{
	public class ApiException : Exception
	{
		public ExceptionCode Code;
		public ApiException(string message, ExceptionCode code) : base(message)
		{
			Code = code;
		}
	}
}
