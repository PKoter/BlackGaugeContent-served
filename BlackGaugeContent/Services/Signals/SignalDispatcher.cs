using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Bgc.Services.Signals
{
	public class SignalDispatcher : ISignalDispatcher
	{
		private readonly IHubContext<SignalHub> _context;

		public SignalDispatcher(IHubContext<SignalHub> context)
		{
			_context = context ?? throw new ArgumentNullException();
		}

		public async Task PushImpulse(string userName, string method, object impulse)
		{
			var client = _context.Clients.User(userName);
			if (client == null)
				return;
			await client.InvokeAsync(method, impulse);
		}
	}
}
