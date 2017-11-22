using System;
using System.ComponentModel.DataAnnotations;

namespace Bgc.ViewModels.Bgc
{
	public class MemeModel
	{
		public MemeCore Core {get; set;}
		public MemeState State {get; set;}
	}

	/// <summary>
	/// Sent only once when loading memes.
	/// </summary>
	public class MemeCore
	{
		public int      Id      {get; set;}
		public string   Base64  {get; set;}
		public string   Title   {get; set;}
		public DateTime AddTime {get; set;}
	}

	/// <summary>
	/// Sent to update meme view state.
	/// Notice: doesn't carry meme id - client knows which meme it's subscribed for.
	/// </summary>
	public class MemeState
	{
		public int Rating       {get; set;}
		public int CommentCount {get; set;}
		public sbyte Vote       {get; set;}
	}

	/// <summary>
	/// Sent when user reacts to meme.
	/// </summary>
	public class MemeReaction
	{
		[Range(1, Int32.MaxValue)]
		public int MemeId {get; set;}
		[Range(1, Int32.MaxValue)]
		public int UserId {get; set;}
		[Range(-1, 1)]
		public sbyte Vote {get; set;}

	}

	public class ElementCount
	{
		public int Count {get; set;}
	}
}
