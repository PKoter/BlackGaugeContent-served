using System.Threading.Tasks;
using Bgc.Models;
using JetBrains.Annotations;

namespace Bgc.Data.Contracts
{
	public interface IBgcSessionsRepository : IDbRepository
	{
		/// <summary>
		/// Gets user browse session or creates new if non-existent.
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		[NotNull]
		[ItemNotNull]
		Task<MemeUserSession> FetchMemeSession(int userId);
	}
}
