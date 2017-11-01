using System.ComponentModel.DataAnnotations;

namespace Bgc.Models.AccountViewModels
{
	public class ExternalLoginViewModel
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; }
	}
}
