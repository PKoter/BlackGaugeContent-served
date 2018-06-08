using System.Threading.Tasks;
using Bgc.Data.Contracts;
using Bgc.Models;
using Microsoft.EntityFrameworkCore;

namespace Bgc.Data.Implementations
{
	public class BgcSessionsRepo : BaseRepo, IBgcSessionsRepository
	{
		public BgcSessionsRepo(BgcFullContext context): base(context, null)
		{
		}

		public async Task<MemeUserSession> FetchMemeSession(int userId)
		{
			var session = await _context.MemeUserSessions
				.FirstOrDefaultAsync(s => s.UserId == userId);
			if (session == null)
			{
				session = new MemeUserSession()
				{
					UserId = userId
				};
				(await _context.Users.FirstAsync(u => u.Id == userId)).MemeSession = session;
				await _context.MemeUserSessions.AddAsync(session);
			}
			return session;
		}
	}
}
