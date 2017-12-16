using System.Threading.Tasks;

namespace Bgc.Services.Signals
{
	public interface ISignalDispatcher
	{
		Task PushImpulse(string userName, string method, object impulse);

	}
}
