using System.Collections.Generic;
using System.Threading.Tasks;
using Bgc.Models;
using Bgc.Models.QueryReady;
using JetBrains.Annotations;

namespace Bgc.Data.Contracts
{
	public interface IMessageRepository : IDbRepository
	{
		/// <summary>
		/// Retrieves 10 messages surrounding last user message or last seen message, or first mesages sent by other.
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="otherName"></param>
		/// <returns></returns>
		[ItemNotNull]
		Task<IList<Message>> GetMessagesAroundLast(int userId, string otherName);

		/// <summary>
		/// Saves message to storage and returns assigned id.
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		Task<long> SaveMessage([NotNull] MessageData message);

		[ItemNotNull]
		Task<IList<IdAndName>> GetUsersIdAndName(string otherName, int userId);

		[ItemNotNull]
		Task<Message> DrawMessage(long messageId);
	}
}
