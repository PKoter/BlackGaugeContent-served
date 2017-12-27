using System.Linq;
using System.Threading.Tasks;
using Bgc.Data.Contracts;
using Bgc.ViewModels.Signals;

namespace Bgc.Services.Signals
{
	public class UserImpulse: IUserImpulse
	{
		private readonly IComradeRepository _comrades;
		private readonly IMessageRepository _messages;

		public UserImpulse(IComradeRepository comrades, IMessageRepository messages)
		{
			_comrades = comrades;
			_messages = messages;
		}

		public async Task<UserImpulseState> GetFullInteractionsState(int userId)
		{
			var state = new UserImpulseState();
			var requests = await _comrades.CountActiveRequests(userId);

			var chatImpulses = await _messages.GetChatImpulses(userId);
			int messageCount = chatImpulses.Sum(m => m.Impulses);

			state.NotifyCount         = requests + messageCount;
			state.ComradeRequestCount = requests;
			state.MessageCount        = messageCount;
			state.ChatImpulses        = chatImpulses;
			return state;
		}
	}
}
