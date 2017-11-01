using System;
using System.Collections.Generic;

namespace Bgc.Models
{
	public partial class Memes
	{
		public Memes()
		{
			MemeComments = new HashSet<MemeComment>();
		}

		public int Id { get; set; }
		public string Title { get; set; }
		public string Base64 { get; set; }
		public int Rating { get; set; }
		public DateTime AddTime { get; set; }
		public int CommentCount {get; set;}

		public ICollection<MemeComment> MemeComments { get; set; }
	}
}
