using System;
using System.ComponentModel.DataAnnotations;

namespace Bgc.Models
{
	public class Message
	{
		[Key]
		public long     Id         {get; set;}
		public int      SenderId   {get; set;}
		public int      ReceiverId {get; set;}
		public string   Text       {get; set;}
		public bool     Seen       {get; set;}
		public DateTime SentTime   {get; set;}
	}
}
