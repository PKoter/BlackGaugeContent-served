using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Bgc.Api;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;

namespace Bgc.Services
{
	public class JwtFactory : IJwtFactory
	{
		private readonly JwtIssuerOptions _jwtOptions;

		public JwtFactory(IOptions<JwtIssuerOptions> jwtOptions)
		{
			_jwtOptions = jwtOptions.Value;
			ThrowInvalidOptions(_jwtOptions);
		}

		private static void ThrowInvalidOptions([NotNull] JwtIssuerOptions options)
		{
			if (options == null) throw new ArgumentNullException(nameof(options));

			if (options.ValidFor <= TimeSpan.Zero)
				throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));

			if (options.SigningCredentials == null)
				throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));

			if (options.JtiGenerator == null)
				throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
		}

		public string GenerateEncodedToken(ClaimsIdentity identity)
		{
			var claims = identity.Claims.Concat(new[]
			{
				//new Claim(JwtRegisteredClaimNames.Jti, _jwtOptions.JtiGenerator),
				new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate
					(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64)
			});

			// Create the JWT security token and encode it.
			var jwt = new JwtSecurityToken(
				issuer:             _jwtOptions.Issuer,
				audience:           _jwtOptions.Audience,
				claims:             claims,
				notBefore:          _jwtOptions.NotBefore,
				expires:            _jwtOptions.Expiration,
				signingCredentials: _jwtOptions.SigningCredentials);

			return new JwtSecurityTokenHandler().WriteToken(jwt);
		}

		public ClaimsIdentity GenerateClaimsIdentity(string userName,string id)
		{
			var claims = new[]
			{
				new Claim(JwtRegisteredClaimNames.UniqueName, userName),
				new Claim(R.AuthTags.Id, id),
				new Claim(R.AuthTags.Role, R.AuthTags.ApiAccess)
			};
			var ci = new ClaimsIdentity(claims, "token", JwtRegisteredClaimNames.UniqueName,
				ClaimsIdentity.DefaultRoleClaimType);
			return ci;
		}

		/// <returns>Date converted to seconds since Unix epoch (Jan 1, 1970, midnight UTC).</returns>
		private static long ToUnixEpochDate(DateTime date)
		{
			return (long)Math.Round(
				(date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
				.TotalSeconds);
		}
	}
}
