using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bgc.Api;
using Bgc.Data.Contracts;
using Bgc.Models;
using Bgc.ViewModels.User;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using ComradeRequest = Bgc.Models.ComradeRequest;

namespace Bgc.Data.Implementations
{
	public class ComradesRepository : BaseRepo, IComradeRepository
	{
		public ComradesRepository(BgcFullContext context) : base(context, null) {}

		public async Task<ComradeRequest> FetchComradeRequest(int senderId, string receiverName, bool createOnly = false)
		{
			var receiverId = await GetUserId(receiverName);
			if(senderId == receiverId)
				throw new InvalidOperationException("comrade request sent by receiver.");

			var request = await GetComradeRequest(senderId, receiverId);
			if (request == null)
			{
				request = new ComradeRequest()
				{
					SenderId = senderId,
					ReceiverId = receiverId,
					Since = DateTime.Now
				};
				_context.ComradeRequests.Add(request);
				SaveChanges();
			}
			else if(createOnly)
				throw new SyncException("comrade request already exists.");
			return request;
		}

		private Task<ComradeRequest> GetComradeRequest(int s, int r)
		{
			return _context.ComradeRequests
				.FirstOrDefaultAsync(rq => rq.SenderId == s && rq.ReceiverId == r);
		}

		public async Task<IEnumerable<ComradeSlim>> GetComradeList(int userId)
		{
			var query = (from c in _context.Comrades.AsNoTracking()
						join u1 in _context.Users.AsNoTracking()
							on c.FirstId equals u1.Id
						where c.SecondId == userId
						select new ComradeSlim()
						{
							Name         = u1.UserName,
							Interactions = c.InteractionCount
						}).Union(
						 from c in _context.Comrades.AsNoTracking()
						join u2 in _context.Users.AsNoTracking()
							on c.SecondId equals u2.Id
						where c.FirstId == userId
						select new ComradeSlim()
						{
							Name         = u2.UserName,
							Interactions = c.InteractionCount
						})
						.OrderByDescending(c => c.Interactions);
			return await query.ToListAsync();
		}

		public async Task<IEnumerable<ViewModels.User.ComradeRequest>> GetAllComradeRequests(
			int userId)
		{
			var query = (from c in _context.ComradeRequests.AsNoTracking()
						join u1 in _context.Users.AsNoTracking()
							on c.SenderId equals u1.Id
						where c.ReceiverId == userId && c.Agreed == false
						select new ViewModels.User.ComradeRequest()
						{
							Id        = c.Id,
							OtherName = u1.UserName,
							// JSON ignored
							Received  = true,
							Since     = c.Since,
							Seen      = c.Seen
						}).Union(
						 from r in _context.ComradeRequests.AsNoTracking()
						join u2 in _context.Users.AsNoTracking()
							on r.ReceiverId equals u2.Id
						where r.SenderId == userId && r.Agreed == false
						select new ViewModels.User.ComradeRequest()
						{
							Id        = r.Id,
							OtherName = u2.UserName,
							Received  = false,
							Since     = r.Since
						})
						.OrderBy(r => r.Since);
			return await query.ToListAsync();
		}

		public async Task<ComradeRequest> DrawComradeRequest(int requestId)
		{
			var request = await _context.ComradeRequests
						.FirstOrDefaultAsync(r => r.Id == requestId);

			_context.Attach(request);
			return request;
		}

		public Comrades MakeComradesFromRequest([NotNull] ComradeRequest request)
		{
			var comrades = new Comrades()
			{
				FirstId  = request.SenderId,
				SecondId = request.ReceiverId,
				Since    = DateTime.Now
			};
			_context.Comrades.Add(comrades);
			return comrades;
		}

		public async Task<int> CountActiveRequests(int userId)
		{
			return await _context.ComradeRequests.AsNoTracking()
				.Where(c => c.ReceiverId == userId && !c.Agreed && !c.Seen)
				.CountAsync();
		}
	}
}
