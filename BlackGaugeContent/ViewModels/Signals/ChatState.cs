using System.ComponentModel.DataAnnotations;
using Bgc.Api;

namespace Bgc.ViewModels.Signals
{
	public class ChatState
	{
		[Range(1, int.MaxValue)]
		public int    UserId    {get; set;}

		[Required]
		[StringLength(R.ModelRules.MaxNameLength, MinimumLength = R.ModelRules.MinNameLength)]
		public string OtherName {get; set;}

		[Range(1, long.MaxValue)]
		public long   MessageId {get; set;}
	}
}
