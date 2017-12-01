using System.Collections.Generic;
using System.Threading.Tasks;
using Bgc.Models;
using JetBrains.Annotations;

namespace Bgc.Data.Contracts
{
	public interface IComradeRepository : IDbRepository
	{
		[ItemNotNull]
		Task<ComradeRequest> FetchComradeRequest(int senderId, string receiverName, bool createOnly = false);

		[ItemNotNull]
		Task<IEnumerable<ComradeRequest>> GetUserRelatedRequests(int userId);

		[ItemNotNull]
		Task<IEnumerable<ViewModels.User.ComradeRequest>> GetReceivedComradeRequests(int userId);

		[ItemNotNull]
		Task<IEnumerable<ViewModels.User.ComradeRequest>> GetAgreedComradeRequests(int userId);
	}
}
