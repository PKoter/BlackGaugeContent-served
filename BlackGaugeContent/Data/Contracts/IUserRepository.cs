using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Bgc.Models;
using Bgc.Models.QueryReady;
using JetBrains.Annotations;

namespace Bgc.Data.Contracts
{
	public interface IUserRepository : IDbRepository
	{
		[NotNull]
		[ItemNotNull]
		Task<IEnumerable<Gender>> GetGenders();

		[NotNull]
		Task<AspUser> GetUser(int userId,
			bool detectChanges = false);

		[NotNull]
		Task<AspUser> GetUser<T>(int userId,
			bool detectChanges = false,
			Expression<Func<AspUser, T>> include = null);

		Task<BgcUser> GetBgcUser(int userId);

		[ItemCanBeNull]
		Task<BgcUser> GetBgcUserInfo(string userName);
	}
}
