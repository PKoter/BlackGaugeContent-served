using System.Linq;
using System.Threading.Tasks;
using Bgc.Controllers;
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
			await _comrades.FillRequestsInModel(state, userId);
			state.NotifyCount = state.RequestsAgreed.Count() + state.RequestsReceived.Count();
			return state;
		}
	}
}
