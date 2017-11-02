using Microsoft.AspNetCore.Mvc;

namespace Bgc.Controllers
{
	[Route("api/[controller]")]
	public class AppMetaController
	{
		/// <summary>
		/// Should be replaced with file or db data, but... Maybe when i have real tos
		/// </summary>
		private const string TermsOfService =
				"Kurczaki, ziemniaki\r\n\r\n    Donald Tusk o ziemniakach \r\n\r\nZiemniak też człowiek i może sobie pomarzyć!\r\n\r\n    Lays o ziemniakach \r\n\r\nA może frytki do tego?\r\n\r\n    McDonald\'s o wyższej formie ziemniaka \r\n\r\nOgraniczenia i limity nie interesują mnie...\r\n\r\n    Znana przyśpiewka ziemniaka z reklamy Nju Mobile ";

		[HttpGet("[action]")]
		public dynamic GetTermsOfService()
		{
			return new { Terms= TermsOfService };
		}

	}
}
