using System.ComponentModel.DataAnnotations;

namespace Bgc.ViewModels.Signals
{
	public class MessageState
	{
		[Range(1, long.MaxValue)]
		public long Id {get; set;}

	}

	public class ChatImpulse
	{
		public string Comrade  {get; set;}
		public int    Impulses {get; set;}
	}
}
