using System.ComponentModel.DataAnnotations;

namespace Bgc.ViewModels.User
{
	public class UserPublicDetails
	{
		[Required]
		public string UserName   {get; set;}
		[Required]
		public string GenderName {get; set;}
		public string Motto      {get; set;}
		public int    Respek     {get; set;}
	}
}
