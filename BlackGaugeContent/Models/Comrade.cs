using System;
using System.ComponentModel.DataAnnotations;

namespace Bgc.Models
{
	public class Comrade
	{
		[Key]
		public int      Id               {get; set;}
		public int      FirstId          {get; set;}
		public int      SecondId         {get; set;}
		public int      InteractionCount {get; set;}
		public DateTime Since            {get; set;}
	}
}
