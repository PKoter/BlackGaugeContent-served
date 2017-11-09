using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Bgc.Development
{
	public class CcarAttribute : ClaimsAuthorizationRequirement
	{
		public CcarAttribute(string claimType, IEnumerable<string> allowedValues) : base(claimType, allowedValues) {}


		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ClaimsAuthorizationRequirement requirement)
		{
			if(context.User != null)
			{
				if(requirement.AllowedValues == null || !requirement.AllowedValues.Any() 
					? context.User.Claims.Any(c => string.Equals(c.Type, requirement.ClaimType, StringComparison.OrdinalIgnoreCase)) 
					: context.User.Claims.Any(c => 
					{
						if(string.Equals(c.Type, requirement.ClaimType, StringComparison.OrdinalIgnoreCase))
							return requirement.AllowedValues.Contains(c.Value, StringComparer.Ordinal);
						return false;
					}))
					context.Succeed(requirement);
			}
			return Task.CompletedTask;
		}
	}
}
