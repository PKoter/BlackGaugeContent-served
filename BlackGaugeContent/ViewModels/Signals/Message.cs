using System;
using System.ComponentModel.DataAnnotations;
using Bgc.Api;
using Newtonsoft.Json;

namespace Bgc.ViewModels.Signals
{
	public class Message
	{
		public long? Id {get; set;}

		/// <summary>
		/// Output property - we know that the user sent the message when they send it.
		/// </summary>
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
		public bool Sent {get; set;}

		[Range(1, int.MaxValue)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
		public int UserId {get; set;}

		/// <summary>
		/// Input property - tells who is receiver of the message.
		/// </summary>
		[Required]
		[StringLength(R.ModelRules.MaxNameLength, MinimumLength = R.ModelRules.MinNameLength)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
		public string OtherName {get; set;}
		/// <summary>
		/// Both output and input property.
		/// </summary>
		[Required]
		[StringLength(R.ModelRules.MaxMessageLength)]
		public string Text {get; set;}

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
		public bool? Seen { get; set; }

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
		public DateTime? SentTime { get; set; }
	}
}
