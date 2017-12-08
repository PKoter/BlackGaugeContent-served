using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Bgc.Api;
using Bgc.Data.Contracts;
using Bgc.Models;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

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

			var request    = await GetComradeRequest(senderId, receiverId);
			if (request == null)
			{
				request = new ComradeRequest()
				{
					SenderId = senderId,
					ReceiverId = receiverId,
					Since = DateTime.Today
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

		public async Task<ComradeRequest> ComradeRequestAgreed(int receiverId, string senderName)
		{
			var senderId = await GetUserId(senderName);
			var request  = await GetComradeRequest(senderId, receiverId);
			if(request == null) 
				throw new SyncException("no comrade request.");
			request.Agreed = true;
			_context.ComradeRequests.Remove(request);
			await _context.SaveChangesAsync();
			return request;
		}

		public async Task SaveComrades([NotNull] Comrade first, [NotNull] Comrade second)
		{
			_context.Comrades.Add(first);
			_context.Comrades.Add(second);
			await _context.SaveChangesAsync();
		}

		public async Task<IEnumerable<ViewModels.User.ComradeRequest>> GetReceivedComradeRequests
		(int userId)
		{
			return await GetSpecificRequests(r => r.ReceiverId == userId, 
				request => request.SenderId);
		}

		public async Task<IEnumerable<ViewModels.User.ComradeRequest>> GetAgreedComradeRequests
		(int userId)
		{
			return await GetSpecificRequests(r => r.SenderId == userId && r.Agreed,
				request => request.ReceiverId);
		}

		public async Task<IEnumerable<ComradeRequest>> GetUserRelatedRequests(int userId)
		{
			return await _context.ComradeRequests.AsNoTracking()
				.Where(r => r.ReceiverId == userId || r.Agreed && r.SenderId == userId)
				.ToListAsync();
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
						OtherName = u.UserName,
						Agreed = r.Agreed,
						Since = r.Since
					}).ToListAsync();
		}
	}
}
