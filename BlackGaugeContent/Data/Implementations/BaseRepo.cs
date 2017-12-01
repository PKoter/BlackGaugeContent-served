using System;
using System.Linq;
using System.Threading.Tasks;
using Bgc.Api;
using Bgc.Data.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Bgc.Data.Implementations
{
	public class BaseRepo : IDbRepository
	{
		protected readonly BgcFullContext _context;

		public BaseRepo(BgcFullContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		public async Task<int> SaveChanges()
		{
			return await _context.SaveChangesAsync();
		}

		/// <summary>
		/// Returns id for username. Valid only for existing users.
		/// </summary>
		/// <param name="userName"></param>
		/// <exception cref="Exception">user does not exist</exception>
		/// <returns></returns>
		protected async Task<int> GetUserId(string userName)
		{
			return await _context.Users.AsNoTracking()
				.Where(u => u.UserName == userName)
				.Select(u => u.Id)
				.FirstAsync();
		}

		/// <summary>
		/// Returns if user exists.
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="throw">should throw exception if negative result?</param>
		/// <returns></returns>
		protected async Task<bool> DoesUserExist(int userId, bool @throw = true)
		{
			var result = await _context.Users.AnyAsync(u => u.Id == userId);
			if (@throw && result == false)
			{
				throw new SqlEntityException(
					"No valid user for {0} credential!", nameof(userId));
			}
			return result;
		}
	}
}
