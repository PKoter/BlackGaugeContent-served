using System;
using System.ComponentModel.DataAnnotations;
using Bgc.Api;
using Newtonsoft.Json;

namespace Bgc.ViewModels.User
{
	public class ComradeRequest
	{
		[Range(1, int.MaxValue)]
		[JsonIgnore]
		public int SenderId {get; set;}

		[Required]
		[StringLength(R.UserModelRules.MaxNameLength, MinimumLength = R.UserModelRules.MinNameLength)]
		public string OtherName {get; set;}

		public DateTime Since  {get; set;}
		public bool     Agreed {get; set;}
	}
}
