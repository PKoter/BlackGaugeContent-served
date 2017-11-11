using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Bgc.Services
{
	public class HttpStrictTransportSecurityMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly HttpStrictTransportSecurityOptions _options;
		private readonly ILogger<HttpStrictTransportSecurityMiddleware> _logger;

		public HttpStrictTransportSecurityMiddleware(
			RequestDelegate next,
			HttpStrictTransportSecurityOptions options,
			ILogger<HttpStrictTransportSecurityMiddleware> logger)
		{
			_logger  = logger  ?? throw new ArgumentNullException(nameof(logger));
			_next    = next    ?? throw new ArgumentNullException(nameof(next));
			_options = options ?? throw new ArgumentNullException(nameof(options));
		}

		// The Invoke method is the entry point of the middleware
		// _next() calls the next middleware
		public Task Invoke(HttpContext context)
		{
			if (!context.Request.IsHttps)
			{
				_logger.LogDebug("HSTS response header is not set because the scheme is not https.");
				return _next(context); // Continue to the next middleware
			}

			if (!_options.EnableForLocalhost && IsLocalhost(context))
			{
				_logger.LogDebug("HSTS response header is disabled for localhost.");
				return _next(context); // Continue to the next middleware
			}

			var headerValue = GetHeaderValue();
			_logger.LogDebug("Adding HSTS response header: {HeaderValue}.", headerValue);
			context.Response.Headers.Add("Strict-Transport-Security", headerValue);

			return _next(context); // Continue to the next middleware
		}

		private string GetHeaderValue()
		{
			// max-age=31536000; includeSubDomains; preload
			var headerValue = "max-age: " + (int)_options.MaxAge.TotalSeconds;
			if (_options.IncludeSubDomains)
			{
				headerValue += "; includeSubDomains";
			}

			if (_options.Preload)
			{
				headerValue += "; preload";
			}

			return headerValue;
		}

		private bool IsLocalhost(HttpContext context)
		{
			return string.Equals(context.Request.Host.Host, "localhost", StringComparison.OrdinalIgnoreCase);
		}
	}

	public class HttpStrictTransportSecurityOptions
	{
		public TimeSpan MaxAge { get; set; } = TimeSpan.FromDays(30);
		public bool IncludeSubDomains { get; set; } = true;
		public bool Preload { get; set; } = false;
		public bool EnableForLocalhost { get; set; } = true;
	}

}