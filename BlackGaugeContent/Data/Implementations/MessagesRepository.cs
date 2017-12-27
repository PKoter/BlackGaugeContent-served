using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Bgc.Data.Contracts;
using Bgc.Models.QueryReady;
using Bgc.ViewModels.Signals;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Message = Bgc.Models.Message;

namespace Bgc.Data.Implementations
{
	public class MessagesRepository : BaseRepo, IMessageRepository
	{
		private const int MessagesAhead  = 5;
		private const int MessagesBefore = 4;
		private const int PageSize       = 10;

		public MessagesRepository(BgcFullContext context) : base(context)
		{
		}

		//todo: security against ie. sql injection
		public async Task<long> SaveMessage([NotNull] MessageData msg)
		{
			var message = new Message()
			{
				ReceiverId = msg.OtherId,
				SenderId   = msg.UserId,
				SentTime   = DateTime.Now,
				Text       = msg.Text
			};
			_context.Messages.Add(message);
			await SaveChanges();
			return message.Id;
		}


		public async Task<IList<Message>> GetMessagesAroundLast(int userId, string otherName)
		{   
			var lastMsg = await GetLastSeenOrSentMessage(userId, otherName);
			if(lastMsg == null)
				// only other's messages can exist then - load em from the start
				return await GetOthersMessages(userId, otherName, page: 0);

			int  otherId   = lastMsg.SenderId != userId ? lastMsg.SenderId : lastMsg.ReceiverId;
			long lastMsgId = lastMsg.Id;

			// messages that are sent by other user after last user message or last seen
			var aheadQuery = (from m in _context.Messages.AsNoTracking()
							 where m.Id > lastMsgId 
								 && m.SenderId == otherId && m.ReceiverId == userId
							 orderby m.Id
							 select m).Take(MessagesAhead);

			// messages sent before last message
			var beforeQuery= (from m in _context.Messages.AsNoTracking()
							 where m.Id < lastMsgId &&
								(m.SenderId == userId  && m.ReceiverId == otherId
								||  m.SenderId == otherId && m.ReceiverId == userId)
							 orderby m.Id descending 
							 select m).Take(MessagesBefore);

			var messages = await aheadQuery.Union(beforeQuery).ToListAsync();

			// queries omit last message to avoid retrieving duplicated data
			messages.Add(lastMsg);
			messages.Sort((a, b) => a.SentTime.CompareTo(b.SentTime));

			return messages;
		}

		/// <summary>
		/// User sent no messages, so we fetch only messages sent by other.
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="otherName"></param>
		/// <param name="page"></param>
		/// <returns></returns>
		private async Task<IList<Message>> GetOthersMessages(int userId, string otherName, 
			int page)
		{
			var query = from m in _context.Messages.AsNoTracking()
						let otherId = (GetUsersIdFromName(otherName)).First()
						where m.SenderId == otherId && m.ReceiverId == userId
						orderby m.Id
						select m;

			return await query
				.Skip(page * PageSize)
				.Take(PageSize)
				.ToListAsync();
		}

		[ItemCanBeNull]
		private async Task<Message> GetLastSeenOrSentMessage(int userId, string otherName)
		{
			var query = (from m in _context.Messages.AsNoTracking()
						let otherId = (GetUsersIdFromName(otherName)).First()
						where (m.SenderId == userId && m.ReceiverId == otherId)
						   || (m.SenderId == otherId && m.ReceiverId == userId && m.Seen)
						orderby m.Id descending
						select m);
			return await query.FirstOrDefaultAsync();
		}
		
		[NotNull]
		private IQueryable<int> GetUsersIdFromName(string name)
		{
			return from u in _context.Users
				   where u.UserName == name
				   select u.Id;
		}

		[ItemNotNull]
		public async Task<IList<IdAndName>> GetUsersIdAndName(string name, int userId)
		{
			return await (from u in _context.Users
				   where u.UserName == name || u.Id == userId
				   select new IdAndName(){
						Id   = u.Id, 
						Name = u.UserName
				   }).Take(2).ToListAsync();
		}


		public async Task<Message> DrawMessage(long messageId)
		{
			var message = await _context.Messages.FirstAsync(m => m.Id == messageId);
			_context.Attach(message);
			return message;
		}


		private IQueryable<long> GetLastSeenOrSentId(int userId, int otherId)
		{
			var query = (from m in _context.Messages
						where (m.SenderId == userId && m.ReceiverId == otherId) 
						   || (m.SenderId == otherId && m.ReceiverId == userId && m.Seen)
						orderby m.Id descending
						select m.Id);//.Take(1);
			return query;
		}

		public async Task<IList<ChatImpulse>> GetChatImpulses(int userId)
		{
			var cmd = BuildSpCommand("GetChatImpulses");

			var param = cmd.CreateParameter();
			param.Value         = userId;
			param.DbType        = DbType.Int32;
			param.ParameterName = "@userId";

			cmd.Parameters.Add(param);

			return await cmd.ToListAsync<ChatImpulse>();

			/*var query = 
				from u in _context.Users
				where (from c in _context.Comrades
					   where c.FirstId == userId
					   select c.SecondId)
					   .Union(
					   from c in _context.Comrades
					   where c.SecondId == userId
					   select c.FirstId).Any(c => c == u.Id)

				select new ChatImpulse()
				{
					Comrade = u.UserName,
					Impulses = (from m in _context.Messages
								let mId = (from m1 in _context.Messages
										   where (m1.SenderId == userId && m1.ReceiverId == u.Id)
										      || (m1.SenderId == u.Id && m1.ReceiverId == userId 
										      &&  m1.Seen)
										   orderby m.Id descending
										   select m.Id).FirstOrDefault()
								where m.Id > mId
								select m).Count()
				};*/
		}
	}
}
