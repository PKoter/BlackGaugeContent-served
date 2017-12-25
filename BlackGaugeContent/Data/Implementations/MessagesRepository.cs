using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bgc.Data.Contracts;
using Bgc.Models;
using Bgc.Models.QueryReady;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

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
			var lastSent = await GetLastUserMessage(userId, otherName);
			var lastSeen = await GetLastSeenMessage(userId, otherName);
			// no sent message and no seen
			if (lastSent == null && lastSeen == null)
				// only other's messages can exist then - load em from the start
				return await GetOthersMessages(userId, otherName, page: 0);

			int  otherId;
			long lastMessageId;
			if (lastSent != null)
			{
				otherId       = lastSent.ReceiverId;
				lastMessageId = lastSent.Id;
			}
			else
			{
				otherId       = lastSeen.SenderId;
				lastMessageId = lastSeen.Id;
			}
			// if both messages exist, choose later one
			if (lastSent != null && lastSeen != null)
				lastMessageId = Math.Max(lastSent.Id, lastSeen.Id);

			// messages that are sent by other user after last user message or last seen
			var aheadQuery = (from m in _context.Messages.AsNoTracking()
							 where m.Id > lastMessageId 
								 && m.SenderId == otherId && m.ReceiverId == userId
							 orderby m.Id
							 select m).Take(MessagesAhead);

			// messages sent before last message
			var beforeQuery= (from m in _context.Messages.AsNoTracking()
							 where m.Id < lastMessageId &&
								(m.SenderId == userId  && m.ReceiverId == otherId
								||  m.SenderId == otherId && m.ReceiverId == userId)
							 orderby m.Id descending 
							 select m).Take(MessagesBefore);

			var messages = await aheadQuery.Union(beforeQuery).ToListAsync();

			// queries omit last message to avoid retrieving duplicated data
			messages.Add(lastSent);
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

		/// <summary>
		/// Every first chat query must determine where is last user message
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="otherName"></param>
		/// <returns></returns>
		[ItemCanBeNull]
		private async Task<Message> GetLastUserMessage(int userId, string otherName)
		{
			var query = from m in _context.Messages.AsNoTracking()
						let otherId = (GetUsersIdFromName(otherName)).First()
						where m.SenderId == userId && m.ReceiverId == otherId
						orderby m.Id descending
						select m;
			return await query.FirstOrDefaultAsync();
		}

		[ItemCanBeNull]
		private async Task<Message> GetLastSeenMessage(int userId, string otherName)
		{
			var query = from m in _context.Messages.AsNoTracking()
						let otherId = (GetUsersIdFromName(otherName)).First()
						where m.SenderId == otherId && m.ReceiverId == userId && m.Seen
						orderby m.Id descending
						select m;
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
	}
}
