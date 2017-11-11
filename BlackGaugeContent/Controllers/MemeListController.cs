using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bgc.Data.Contracts;
using Bgc.ViewModels.Bgc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bgc.Controllers
{
	[Route("api/[controller]")]
	public class MemeListController : Controller
	{
		private readonly IBgcMemeRepository _repo;


		public MemeListController(IBgcMemeRepository repo)
		{
			_repo = repo;
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<IEnumerable<MemeModel>> PageMemes(int pageIndex)
		{
			return await _repo.PageElements(pageIndex);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(policy: "BgcUser")]
		public async Task<MemeState> MemeReaction([FromBody] MemeReaction reaction)
		{
			if (ModelState.IsValid && reaction.Vote != 0)
			{
				try
				{
					var meme = await _repo.GetSingle(reaction.MemeId);
					meme.Rating += reaction.Vote;

					var earlierReaction = await _repo.GetOrMakeRating(reaction);

					earlierReaction.Vote = reaction.Vote;

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
