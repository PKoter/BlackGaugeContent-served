﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Bgc.Api;
using Bgc.Data.Contracts;
using Bgc.Development;
using Microsoft.EntityFrameworkCore;

namespace Bgc.Data.Implementations
{
	public class BaseRepo : IDbRepository
	{
		protected readonly BgcFullContext _context;
		private            DatabaseProxy  _proxy;

		public BaseRepo(BgcFullContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
			_proxy   = context.Proxy;
		}

		public async Task<int> SaveChanges()
		{
			if (_proxy.BlockSaving)
				return 0;
			return await _context.SaveChangesAsync();
		}

		/// <summary>
		/// Returns id for username. Valid only for existing users.
		/// </summary>
		/// <param name="userName"></param>
		/// <exception cref="Exception">user does not exist</exception>
		/// <returns></returns>
		protected async Task<int> GetUserId(string userName)
		{
			return await _context.Users.AsNoTracking()
				.Where(u => u.UserName == userName)
				.Select(u => u.Id)
				.FirstAsync();
		}

		/// <summary>
		/// Returns username for id. Valid only for existing users.
		/// </summary>
		/// <param name="userId"></param>
		/// <exception cref="Exception">user does not exist</exception>
		/// <returns></returns>
		public async Task<string> GetUserName(int userId)
		{
			return await _context.Users.AsNoTracking()
				.Where(u => u.Id == userId)
				.Select(u => u.UserName)
				.FirstAsync();
		}

		/// <summary>
		/// Returns if user exists.
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="throw">should throw exception if negative result?</param>
		/// <returns></returns>
		protected async Task<bool> DoesUserExist(int userId, bool @throw = true)
		{
			var result = await _context.Users.AnyAsync(u => u.Id == userId);
			if (@throw && result == false)
			{
				throw new SqlEntityException(
					"No valid user for {0} credential!", nameof(userId));
			}
			return result;
		}
	}
}
