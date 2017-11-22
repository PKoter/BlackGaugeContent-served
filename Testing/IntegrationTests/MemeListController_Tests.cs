using System;
using System.Linq;
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
				var context = contextProvider.CreateContext(c => c.SeedUsers(1).SeedMemes(10));
				var repo = new BgcMemeRepo(context);
				var sessions = new BgcSessionsRepo(context);

				var controller = new MemeListController(repo, sessions);
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
				var rating = await repo.FetchRating(reaction);

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

		[TestMethod]
		public async Task PageMemes_ShouldHandleDefaultSequence()
		{
			using (var contextProvider = new EFInMemoryDbCreator.Sqlite<BgcFullContext>())
			{
				// arrange
				var context = contextProvider.CreateContext(c => c.SeedUsers(1).SeedMemes(10));
				var repo = new BgcMemeRepo(context);
				var sessions = new BgcSessionsRepo(context);
				repo.PageMemeCount = 5;
				var controller = new MemeListController(repo, sessions);

				// first page count
				var outcome = await controller.PageMemes(0, 0);

				// count
				Assert.AreEqual(5, outcome.Count());
				var last = outcome.Last();

				// sequencial 
				outcome = await controller.PageMemes(1, 0);
				Assert.AreEqual(last.Core.Id -1, outcome.First().Core.Id);

				// new memes are automatically appended
				context.SeedMemes(1);
				outcome = await controller.PageMemes(1, 0);
				Assert.AreEqual(last.Core.Id, outcome.First().Core.Id);
			}
		}

		[TestMethod]
		public async Task PageMemes_ShouldHandleUserSession()
		{
			using (var contextProvider = new EFInMemoryDbCreator.Sqlite<BgcFullContext>())
			{
				// arrange
				var context = contextProvider.CreateContext(c => c.SeedUsers(1).SeedMemes(10));
				var repo = new BgcMemeRepo(context);
				var sessions = new BgcSessionsRepo(context);
				repo.PageMemeCount = 5;
				var controller = new MemeListController(repo, sessions);
				controller.PageMemeCount = 5;

				// first page count
				var outcome = await controller.PageMemes(0, 1);

				// session is saved
				var session = await sessions.FetchMemeSession(1);
				Assert.AreEqual(outcome.First().Core.Id, session.FirstMemeId);
				Assert.AreEqual(outcome.Last().Core.Id, session.LastMemeId);
				
				// sequence stays on user session
				var last = outcome.Last();
				context.SeedMemes(1);

				// sequencial 
				outcome = await controller.PageMemes(1, 1);
				Assert.AreEqual(last.Core.Id -1, outcome.First().Core.Id);

				// new memes do not interfere user session
				session = await sessions.FetchMemeSession(1);
				dynamic newCount = await controller.CountNewMemes(1, session.FirstMemeId);
				if (newCount.Count != 1) throw new AssertFailedException();
			}
		}
	}
}
