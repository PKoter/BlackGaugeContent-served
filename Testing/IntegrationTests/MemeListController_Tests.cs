using System;
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
		public void MemeReaction_DbIntegration()
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
				var memeTask = repo.GetSingle(reaction.MemeId);
				memeTask.Wait();
				var mRating = memeTask.Result.Rating;

				// act
				var result = controller.MemeReaction(reaction);
				result.Wait();
				var outcome = result.Result;

				// prepare assert
				var memeRating = repo.GetOrMakeRating(reaction);
				memeRating.Wait();
				var rating = memeRating.Result;

				memeTask = repo.GetSingle(reaction.MemeId);
				memeTask.Wait();
				var meme2 = memeTask.Result;

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
