using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Bgc.Data.Contracts;
using Bgc.Models;
using Bgc.Models.QueryReady;
using Microsoft.EntityFrameworkCore;

namespace Bgc.Data.Implementations
{
	public class UserRepo : BaseRepo, IUserRepository
	{
		public UserRepo(BgcFullContext context) : base(context) {}

		public async Task<IEnumerable<Gender>> GetGenders()
		{
			return await _context.Genders
				.AsNoTracking()
				.OrderBy(g => g.Id)
				.ToListAsync();
		}

		public async Task<AspUser> GetUser<T>(int userId, bool detectChanges = false, Expression<Func<AspUser, T>> include = null)
		{
			var userQuery = _context.Users.AsNoTracking();
			if (include != null)
				userQuery = userQuery.Include(include);
			var user = await userQuery.FirstAsync(u => u.Id == userId);
			if (detectChanges)
				_context.Attach(user);
			return user;
		}

		public async Task<AspUser> GetUser(int userId, bool detectChanges = false)
		{
			return await GetUser<AspUser>(userId, detectChanges);
		}

		public async Task<BgcUser> GetBgcUser(int userId)
		{
			var userQuery = _context.Users.AsNoTracking();
			var user = await userQuery.Select(u => new BgcUser()
				{
					Id        = u.Id,
					GenderId  = u.GenderId,
					Respek    = u.Respek,
					DogeCoins = u.DogeCoins,
					Motto     = u.Motto,
					Name      = u.UserName
				}).FirstAsync(u => u.Id == userId);
			return user;
		}
	}
}
