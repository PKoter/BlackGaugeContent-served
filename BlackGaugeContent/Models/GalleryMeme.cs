using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Memstore.Models
{
    public class GalleryMeme
    {
		public int 	  ID { get; set; }
		[Required]
		[StringLength(120, MinimumLength = 8)]
	    public string Url { get; set; }
	    public string Title {get; set;}
	    public int 	  Rating {get; set;}
		public DateTime AddTime {get; set;}
    }
}
