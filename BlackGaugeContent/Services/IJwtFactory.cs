using System.Security.Claims;

namespace Bgc.Services
{
	public interface IJwtFactory
	{
		string GenerateEncodedToken(string userName, ClaimsIdentity identity);
		ClaimsIdentity GenerateClaimsIdentity(string userName,string id);
	}
}
