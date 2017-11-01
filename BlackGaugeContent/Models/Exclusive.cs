namespace Bgc.Models
{
	public partial class Exclusive
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public int Cost { get; set; }
		public int? OwnerId { get; set; }
		public int RespekGain { get; set; }

		public AspUser Owner { get; set; }
	}
}
