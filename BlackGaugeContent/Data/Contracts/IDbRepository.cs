using System.Threading.Tasks;

namespace Bgc.Data.Contracts
{
	public interface IDbRepository
	{
		Task<int> SaveChanges();
	}
}
