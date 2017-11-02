using System.Collections.Generic;
using System.Linq;
using Bgc.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bgc.Controllers
{
	[Route("api/[controller]")]
	public class MemeListController : Controller
	{
		private readonly BgcContext _context;

		public MemeListController(BgcContext context)
		{
			_context = context;
		}

		[HttpGet("[action]")]
		public IEnumerable<Memes> ListAll()
		{
			return _context.Memes.ToList();
		}
	}
}
