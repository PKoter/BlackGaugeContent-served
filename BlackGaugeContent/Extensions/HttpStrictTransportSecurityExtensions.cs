using System;
using Bgc.Services;
using Microsoft.AspNetCore.Builder;

namespace Bgc.Extensions
{
	public static class HttpStrictTransportSecurityExtensions
	{
		public static IApplicationBuilder UseHttpStrictTransportSecurity(
			this IApplicationBuilder app,
			HttpStrictTransportSecurityOptions options)
		{
			if (app == null)     throw new ArgumentNullException(nameof(app));
			if (options == null) throw new ArgumentNullException(nameof(options));

			return app.UseMiddleware<HttpStrictTransportSecurityMiddleware>(options);
		}

		public static IApplicationBuilder UseHttpStrictTransportSecurity(this IApplicationBuilder app)
		{
			return UseHttpStrictTransportSecurity(app, new HttpStrictTransportSecurityOptions());
		}

		public static IApplicationBuilder UseHttpStrictTransportSecurity(
			this IApplicationBuilder app,
			Action<HttpStrictTransportSecurityOptions> configure)
		{
			if (app == null) throw new ArgumentNullException(nameof(app));

			var options = new HttpStrictTransportSecurityOptions();
			configure?.Invoke(options);

			return UseHttpStrictTransportSecurity(app, options);
		}
	}
}