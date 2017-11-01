using System.ComponentModel.DataAnnotations;
using Bgc.Api;

namespace Bgc.Models.AccountViewModels
{
	public class RegisterModel
	{
		[Required]
		[StringLength(128, MinimumLength = 5)]
		public string Name {get; set;}

		[Required]
		[EmailAddress]
		[Display(Name = "Email")]
		public string Email { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm password")]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }

		[Required]
		[System.ComponentModel.DataAnnotations.Range(1, R.MaxGendersCount)]
		public int GenderId {get; set;}
	}

	/// <summary>
	/// Used to async ask during registration if name/email are unique.
	/// </summary>
	public class RegisterValueUniqueness
	{
		public string Value {get; set;}
		public string ValueType {get; set;}
		public bool Unique {get; set;}
	}
}
