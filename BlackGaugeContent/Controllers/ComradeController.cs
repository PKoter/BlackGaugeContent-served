using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security;
using System.Threading.Tasks;
using Bgc.Api;
using Bgc.Data.Contracts;
using Bgc.Services.Signals;
using Bgc.ViewModels;
using Bgc.ViewModels.Account;
using Bgc.ViewModels.Signals;
using Bgc.ViewModels.User;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Bgc.Controllers
{
	[Route("api/[controller]/[action]")]
	[Authorize(Policy = R.Privileges.User)]
	public class ComradeController: Controller
	{
		private readonly IUserRepository    _users;
		private readonly IComradeRepository _comrades;
		private readonly ISignalDispatcher _signalDispatcher;

		public ComradeController(IUserRepository users, IComradeRepository comrades, ISignalDispatcher signalDispatcher)
		{
			Debug.Assert(users != null, nameof(users) + " != null");
			_users = users;
			Debug.Assert(comrades != null, nameof(comrades) + " != null");
			_comrades = comrades;
			Debug.Assert(signalDispatcher != null, nameof(signalDispatcher) + " != null");
			_signalDispatcher = signalDispatcher;
		}

		private Feedback Success {get {return new Feedback() {Result = FeedResult.Success};}}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<Feedback> MakeComradeRequest([FromBody] ComradeRequest req)
		{
			if (ModelState.IsValid == false)
				return null;
			Debug.Assert(req.SenderId != null, "req.SenderId != null"); 
			var name = req.OtherName;
			var request = await 
				_comrades.FetchComradeRequest(req.SenderId.Value, name, createOnly: true);
			var answer = new ComradeRequestImpulse()
			{
				RequestId = request.Id
			};

			await _signalDispatcher.PushImpulse(name, R.ImpulseType.ComradeRequest, answer);
			return Success;
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
				Received = received,
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
			if (!ModelState.IsValid)
				return null;
			var request = await _comrades.DrawComradeRequest(req.Id);
			if (request == null || request.Agreed || request.ReceiverId != req.ReceiverId)
			{
				Debug.Fail("Security break: comrade request confirmation failed.");
				throw new SecurityException();
			}
			await _comrades.MakeComradesFromRequest(request);
			return Success;
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<Feedback> SeenComradeRequest([FromBody] SeenComradeRequest req)
		{
			if (!ModelState.IsValid || req.Seen == false)
				return null;
			var request = await _comrades.DrawComradeRequest(req.Id);
			if (request == null || request.Agreed || request.Seen == req.Seen)
			{
				Debug.Fail("Security break: comrade request confirmation failed.");
				throw new SecurityException();
			}
			request.Seen = req.Seen;
			await _comrades.SaveChanges();
			return Success;
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
