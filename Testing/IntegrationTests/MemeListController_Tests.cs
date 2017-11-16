using System;
using System.Threading.Tasks;
using Bgc.Controllers;
using Bgc.Data;
using Bgc.Data.Implementations;
using Bgc.ViewModels.Bgc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Testing.IntegrationTests
{
	[TestClass]
	public class MemeListController_Tests
	{
		private readonly Random _random = new Random();

		[TestMethod]
		public async Task MemeReaction_DbIntegration()
		{
			using (var contextProvider = new EFInMemoryDbCreator.Sqlite<BgcFullContext>())
			{
				// arrange
				var repo = new BgcMemeRepo(
					contextProvider.CreateContext(c => c.SeedUsers(1).SeedMemes()));

				var controller = new MemeListController(repo);
				var reaction = new MemeReaction()
				{
					MemeId = _random.Next(1, EFInMemoryDbCreator.MemeCount),
					UserId = 1,
					Vote = (sbyte)(_random.Next(0, 10) % 2 == 0 ? -1 : 1)
				};
				var mRating = (await repo.GetSingle(reaction.MemeId)).Rating;
				

				// act
				var outcome = await controller.MemeReaction(reaction);
				// fresh context
				repo = new BgcMemeRepo(contextProvider.GetFreshContext());

				// prepare assert
				var rating = await repo.GetOrMakeRating(reaction);

				var meme2 = await repo.GetSingle(reaction.MemeId);

				// assert
				Assert.AreEqual(reaction.Vote, rating.Vote);
				Assert.AreNotEqual(meme2.Rating, mRating);
				mRating += reaction.Vote;
				Assert.AreEqual(mRating, meme2.Rating);
				Assert.AreEqual(outcome.Rating, meme2.Rating);
				Assert.AreEqual(1, rating.Id);
			}
		}
	}
}
