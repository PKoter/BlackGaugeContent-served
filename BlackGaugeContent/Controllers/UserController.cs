using System.Collections.Generic;
using System.Linq;
using Bgc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
		public IEnumerable<Gender> GetGenders()
		{
			return _context.Genders.ToList();
		}

	}
}
