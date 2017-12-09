using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Bgc.Data.Contracts;
using Bgc.Models;
using Bgc.Models.QueryReady;
using Bgc.ViewModels.User;
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

		public async Task<BgcUser> GetBgcUserInfo(string userName)
		{
			return await _context.Users.AsNoTracking()
				.Join(_context.Genders.AsNoTracking(),
					u => u.GenderId,
					g => g.Id,
					(u, g) => new BgcUser()
					{
						Respek     = u.Respek,
						Motto      = u.Motto,
						Name       = u.UserName,
						GenderName = g.GenderName
					})
				.SingleOrDefaultAsync(u => u.Name.Contains(userName));
			
		}

		public async Task<UserInfo> GetUserInfo(int id, string userName)
		{
			var query = from user in _context.Users
						join gender in _context.Genders
						on user.GenderId equals gender.Id

						join req1 in _context.ComradeRequests
						on user.Id equals req1.ReceiverId into rs1
							let r1 = rs1.FirstOrDefault(r => r.SenderId == id)

						join req2 in _context.ComradeRequests
						on user.Id equals req2.SenderId into rs2
							let r2 = rs2.FirstOrDefault(r => r.ReceiverId == id)
						select new UserInfo()
						{
							Respek     = user.Respek,
							Motto      = user.Motto,
							UserName   = user.UserName,
							GenderName = gender.GenderName,
							IsComrade  = r1 != null ? r1.Agreed : r2 != null && r2.Agreed,
							RequestReceived    = r2 != null,
							ComradeRequestSent = r1 != null
						};
			return await query.SingleOrDefaultAsync(u => u.UserName.Contains(userName));
			
		}
	}
}
