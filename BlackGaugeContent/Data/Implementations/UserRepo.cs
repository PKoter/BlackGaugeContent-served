using System.Threading.Tasks;
using Bgc.Data.Contracts;
using Bgc.Models;
using Microsoft.EntityFrameworkCore;

namespace Bgc.Data.Implementations
{
	public class UserRepo : BaseRepo, IUserRepository
	{
		public UserRepo(BgcFullContext context) : base(context) {}


		public async Task<AspUser> GetUser(int userId, bool detectChanges = false)
		{
			var user = await _context.Users.FirstAsync(u => u.Id == userId);
			if (detectChanges)
				_context.Attach(user);
			return user;
		}
	}
}
