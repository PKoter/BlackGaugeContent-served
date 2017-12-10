using System;
using System.ComponentModel.DataAnnotations;
using Bgc.Api;
using Newtonsoft.Json;

namespace Bgc.ViewModels.User
{
	public class ComradeRequest
	{
		[Range(1, int.MaxValue)]
		public int? Id {get; set;}

		[Required]
		[StringLength(R.UserModelRules.MaxNameLength, MinimumLength = R.UserModelRules.MinNameLength)]
		public string OtherName {get; set;}

		/// <summary>
		/// Should be not null when from client side.
		/// </summary>
		[Required]
		[Range(1, int.MaxValue)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
		public int?  SenderId {get; set;}
		/// <summary>
		/// Used at server side, but won't be sent.
		/// </summary>
		[JsonIgnore]
		public bool  Received {get; set;}
		public bool?   Agreed {get; set;}
		[JsonIgnore]
		public DateTime Since {get; set;}

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
		public string TimeSpan {get; set;}
	}

	/// <summary>
	/// Message from client side to accept request.
	/// </summary>
	public class ComradeRequestFeedback
	{
		public int Id         {get; set;}
		public int ReceiverId {get; set;}
	}
}
