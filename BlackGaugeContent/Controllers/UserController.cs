using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bgc.Data;
using Bgc.Data.Contracts;
using Bgc.Models;
using Bgc.ViewModels.Account;
using Bgc.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bgc.Controllers
{
	[Route("api/[controller]/[action]")]
	public class UserController : Controller
	{
		private readonly BgcFullContext _context;
		private readonly IUserRepository _users;

		public UserController(BgcFullContext context, IUserRepository users)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
			_users   = users ?? throw new ArgumentNullException(nameof(users));
		}


		[HttpGet]
		[AllowAnonymous]
		public async Task<IEnumerable<Gender>> GetGenders()
		{
			return await _users.GetGenders();
		}

		[HttpGet("{value}/{type}")]
		[Produces("application/json")]
		[AllowAnonymous]
		public async Task<RegisterValueUniqueness> CheckUniqueness(string value, string type)
		{
			var result = new RegisterValueUniqueness() { ValueType = type };
			IQueryable<string> query;
			if ("name".Equals(type))
			{
				query = _context.Users.Select(u => u.UserName);
			}
			else if ("email".Equals(type))
			{
				query = _context.Users.Select(u => u.Email);
			}
			else
				return null;

			result.Unique = await query.FirstOrDefaultAsync(u => u == value) == null;
			return result;
		}

		[HttpGet("{userId}")]
		[Authorize(Policy = "BgcUser")]
		public async Task<UserAccountDetails> GetAccountDetails(int userId)
		{
			if (userId <= 0)
				return null;
			var user = await _users.GetBgcUser(userId);
			if (user == null)
				return new UserAccountDetails();
			var details = new UserAccountDetails()
			{
				Alive = true,
				GenderId = user.GenderId,
				Motto = user.Motto
			};
			return details;
		}

		[HttpPost]
		[Authorize(Policy = "BgcUser")]
		[ValidateAntiForgeryToken]
		public async Task<AccountFeedback> SetAccountDetails([FromBody] UserAccountDetails details)
		{
			if (ModelState.IsValid == false)
				return null;

			var user = await _users.GetUser(details.UserId, detectChanges: true);
			if (user == null)
				return null;
			user.GenderId = (byte)details.GenderId;
			user.Motto = details.Motto;
			await _users.SaveChanges();
			return new AccountFeedback(){Result = FeedResult.Success};
		}

		[HttpGet("{userName}")]
		[Authorize(Policy = "BgcUser")]
		public async Task<UserPublicDetails> GetUserPublicDetails(string userName)
		{
			if (String.IsNullOrEmpty(userName))
				return new UserPublicDetails();
			var user = await _users.GetBgcUserInfo(userName);
			if (user == null)
				return new UserPublicDetails();
			var details = new UserPublicDetails()
			{
				UserName   = user.Name,
				Respek     = user.Respek,
				Motto      = user.Motto,
				GenderName = user.GenderName
			};
			return details;
		}
	}
}
