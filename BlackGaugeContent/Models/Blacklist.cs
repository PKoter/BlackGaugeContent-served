using System;

namespace Bgc.Models
{
	public partial class Blacklist
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public int BlacklistedId { get; set; }
		public DateTime Since { get; set; }

		public AspUser Blacklisted { get; set; }
		public AspUser User { get; set; }
	}
}
