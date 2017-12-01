using System.ComponentModel.DataAnnotations;
using Bgc.Api;

namespace Bgc.ViewModels.Account
{
	public class RegisterModel
	{
		[Required]
		[StringLength(R.UserModelRules.MaxNameLength, MinimumLength = R.UserModelRules.MinNameLength)]
		public string Name {get; set;}

		[Required]
		[EmailAddress]
		[StringLength(R.UserModelRules.MaxEmailLength, MinimumLength = R.UserModelRules.MinEmailLength)]
		public string Email { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }

		[Required]
		[Range(1, R.MaxGendersCount)]
		public int GenderId {get; set;}
	}

	/// <summary>
	/// Used to async ask during registration if name/email are unique.
	/// </summary>
	public class RegisterValueUniqueness
	{
		public string ValueType {get; set;}
		public bool Unique {get; set;}
	}
}
