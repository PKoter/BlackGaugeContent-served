using System.ComponentModel.DataAnnotations;

namespace Bgc.ViewModels.Signals
{
	public class MessageState
	{
		[Range(1, long.MaxValue)]
		public long Id {get; set;}

	}
}
