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
using Bgc.ViewModels.User;
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
		private readonly ISignalDispatcher  _signalDispatcher;

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
			if (ModelState.IsValid == false || req.SenderId == null)
				return null;
			
			var otherName = req.OtherName;
			var request   = await 
				_comrades.FetchComradeRequest(req.SenderId.Value, otherName, createOnly: true);

			var userName = await _comrades.GetUserName(req.SenderId.Value);

			var answer = new ComradeRequest()
			{
				Id        = request.Id,
				OtherName = userName,
				TimeSpan  = TimeSpan(request.Since)
			};
			await _signalDispatcher.PushImpulse(otherName, R.ImpulseType.ComradeRequest, answer);

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

		[HttpGet("{userId}")]
		public async Task<IEnumerable<ComradeSlim>> GetUserComrades(int userId)
		{
			if(userId <= 0)
				return null;
			var comrades = await _comrades.GetComradeList(userId);
			return comrades;
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
				return null;
			}

			request.Seen   = true;
			request.Agreed = true;
			var comrades   = _comrades.MakeComradesFromRequest(request);
			_comrades.SaveChanges();

			var answer = new ComradeRequest()
			{
				Id     = request.Id,
				Agreed = true,
			};
			await _signalDispatcher
				.PushImpulse(req.OtherName, R.ImpulseType.ComradeRequest, answer);
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
			_comrades.SaveChanges();
			return Success;
		}
	}
}
