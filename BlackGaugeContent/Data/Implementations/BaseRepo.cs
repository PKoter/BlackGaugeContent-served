using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Bgc.Api;
using Bgc.Data.Contracts;
using Bgc.Development;
using JetBrains.Annotations;
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

		[NotNull]
		protected DbCommand BuildSpCommand(string spName)
		{
			var command = _context.Database.GetDbConnection().CreateCommand();
			command.CommandText = spName;
			command.CommandType = CommandType.StoredProcedure;
			return command;
		}
	}

	internal static class SqlExtensions
	{
		/// <summary>
		/// Executes stored procedure command and builds list of DTO. Caution: names of columns and fields must match.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="command"></param>
		/// <returns></returns>
		[ItemNotNull]
		internal static async Task<List<T>> ToListAsync<T>([NotNull] this DbCommand command) where T : class, new()
		{
			var list = new List<T>();
			using(command)
			{
				if((command.Connection.State & ConnectionState.Open) == 0)
					command.Connection.Open();

				using(var reader = await command.ExecuteReaderAsync())
				{
					var colNumber = reader.FieldCount;
					var properties = new List<PropertyInfo>(colNumber);
					var type = typeof(T);

					for(int i = 0; i < colNumber; i++)
					{
						var colName = reader.GetName(i);
						properties.Add(type.GetProperty(colName));
					}

					while(reader.Read())
					{
						var obj = new T();
						for(int i = 0; i < colNumber; i++)
						{
							properties[i].SetValue(obj, reader[i]);
						}

						list.Add(obj);
					}
				}
			}
			return list;
		}

		/// <summary>
		/// Executes stored procedure command and builds list of DTO. Map is used to cast column data into right properties.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="command"></param>
		/// <param name="map"></param>
		/// <returns></returns>
		[ItemNotNull]
		internal static async Task<List<T>> ToListAsync<T>([NotNull] this DbCommand command, Action<DbDataReader, T> map) where T : class, new()
		{
			var list = new List<T>();
			using(command)
			{
				if((command.Connection.State & ConnectionState.Open) == 0)
					command.Connection.Open();

				using(var reader = await command.ExecuteReaderAsync())
				{
					while(reader.Read())
					{
						var obj = new T();
						map(reader, obj);
						list.Add(obj);
					}
				}
			}
			return list;
		}
	}
}
