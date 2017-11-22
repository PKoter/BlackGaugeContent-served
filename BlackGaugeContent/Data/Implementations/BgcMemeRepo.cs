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
	public class BgcMemeRepo : BaseRepo, IBgcMemeRepository
	{
		public int PageMemeCount {private get; set;} = R.MemesOnPageCount;

		public BgcMemeRepo(BgcFullContext context): base(context)
		{
		}

		public async Task<IEnumerable<MemeModel>> PageElements(int pageIndex, int userId)
		{
			if(pageIndex < 0)
				throw new ArgumentOutOfRangeException();
			var page = _context.Memes
				.OrderByDescending(m => m.Id)
				.Skip(PageMemeCount * pageIndex)
				.Take(PageMemeCount);
			return await PageMemes(userId, page);
		}

		public async Task<IList<MemeModel>> PageMemesAfter(int userId, int lastMemeId)
		{
			var page = _context.Memes
				.Where(m => m.Id < lastMemeId)
				.OrderByDescending(m => m.Id)
				.Take(PageMemeCount);
			return await PageMemes(userId, page);
		}

		private async Task<IList<MemeModel>> PageMemes(int userId, IQueryable<Meme> page)
		{
			return await
				(from meme in page
				join reacts in _context.MemeRatings
					on meme.Id equals reacts.MemeId into rts
				let rt = rts.FirstOrDefault(r => r.UserId == userId)
				select new MemeModel()
				{
					Core = new MemeCore()
					{
						Id = meme.Id,
						Title = meme.Title,
						AddTime = meme.AddTime,
						Base64 = meme.Base64
					},
					State = new MemeState()
					{
						Rating = meme.Rating,
						CommentCount = meme.CommentCount,
						Vote = rt != null ? rt.Vote : (sbyte)0
					}
				}).ToListAsync();
		}

		public async Task<int> CountMemesBefore(int firstMemeId)
		{
			return await _context.Memes
				.Where(m => m.Id > firstMemeId)
				.CountAsync();
		}

		public async Task<Meme> GetSingle(int elementId)
		{
			var element = await _context.Memes
				.FirstAsync(m => m.Id == elementId);
			_context.Attach(element);
			return element;
		}

		public async Task<MemeRating> FetchRating(MemeReaction reaction)
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
