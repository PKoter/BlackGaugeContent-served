using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bgc.Models
{
	public class MemeUserSession
	{
		[Key]
		public int Id {get; set;}
		public int FirstMemeId {get; set;}
		public int LastMemeId {get; set;}
		
		public int UserId {get; set;}
		[ForeignKey("UserId")]
		public AspUser User {get; set;}
	}
}
