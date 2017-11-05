using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Bgc.Models;
using Bgc.Models.AccountViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bgc.Controllers
{
	[Route("api/[controller]")]
	public class UserController : Controller
	{
		private readonly BgcContext _context;

		public UserController(BgcContext context)
		{
			_context = context;
		}


		[HttpGet("[action]")]
		[AllowAnonymous]
		public async Task<IEnumerable<Gender>> GetGenders()
		{
			return await _context.Genders.ToListAsync();
		}

		[HttpGet("[action]")]
		[Produces("application/json")]
		[AllowAnonymous]
		public async Task<RegisterValueUniqueness> CheckUniqueness(string value, string type)
		{
			var result = new RegisterValueUniqueness() { ValueType = type };
			IQueryable<string> query;
			if ("name".Equals(type))
			{
				query = _context.Users.Select(u => u.UserName);
			}
			else if ("email".Equals(type))
			{
				query = _context.Users.Select(u => u.Email);
			}
			else
				throw new SecurityException();

			result.Unique = await query.FirstOrDefaultAsync(u => u == value) == null;
			return result;
		}
	}
}
