using System.Collections.Generic;
using System.Threading.Tasks;
using Bgc.Models;
using Bgc.ViewModels.Bgc;
using JetBrains.Annotations;

// ReSharper disable ConditionalAnnotation

namespace Bgc.Data.Contracts
{
	public interface IBgcMemeRepository : IBgcRepository<Meme, MemeModel>
	{
		/// <summary>
		/// Gets user rating for given name or creates new.
		/// </summary>
		/// <param name="reaction"></param>
		/// <returns></returns>
		[NotNull]
		[ItemNotNull]
		Task<MemeRating> FetchRating(MemeReaction reaction);

		void DeleteRating(MemeRating rating);

		[ItemNotNull]
		Task<IList<MemeModel>> PageMemesAfter(int userId, int lastMemeId);

		Task<int> CountMemesBefore(int firstMemeId);
	}
}
