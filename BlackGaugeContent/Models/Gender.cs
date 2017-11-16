using System.Collections.Generic;

namespace Bgc.Models
{
	public partial class Gender
	{
		public Gender()
		{
			Users = new HashSet<AspUser>();
		}

		public byte Id { get; set; }
		public string GenderName { get; set; }
		public string Description { get; set; }

		public ICollection<AspUser> Users { get; set; }
	}
}
