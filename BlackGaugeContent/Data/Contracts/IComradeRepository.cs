using System.Collections.Generic;
using System.Threading.Tasks;
using Bgc.ViewModels.User;
using JetBrains.Annotations;
using ComradeRequest = Bgc.Models.ComradeRequest;

namespace Bgc.Data.Contracts
{
	public interface IComradeRepository : IDbRepository
	{
		[ItemNotNull]
		Task<ComradeRequest> FetchComradeRequest(int senderId, string receiverName, bool createOnly = false);

		[ItemCanBeNull]
		Task<ComradeRequest> DrawComradeRequest(int requestId);

		[ItemNotNull]
		Task<IEnumerable<ViewModels.User.ComradeRequest>> GetReceivedComradeRequests(int userId);

		[ItemNotNull]
		Task<IEnumerable<ViewModels.User.ComradeRequest>> GetAgreedComradeRequests(int userId);

		[ItemNotNull]
		Task<IEnumerable<ComradeSlim>> GetComradeList(int userId);

		[ItemNotNull]
		Task<IEnumerable<ViewModels.User.ComradeRequest>> GetAllComradeRequests(int userId);

		/// <summary>
		/// Works only on prefetched request, marks agreed, creates comrades, then saves changes.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		Task MakeComradesFromRequest(ComradeRequest request);
	}
}
