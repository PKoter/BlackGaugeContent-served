using Bgc.Controllers;

namespace Microsoft.AspNetCore.Mvc
{
	public static class UrlHelperExtensions
	{
		public static string EmailConfirmationLink(this IUrlHelper urlHelper, int userId, string code, string scheme)
		{
			/*
			return urlHelper.Action(
				action: nameof(AccountController.ConfirmEmail),
				controller: "Account",
				values: new { userId, code },
				protocol: scheme).Replace("api/Account/C", "account/c");*/
			return $"{scheme}://localhost:44315/account/confirmEmail?userId={userId}&code={code}";
		}

		public static string ResetPasswordCallbackLink(this IUrlHelper urlHelper, int userId, string code, string scheme)
		{
			return urlHelper.Action(
				action: nameof(AccountController.ResetPassword),
				controller: "Account",
				values: new { userId, code },
				protocol: scheme);
		}
	}
}
