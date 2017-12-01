using System.Diagnostics;
using System.Threading.Tasks;
using Bgc.Data.Contracts;
using Bgc.ViewModels;
using Bgc.ViewModels.Account;
using Bgc.ViewModels.User;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Bgc.Controllers
{
	[Route("api/[controller]/[action]")]
	[Authorize("BgcUser")]
	public class ComradeController: Controller
	{
		private readonly IUserRepository    _users;
		private readonly IComradeRepository _comrades;

		public ComradeController(IUserRepository users, IComradeRepository comrades)
		{
			Debug.Assert(users != null, nameof(users) + " != null");
			_users = users;
			Debug.Assert(comrades != null, nameof(comrades) + " != null");
			_comrades = comrades;
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<Feedback> MakeComradeRequest([FromBody] ComradeRequest req)
		{
			if (ModelState.IsValid == false)
				return null;
			
			var request = await 
				_comrades.FetchComradeRequest(req.SenderId, req.OtherName, createOnly: true);
			var result = new Feedback()
			{
				Result = FeedResult.Success
			};
			return result;
		}
	}

	public static class _ComradeRequests
	{
		public static async Task FillRequestsInModel(this IComradeRepository comrades, [NotNull] IComradeModel model, int userId)
		{
			model.RequestsReceived = await comrades.GetReceivedComradeRequests(userId);
			model.RequestsAgreed   = await comrades.GetAgreedComradeRequests(userId);
		}
	}
}
