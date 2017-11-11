using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bgc.Models
{
	public class MemeRating
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int  Id     {get; set;}
		public int  UserId {get; set;}
		public int  MemeId {get; set;}
		public byte Rating {get; set;}
		[NotMapped]
		public sbyte Vote {
			get {return (sbyte)Rating;} 
			set {Rating = (byte)value;}
		}

		public Meme    Meme {get; set;}
		public AspUser User {get; set;}

		public override bool Equals(object obj)
		{
			var rating = obj as MemeRating;
			return rating != null &&
				   Id == rating.Id &&
				   UserId == rating.UserId &&
				   MemeId == rating.MemeId;
		}

		public override int GetHashCode()
		{
			var hashCode = 1584987736;
			hashCode = hashCode * -1521134295 + UserId.GetHashCode();
			hashCode = hashCode * -1521134295 + MemeId.GetHashCode();
			hashCode = hashCode * -1521134295 + Vote.GetHashCode();
			return hashCode;
		}
	}
}
