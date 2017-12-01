using System;

namespace Bgc.Api
{
	public class SqlEntityException : Exception
	{
		public SqlEntityException(string message) : base(message)
		{
		}

		public SqlEntityException(string message, params object[] args) : base(string.Format(message, args))
		{
		}
	}

	public class SyncException : Exception
	{
		public SyncException(string message) : base(message)
		{
		}

		public SyncException(string message, params object[] args) : base(string.Format(message, args))
		{
		}
	}
}
