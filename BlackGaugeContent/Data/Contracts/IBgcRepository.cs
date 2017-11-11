using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Bgc.Data.Contracts
{
	public interface IBgcRepository<T, T2> where T: class where T2 : class
	{
		[ItemNotNull]
		Task<IEnumerable<T2>> PageElements(int pageIndex);

		[ItemNotNull]
		Task<T> GetSingle(int elementId);

		void SaveChanges();
	}
}
