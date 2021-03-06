﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bgc.Api;
using Bgc.Data.Contracts;
using Bgc.Models;
using Bgc.ViewModels.Bgc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bgc.Controllers
{
	[Route("api/[controller]/[action]")]
	public class MemeListController : Controller
	{
		private readonly IBgcMemeRepository _repo;
		private readonly IBgcSessionsRepository _sessions;

		public int PageMemeCount {private get; set;} = R.MemesOnPageCount;

		public MemeListController(IBgcMemeRepository repo, IBgcSessionsRepository sessions)
		{
			_repo     = repo ?? throw new ArgumentNullException(nameof(repo));
			_sessions = sessions ?? throw new ArgumentNullException(nameof(sessions));
		}

		[HttpGet("{pageIndex}/{userId}")]
		[AllowAnonymous]
		public async Task<IEnumerable<MemeModel>> PageMemes(int pageIndex, int userId)
		{
			if(userId == 0)
				return await _repo.PageElements(pageIndex, userId);
			var session = await _sessions.FetchMemeSession(userId);
			IEnumerable<MemeModel> memes;
			if (pageIndex == 0 || session.LastMemeId == 0)
				memes = await _repo.PageElements(pageIndex, userId);
			else
			{
				memes = await _repo.PageMemesAfter(userId, session.LastMemeId);
			}
			UpdateMemeSessionAsync(memes, session);
			return memes;
		}

		private void UpdateMemeSessionAsync(IEnumerable<MemeModel> memes, MemeUserSession session)
		{
			session.LastMemeId  = memes.Last().Core.Id;
			session.FirstMemeId = memes.First().Core.Id;
			_sessions.SaveChanges();
		}

		[HttpGet("{pageIndex}/{firstMemeId}")]
		[AllowAnonymous]
		public async Task<ElementCount> CountNewMemes(int pageIndex, int firstMemeId)
		{
			if (pageIndex < 0 || firstMemeId <= 0)
				return null;
			var count = await _repo.CountMemesBefore(firstMemeId);
			count -= PageMemeCount * pageIndex;
			return new ElementCount{Count = count};
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(policy: R.Privileges.User)]
		public async Task<MemeState> MemeReaction([FromBody] MemeReaction reaction)
		{
			if (ModelState.IsValid == false)
				return null;
			
			var meme = await _repo.DrawMeme(reaction.MemeId);
			var earlierRating = await _repo.FetchRating(reaction);
			// prevent forgering of likes without client
			if (reaction.Vote == earlierRating.Vote)
				return null;
			// add rating
			if (reaction.Vote != 0)
			{
				meme.Rating += reaction.Vote - earlierRating.Vote;
				earlierRating.Vote = reaction.Vote;
			}
			// remove previous, user undid his rating
			else
			{
				meme.Rating -= earlierRating.Vote;
				_repo.DeleteRating(earlierRating);
			}

			_repo.SaveChanges();

			return new MemeState()
			{
				CommentCount = meme.CommentCount,
				Rating       = meme.Rating,
				Vote         = reaction.Vote
			};
		}
	
	}
}
