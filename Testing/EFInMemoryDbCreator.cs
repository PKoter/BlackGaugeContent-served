using System;
using Bgc.Data;
using JetBrains.Annotations;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Testing
{
	[UsedImplicitly]
	public class EFInMemoryDbCreator
	{
		public const int MemeCount = 15;
		private static SqliteConnection _connection;

		/// <summary>
		/// Single test in-memory db instance.
		/// </summary>
		public class Sqlite<T> : IDisposable where T: DbContext
		{
			private SqliteConnection _connection;
			private DbContextOptions<T> _options;

			public Sqlite()
			{
				_connection = GetConnection();
			}

			[NotNull]
			public T CreateContext([NotNull] Action<T> seedAction)
			{
				_options = GetOptions<T>(_connection);
				var context = GetContext(_options);
				EnsureSchema(context);
				seedAction(context);
				return context;
			}

			public T GetFreshContext()
			{
				return GetContext<T>(_options);
			}

			private T2 GetContext<T2>(DbContextOptions<T2> options) where T2 : DbContext
			{
				return Activator.CreateInstance(typeof(T2), options) as T2;
			}

			private void EnsureSchema(DbContext context)
			{
				context.Database.EnsureCreated();
			}

			public void Dispose()
			{
				_connection?.Close();
			}
		}

		public static BgcFullContext CreateBgcContext()
		{
			_connection = GetConnection();
			var context = GetBgcContext(_connection);
			DbDataSeeder.SeedMemes(context, MemeCount);
			return context;
		}

		private static SqliteConnection GetConnection()
		{
			var connection = new SqliteConnection("DataSource=:memory:");
			connection.Open();
			return connection;
		}

		private static DbContextOptions<T> GetOptions<T>(SqliteConnection connection) where T: DbContext
		{
			return new DbContextOptionsBuilder<T>()
				.UseSqlite(connection, x => x.SuppressForeignKeyEnforcement())
				.Options;
		}

		private static BgcFullContext GetBgcContext(SqliteConnection connection)
		{
			var options = GetOptions<BgcFullContext>(connection);
			return new BgcFullContext(options);
		}

		public static void CloseConnection()
		{
			_connection?.Close();
		}
	}
}
