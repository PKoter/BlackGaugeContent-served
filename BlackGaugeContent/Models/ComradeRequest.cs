using System;
using System.ComponentModel.DataAnnotations;

namespace Bgc.Models
{
	public class ComradeRequest
	{
		[Key]
		public int      Id         {get; set;}
		public int      SenderId   {get; set;}
		public int      ReceiverId {get; set;}
		public DateTime Since      {get; set;}
		public bool     Agreed     {get; set;}
		/// <summary>
		/// Turns off notification for request.
		/// </summary>
		public bool     Seen       {get; set;}
	}
}
