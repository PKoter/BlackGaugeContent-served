using System.Collections.Generic;
using System.Threading.Tasks;
using Bgc.Models;
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
		Task<IEnumerable<ComradeSlim>> GetComradeList(int userId);

		[ItemNotNull]
		Task<IEnumerable<ViewModels.User.ComradeRequest>> GetAllComradeRequests(int userId);

		/// <summary>
		/// Works only on prefetched request, which is used to create comrades.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		Task<Comrades> MakeComradesFromRequest(ComradeRequest request);

		/// <summary>
		/// Counts received requests that are pending and not seen.
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		Task<int> CountActiveRequests(int userId);

		Task<string> GetUserName(int userId);
	}
}
