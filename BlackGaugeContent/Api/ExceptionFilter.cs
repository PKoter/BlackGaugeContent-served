using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Bgc.Api
{
	public class ExceptionFilter : IExceptionFilter, IDisposable
	{
		private ILogger _logger;
		private bool _isDevEnv = true;

		public ExceptionFilter(ILoggerFactory factory)
		{
			_logger = factory.CreateLogger("** EXCEPTION **");
		}


		public void OnException(ExceptionContext context)
		{
			if (_isDevEnv)
			{
				var response = new 
				{
					context.Exception.Message,
					context.Exception.StackTrace
				};
				context.Result = new ObjectResult(response)
				{
					StatusCode = 500
				};
			}
			if (!_isDevEnv)
			{
				context.ExceptionHandled = true;
			}
			_logger.LogError(context.Exception, "** EXCEPTION **");
			_logger.LogDebug("Request: {0}",context.HttpContext.Request.Path);
		}


		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}
