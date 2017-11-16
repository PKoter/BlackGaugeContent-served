using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bgc.Data.Contracts;
using Bgc.ViewModels.Bgc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bgc.Controllers
{
	[Route("api/[controller]/[action]")]
	public class MemeListController : Controller
	{
		private readonly IBgcMemeRepository _repo;


		public MemeListController(IBgcMemeRepository repo)
		{
			_repo = repo;
		}

		[HttpGet("{pageIndex}")]
		[AllowAnonymous]
		public async Task<IEnumerable<MemeModel>> PageMemes(int pageIndex)
		{
			return await _repo.PageElements(pageIndex);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		//[Authorize(policy: "BgcUser")]
		public async Task<MemeState> MemeReaction([FromBody] MemeReaction reaction)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var meme = await _repo.GetSingle(reaction.MemeId);
					var earlierRating = await _repo.GetOrMakeRating(reaction);
					// prevent forgering of likes without client
					if (reaction.Vote == earlierRating.Vote)
						return null;
					// add rating
					if (reaction.Vote != 0)
					{
						meme.Rating += reaction.Vote;
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
						Rating = meme.Rating
					};
				}
				catch (Exception e) 
				{
					#if DEBUG
					throw e;
					#endif
				}
			}
			return new MemeState();
		}
	
	}
}
