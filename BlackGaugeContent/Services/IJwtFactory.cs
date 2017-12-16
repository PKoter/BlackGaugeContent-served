using System.Security.Claims;
using JetBrains.Annotations;

namespace Bgc.Services
{
	public interface IJwtFactory
	{
		string GenerateEncodedToken([NotNull] ClaimsIdentity identity);
		[NotNull]
		ClaimsIdentity GenerateClaimsIdentity(string userName,string id);
	}
}
