using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bgc.Api;
using Bgc.Data.Contracts;
using Bgc.Models.QueryReady;
using Bgc.Services.Signals;
using Bgc.ViewModels;
using Bgc.ViewModels.Account;
using Bgc.ViewModels.Signals;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bgc.Controllers
{
	[Route("api/[controller]/[action]")]
	[Authorize(Policy = R.Privileges.User)]
	public class MessageController : Controller
	{
		private readonly IMessageRepository _messages;
		private readonly ISignalDispatcher  _signalDispatcher;

		public MessageController(IMessageRepository messages, ISignalDispatcher signalDispatcher)
		{
			_messages         = messages;
			_signalDispatcher = signalDispatcher;
		}

		[HttpGet("{userId}/{otherName}")]
		public async Task<IEnumerable<ViewModels.Signals.Message>> GetSurroundingLastMessages(
		  int userId, string otherName)
		{
			if(userId <= 0 
			  || string.IsNullOrWhiteSpace(otherName) 
			  || otherName.Length < R.ModelRules.MinNameLength)
				return null;

			var messages = await _messages.GetMessagesAroundLast(userId, otherName);
			var msgs = new List<ViewModels.Signals.Message>(messages.Count);
			msgs.AddRange(messages.Select(m => new ViewModels.Signals.Message()
			{
				Sent = m.SenderId == userId,
				Text = m.Text
			}));
			// only last and first messages need to have id
			if (msgs.Count > 0)
			{
				msgs[0].Id             = messages.First().Id;
				msgs[msgs.Count -1].Id = messages.Last().Id;
			}

			return msgs;
		}

		[HttpPost]
		public async Task<Feedback> SendMessage([FromBody] Message message)
		{
			if (!ModelState.IsValid)
				return null;

			// get required data before actual processings
			var usersIds = await _messages.GetUsersIdAndName(message.OtherName, message.UserId);
			var otherId  = usersIds.First(u => u.Name == message.OtherName).Id;
			var userName = usersIds.First(u => u.Id == message.UserId).Name;

			// save sent message to storage
			var saveData = new MessageData
			{
				UserId  = message.UserId,
				OtherId = otherId,
				Text    = message.Text
			};
			var messageId = _messages.SaveMessage(saveData);

			// send message to receiver with right data
			var passedMessage = new Message()
			{
				Id        = messageId,
				OtherName = userName,
				Text      = message.Text
			};
			await _signalDispatcher
				.PushImpulse(message.OtherName, R.ImpulseType.Message, passedMessage);

			// if we get this far then there's solid chance everthing is ok.
			return new Feedback() {Result = FeedResult.Success};
		}


		[HttpPost]
		public async Task<Feedback> SeenMessage([FromBody] MessageState state)
		{
			if (ModelState.IsValid == false)
				return null;

			var message = await _messages.DrawMessage(state.Id);
			if (message.Seen)
				return null;

			message.Seen = true;
			_messages.SaveChanges();
			return new Feedback() { Result = FeedResult.Success };
		}

		[HttpGet("{msgId}/{userId}/{otherName}")]
		public async Task<IEnumerable<Message>> GetPreviousMessages(long msgId,
			int userId, string otherName)
		{
			var chat = new ChatState()
			{
				MessageId = msgId,
				UserId    = userId,
				OtherName = otherName
			};
			if (TryValidateModel(chat) == false)
				return null;

			var messages = await _messages.GetPreviousMessages(chat);

			return PreprareModels(messages, userId);
		}
		[HttpGet("{msgId}/{userId}/{otherName}")]
		public async Task<IEnumerable<Message>> GetNextMessages(long msgId,
			int userId, string otherName)
		{
			var chat = new ChatState()
			{
				MessageId = msgId,
				UserId    = userId,
				OtherName = otherName
			};
			if(TryValidateModel(chat) == false)
				return null;

			var messages = await _messages.GetNextMessages(chat);

			return PreprareModels(messages, userId);
		}

		private IList<Message> PreprareModels([NotNull] IList<Models.Message> messages, int userId)
		{
			var msgs = new List<Message>(messages.Count);

			msgs.AddRange(messages.Select(m => new Message()
			{
				Sent = m.SenderId == userId,
				Text = m.Text
			}));
			// only last and first messages need to have id
			if(msgs.Count > 0)
			{
				msgs[0].Id = messages.First().Id;
				msgs[msgs.Count - 1].Id = messages.Last().Id;
			}
			return msgs;
		}
	}
}
