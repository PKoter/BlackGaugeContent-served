using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;

namespace Bgc.Api
{
	/// <summary>
	/// Used to rewrite auth token coming from signalR request to request header so that it may be authorized by usual middleware.
	/// </summary>
	[UsedImplicitly]
	public class SignalAuthorizationMiddleware
	{
		private const string Authorization = "Authorization";
		private const string AuthQuery     = "auth";
		private readonly RequestDelegate _next;

		public SignalAuthorizationMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		/// <summary>
		/// Generates antiforgery token when request is home page, user logs in or out.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		[UsedImplicitly]
		public async Task Invoke(HttpContext context)
		{
			if (string.IsNullOrWhiteSpace(context.Request.Headers[AuthQuery]))
			{
				if (context.Request.QueryString.HasValue)
				{
					var token = context.Request.QueryString.Value
						.Split('&')
						.SingleOrDefault(x => x.Contains(AuthQuery))?.Split('=')[1];
					if (!string.IsNullOrWhiteSpace(token))
					{
						context.Request.Headers.Add(Authorization, new[] {$"Bearer {token}"});
					}
				}
			}
			await _next.Invoke(context);
		}
	}
}
