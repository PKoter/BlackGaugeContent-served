using System.Collections.Generic;

namespace Bgc.Models.QueryReady
{
	public class BgcUser
	{
		public int    Id        {get; set;}
		public string Name      {get; set;}
		public byte   GenderId  {get; set;}
		public string GenderName{get; set;}
		public int    DogeCoins {get; set;}
		public int    Respek    {get; set;}
		public string Motto     {get; set;}
		public bool   Alive     {get; set;}

		public Gender Gender { get; set; }
		public ICollection<Blacklist>   Blacklisted { get; set; }
		public ICollection<Blacklist>   BlacklistsUser { get; set; }
		public ICollection<Exclusive>   Exclusive { get; set; }
		public ICollection<MemeComment> MemeComments { get; set; }
		public ICollection<MemeRating>  MemeRatings { get; set; }
		public MemeUserSession          MemeSession {get; set;}
	}
}
