using System;
using System.Threading.Tasks;
using Bgc.Api;
using Microsoft.AspNetCore.SignalR;

namespace Bgc.Services.Signals
{
	public class SignalHub : Hub
	{
		public SignalHub()
		{
			//_logger = logger;
		}

		public async Task NotifyAll(string name, string message)
		{
			await Clients.All.InvokeAsync("notifyAll", name, message);
		}

		public async Task ComradeRequest(object arg)
		{
			await Clients.All.InvokeAsync(R.ImpulseType.ComradeRequest, arg);
		}

		public override async Task OnConnectedAsync()
		{
			await base.OnConnectedAsync();
			//_logger.LogInformation("user connected");
		}

		public override async Task OnDisconnectedAsync(Exception exception)
		{
			await base.OnDisconnectedAsync(exception);
			//_logger.LogInformation("user connected");
		}
	}
}
