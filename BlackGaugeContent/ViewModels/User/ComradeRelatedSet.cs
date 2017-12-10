using System.Collections.Generic;

namespace Bgc.ViewModels.User
{
	/// <summary>
	/// Transfers request and comrade data to view.
	/// </summary>
	public class ComradeRelatedSet
	{
		public IEnumerable<ComradeSlim>    Comrades {get; set;}
		public IEnumerable<ComradeRequest> Received {get; set;}
		public IEnumerable<ComradeRequest> Sent     {get; set;}
	}
}
