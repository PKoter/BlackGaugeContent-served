using Microsoft.AspNetCore.SignalR;

namespace Bgc.Services.Signals
{
	public class SignalHub : Hub
	{

		public void SendToAll(string name, string message)
		{
			Clients.All.InvokeAsync("sendToAll", name, message);
		}
	}
}
