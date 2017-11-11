using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bgc.Api;
using Bgc.Data.Contracts;
using Bgc.Models;
using Bgc.ViewModels.Bgc;
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

		public async Task<IEnumerable<MemeModel>> PageElements(int pageIndex)
		{
			if(pageIndex < 0)
				throw new ArgumentOutOfRangeException();
			return await _context.Memes
				.Skip(R.MemesOnPageCount * pageIndex)
				.Take(R.MemesOnPageCount)
				.Select(m => new MemeModel()
				{
					Core = new MemeCore()
					{
						Id      = m.Id,
						Title   = m.Title,
						AddTime = m.AddTime,
						Base64  = m.Base64
					},
					State = new MemeState()
					{
						Rating       = m.Rating,
						CommentCount = m.CommentCount
					}
				})
				.ToListAsync();
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

	}
}
