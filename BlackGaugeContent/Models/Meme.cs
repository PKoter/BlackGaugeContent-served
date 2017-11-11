using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bgc.Models
{
	public partial class Meme
	{
		public Meme()
		{
			MemeComments = new HashSet<MemeComment>();
			MemeRatings  = new HashSet<MemeRating>();
		}

		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int      Id           { get; set; }
		public string   Title        { get; set; }
		public string   Base64       { get; set; }
		public int      Rating       { get; set; }
		public DateTime AddTime      { get; set; }
		public int      CommentCount {get; set;}

		public ICollection<MemeComment> MemeComments { get; set; }
		public ICollection<MemeRating>  MemeRatings {get; set;}
	}
}
