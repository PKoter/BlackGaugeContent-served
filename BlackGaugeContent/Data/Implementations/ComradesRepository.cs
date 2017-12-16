using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
		public ComradesRepository(BgcFullContext context) : base(context) {}

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
				await _context.SaveChangesAsync();
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

		public async Task<IEnumerable<ViewModels.User.ComradeRequest>> GetReceivedComradeRequests
		(int userId)
		{
			return await GetSpecificRequests(r => r.ReceiverId == userId && r.Agreed == false, 
				request => request.SenderId);
		}

		public async Task<IEnumerable<ViewModels.User.ComradeRequest>> GetAgreedComradeRequests
		(int userId)
		{
			return await GetSpecificRequests(r => r.SenderId == userId && r.Agreed,
				request => request.ReceiverId);
		}

		private async Task<IEnumerable<ViewModels.User.ComradeRequest>> GetSpecificRequests
			([NotNull] Expression<Func<ComradeRequest, bool>> predicate,
			 [NotNull] Expression<Func<ComradeRequest, int>> joinKeySelector)
		{
			return await _context.ComradeRequests.AsNoTracking()
					.Where(predicate)
					.Join(_context.Users,
					joinKeySelector,
					u => u.Id,
					(r, u) => new ViewModels.User.ComradeRequest()
					{
						Id = r.Id,
						OtherName = u.UserName,
						Agreed = r.Agreed,
						Since = r.Since
					}).ToListAsync();
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
							Agreed    = false,
							Received  = true,
							Since     = c.Since
						}).Union(
						 from r in _context.ComradeRequests.AsNoTracking()
						join u2 in _context.Users.AsNoTracking()
							on r.ReceiverId equals u2.Id
						where r.SenderId == userId && r.Agreed == false
						select new ViewModels.User.ComradeRequest()
						{
							Id        = r.Id,
							OtherName = u2.UserName,
							Agreed    = false,
							Received  = false,
							Since     = r.Since
						})
						.OrderBy(r => r.Since);
			return await query.ToListAsync();
		}

		public async Task<ComradeRequest> DrawComradeRequest(int requestId)
		{
			return await _context.ComradeRequests
				.FirstOrDefaultAsync(r => r.Id == requestId);
		}

		public async Task MakeComradesFromRequest(ComradeRequest request)
		{
			_context.Attach(request);
			request.Agreed = true;
			var comrades = new Comrade()
			{
				FirstId  = request.SenderId,
				SecondId = request.ReceiverId,
				Since    = DateTime.Now
			};
			_context.Comrades.Add(comrades);
			await _context.SaveChangesAsync();
		}
	}
}
