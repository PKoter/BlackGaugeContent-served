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
		public void MemeReaction_MemeRatingShouldIncrease()
		{
			var reaction = StubMemeReaction(1);
			var repo = CreateMockRepository(reaction);
			var controller = new MemeListController(repo);

			var result = controller.MemeReaction(reaction);
			Assert.AreEqual(1, result.Result.Rating);
		}

		[TestMethod]
		public void MemeReaction_MemeRatingShouldDecrease()
		{
			var reaction = StubMemeReaction(-1);
			var repo = CreateMockRepository(reaction);
			var controller = new MemeListController(repo);

			var result = controller.MemeReaction(reaction);

			Assert.AreEqual(-1, result.Result.Rating);
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

		private IBgcMemeRepository CreateMockRepository(MemeReaction reaction)
		{
			var mockRepo = Substitute.For<IBgcMemeRepository>();
			mockRepo.GetSingle(1)
				.Returns(Task.FromResult(new Meme()
				{
					Id = 1,
					Rating = 0,
				}));
			mockRepo.GetOrMakeRating(reaction)
				.Returns(Task.FromResult(new MemeRating()
				{
					MemeId = 1,
					UserId = 1
				}));
			return mockRepo;
		}
		#endregion
	}
}
