using System.Threading.Tasks;
using Bgc.Models;
using JetBrains.Annotations;

namespace Bgc.Data.Contracts
{
	public interface IUserRepository : IDbRepository
	{
		[NotNull]
		Task<AspUser> GetUser(int userId, bool detectChanges = false);
	}
}
