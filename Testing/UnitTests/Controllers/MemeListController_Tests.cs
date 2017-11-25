using System.Threading.Tasks;
using Bgc.Controllers;
using Bgc.Data.Contracts;
using Bgc.Models;
using Bgc.ViewModels.Bgc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Testing.UnitTests.Controllers
{
	[TestClass]
	public class MemeListController_Tests
	{
		[TestMethod]
		public async Task MemeReaction_MemeRatingShouldIncrease()
		{
			var reaction = StubMemeReaction(1);
			var repo = CreateMockRepository(reaction, 0);
			var controller 
				= new MemeListController(repo, Substitute.For<IBgcSessionsRepository>());

			var result = await controller.MemeReaction(reaction);
			Assert.AreEqual(1, result.Rating);
		}

		[TestMethod]
		public async Task MemeReaction_MemeRatingShouldDecrease()
		{
			var reaction = StubMemeReaction(-1);
			var repo = CreateMockRepository(reaction, 0);
			var controller 
				= new MemeListController(repo, Substitute.For<IBgcSessionsRepository>());

			var result = await controller.MemeReaction(reaction);

			Assert.AreEqual(-1, result.Rating);
		}

		[TestMethod]
		public async Task MemeReaction_MemeRatingShouldBeDeleted()
		{
			var reaction = StubMemeReaction(0);
			var repo = CreateMockRepository(reaction, 1);
			var controller 
				= new MemeListController(repo, Substitute.For<IBgcSessionsRepository>());

			var result = await controller.MemeReaction(reaction);

			Assert.AreEqual(-1, result.Rating);

		}
		#region stub mocks
		private MemeReaction StubMemeReaction(sbyte vote)
		{
			return new MemeReaction()
			{
				MemeId = 1,
				UserId = 1,
				Vote = vote
			};
		}

		private IBgcMemeRepository CreateMockRepository(MemeReaction reaction, sbyte vote)
		{
			var mockRepo = Substitute.For<IBgcMemeRepository>();
			mockRepo.DrawMeme(1)
				.Returns(Task.FromResult(new Meme()
				{
					Id = 1,
					Rating = 0,
				}));
			mockRepo.FetchRating(reaction)
				.Returns(Task.FromResult(new MemeRating()
				{
					MemeId = reaction.MemeId,
					UserId = reaction.UserId,
					Vote   = vote
				}));
			return mockRepo;
		}
		#endregion
	}
}
