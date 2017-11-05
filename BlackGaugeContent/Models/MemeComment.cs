namespace Bgc.Models
{
	public partial class MemeComment
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public int MemeId { get; set; }
		public string Comment { get; set; }
		public int Upvotes { get; set; }
		public int Downvotes { get; set; }

		public Meme Meme { get; set; }
		public AspUser User { get; set; }
	}
}
