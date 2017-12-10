using System.Collections.Generic;

namespace Bgc.ViewModels.User
{
	public interface IComradeModel
	{
		IEnumerable<ComradeRequest> Received {get; set;}
		IEnumerable<ComradeRequest> Agreed   {get; set;}
	}
}
