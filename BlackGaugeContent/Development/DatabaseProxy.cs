namespace Bgc.Development
{
	/// <summary>
	/// Used to block save of data when it's nuisance to dump it each time testing.
	/// </summary>
	public class DatabaseProxy
	{
		public bool BlockSaving {get; set;} = false;

		public DatabaseProxy()
		{
			_instance = this;
		}

		private static DatabaseProxy _instance;

	}
}
