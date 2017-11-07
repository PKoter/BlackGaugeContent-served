namespace Bgc.ViewModels.Account
{
	public class AccountFeedback
	{
		public FeedResult Result  {get; set;}
		public string     Message {get; set;}
		public string     Options {get; set;}
	}

	public enum FeedResult
	{
		Error,
		Success,
		Redirect
	}
}
