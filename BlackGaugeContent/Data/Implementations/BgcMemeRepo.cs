using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bgc.Api;
using Bgc.Data.Contracts;
using Bgc.Models;
using Bgc.ViewModels.Bgc;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Bgc.Data.Implementations
{
	public class BgcMemeRepo : IBgcMemeRepository
	{
		private readonly BgcFullContext _context;

		public BgcMemeRepo(BgcFullContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		public async Task<IEnumerable<MemeModel>> PageElements(int pageIndex, int userId)
		{
			if(pageIndex < 0)
				throw new ArgumentOutOfRangeException();
			var page = _context.Memes
				.Skip(R.MemesOnPageCount * pageIndex)
				.Take(R.MemesOnPageCount);
			return await 
				(from meme in page
				 join reacts in _context.MemeRatings
				 on meme.Id equals reacts.MemeId into rts
				 let rt = rts.FirstOrDefault(r => r.UserId == userId)
				 select new MemeModel()
				 {
				 	Core = new MemeCore()
				 	{
				 		Id      = meme.Id,
				 		Title   = meme.Title,
				 		AddTime = meme.AddTime,
				 		Base64  = meme.Base64
				 	},
				 	State = new MemeState()
				 	{
				 		Rating       = meme.Rating,
				 		CommentCount = meme.CommentCount,
				 		Vote = rt != null ? rt.Vote : (sbyte)0
				 	}
				 }).ToListAsync();
		}

		public async Task<Meme> GetSingle(int elementId)
		{
			var element = await _context.Memes
				.FirstAsync(m => m.Id == elementId);
			_context.Attach(element);
			return element;
		}

		public void SaveChanges()
		{
			_context.SaveChanges();
		}

		public async Task<MemeRating> GetOrMakeRating(MemeReaction reaction)
		{
			var rating = await _context.MemeRatings
				.FirstOrDefaultAsync(r =>
					r.MemeId == reaction.MemeId && r.UserId == reaction.UserId);

			if (rating == null)
			{
				rating = new MemeRating()
				{
					UserId = reaction.UserId,
					MemeId = reaction.MemeId,
				};
				await _context.MemeRatings.AddAsync(rating);
			}
			return rating;
		}

		public void DeleteRating([NotNull] MemeRating rating)
		{
			_context.MemeRatings.Remove(rating);
		}

	}
}
