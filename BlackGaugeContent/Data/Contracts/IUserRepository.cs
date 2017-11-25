using System.Collections.Generic;
using System.Threading.Tasks;
using Bgc.Models;
using JetBrains.Annotations;

namespace Bgc.Data.Contracts
{
	public interface IUserRepository : IDbRepository
	{
		[NotNull]
		[ItemNotNull]
		Task<IEnumerable<Gender>> GetGenders();

		[NotNull]
		Task<AspUser> GetUser(int userId, bool detectChanges = false);
	}
}
