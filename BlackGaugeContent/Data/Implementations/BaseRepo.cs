using System;
using System.Threading.Tasks;
using Bgc.Data.Contracts;

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
	}
}
