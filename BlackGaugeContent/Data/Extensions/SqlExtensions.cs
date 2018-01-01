using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Bgc.Data.Extensions
{
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

		/// <summary>
		/// Executes stored procedure command and builds list of DTO. Caution: names of columns and fields must match. Uses cached mapping.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="command"></param>
		/// <param name="mapper"></param>
		/// <returns></returns>
		[ItemNotNull]
		internal static async Task<List<T>> ToListAsync<T>([NotNull] this DbCommand command, MappingManager mapper) where T : class, new()
		{
			var list = new List<T>();
			using(command)
			{
				if((command.Connection.State & ConnectionState.Open) == 0)
					command.Connection.Open();

				using(var reader = await command.ExecuteReaderAsync())
				{
					var map = mapper.GetSpMappingFor<T>(reader, command.CommandText);
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
