using System.Threading.Tasks;
using Bgc.ViewModels.Signals;

namespace Bgc.Services.Signals
{
	public interface IUserImpulse
	{
		Task<UserImpulseState> GetFullInteractionsState(int userId);
	}
}
