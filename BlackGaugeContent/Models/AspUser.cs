using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Bgc.Models
{
	// Add profile data for application users by adding properties to the ApplicationUser class
	public class AspUser : IdentityUser<int>
	{
		public string Motto     { get; set; }
		public byte   GenderId  { get; set; }
		public int    Respek    { get; set; }
		public int    DogeCoins {get; set;}
		
		public int MemeSessionId {get; set;}

		public Gender Gender { get; set; }
		public ICollection<Blacklist>    Blacklisted { get; set; }
		public ICollection<Blacklist>    BlacklistsUser { get; set; }
		public ICollection<Exclusive>    Exclusive { get; set; }
		public ICollection<MemeComment>  MemeComments { get; set; }
		public ICollection<MemeRating>   MemeRatings { get; set; }
		[ForeignKey("MemeSessionId")]
		public MemeUserSession           MemeSession {get; set;}
	}
}
