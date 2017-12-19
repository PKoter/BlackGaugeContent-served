using System.Threading.Tasks;
using Bgc.Data.Contracts;
using Bgc.ViewModels.Signals;

namespace Bgc.Services.Signals
{
	public class UserImpulse: IUserImpulse
	{
		private readonly IComradeRepository _comrades;

		public UserImpulse(IComradeRepository comrades)
		{
			_comrades = comrades;
		}

		public async Task<UserImpulseState> GetFullInteractionsState(int userId)
		{
			var state = new UserImpulseState();
			var requests = await _comrades.CountActiveRequests(userId);

			state.NotifyCount         = requests;
			state.ComradeRequestCount = requests;
			return state;
		}
	}
}
