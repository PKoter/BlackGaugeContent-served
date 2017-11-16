using System.Threading.Tasks;
using Bgc.Models;
using Bgc.ViewModels.Bgc;
using JetBrains.Annotations;

// ReSharper disable ConditionalAnnotation

namespace Bgc.Data.Contracts
{
	public interface IBgcMemeRepository : IBgcRepository<Meme, MemeModel>
	{
		[NotNull]
		[ItemNotNull]
		Task<MemeRating> GetOrMakeRating(MemeReaction reaction);

		void DeleteRating(MemeRating rating);
	}
}
