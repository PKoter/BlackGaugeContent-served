using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security;
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
			#if DEBUG
				Debug.Assert(req.SenderId != null, "req.SenderId != null"); 
			#endif
			var request = await 
				_comrades.FetchComradeRequest(req.SenderId.Value, req.OtherName, createOnly: true);
			var result = new Feedback()
			{
				Result = FeedResult.Success
			};
			return result;
		}

		[HttpGet("{userId}")]
		public async Task<ComradeRelatedSet> GetUserComradeRelations(int userId)
		{
			if (userId <= 0)
				return null;
			var comrades = await _comrades.GetComradeList(userId);
			var requests = await _comrades.GetAllComradeRequests(userId);
			var received = new List<ComradeRequest>();
			var sent     = new List<ComradeRequest>();
			foreach (var r in requests)
			{
				if(r.Received)
					received.Add(r);
				else
					sent.Add(r);
				r.TimeSpan = TimeSpan(r.Since);
			}
			var set = new ComradeRelatedSet()
			{
				Received    = received,
				Sent     = sent,
				Comrades = comrades
			};
			return set;
		}

		/// <summary>
		/// Returns text representation of approximate time span since request.
		/// </summary>
		/// todo: change text generation method.
		/// <param name="time"></param>
		/// <returns></returns>
		private string TimeSpan(DateTime time)
		{
			var span = DateTime.Now - time;
			var days = span.Days;
			if (days > 1)
				return days + " days";
			if (days > 0)
				return days + " day";
			var hours = span.Hours;
			if (hours > 1)
				return hours + " hours";
			if (hours > 0)
				return hours + " hour";
			var minutes = (int)Math.Ceiling((double)span.Minutes);
			if(minutes > 1)
				return minutes + " minutes";
			return minutes + " minute";
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<Feedback> ConfirmComradeRequest([FromBody] ComradeRequestFeedback req)
		{
			if(req.Id <= 0 || req.ReceiverId <= 0)
				return new Feedback();
			var request = await _comrades.DrawComradeRequest(req.Id);
			if (request == null || request.Agreed || request.ReceiverId != req.ReceiverId)
			{
				Debug.Fail("Security break: comrade request confirmation failed.");
				throw new SecurityException();
			}
			await _comrades.MakeComradesFromRequest(request);
			return new Feedback {Result = FeedResult.Success};
		}
	}

	public static class _ComradeRequests
	{
		public static async Task FillRequestsInModel(this IComradeRepository comrades, [NotNull] IComradeModel model, int userId)
		{
			model.Received = await comrades.GetReceivedComradeRequests(userId);
			model.Agreed   = await comrades.GetAgreedComradeRequests(userId);
		}
	}
}
