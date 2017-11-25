using System.ComponentModel.DataAnnotations;

namespace Bgc.ViewModels.User
{
	public class UserAccountDetails
	{
		[Range(1, int.MaxValue)]
		public int    UserId {get; set;}
		[Range(1, byte.MaxValue)]
		public int    GenderId {get; set;}
		public string GenderName {get; set;}
		public string Motto {get; set;}
		public int    Coins {get; set;}
		public bool   Alive {get; set;} = true;
	}
}