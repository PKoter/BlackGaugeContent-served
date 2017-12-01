using System.Collections.Generic;

namespace Bgc.ViewModels.User
{
	public interface IComradeModel
	{
		IEnumerable<ComradeRequest> RequestsReceived {get; set;}
		IEnumerable<ComradeRequest> RequestsAgreed   {get; set;}
	}
}
