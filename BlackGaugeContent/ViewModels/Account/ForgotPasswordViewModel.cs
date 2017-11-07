using System.ComponentModel.DataAnnotations;

namespace Bgc.ViewModels.Account
{
	public class ForgotPasswordViewModel
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; }
	}
}
