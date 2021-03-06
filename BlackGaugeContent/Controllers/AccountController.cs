﻿using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Bgc.Models;
using Bgc.Services;
using Bgc.Services.Signals;
using Bgc.ViewModels.Account;
using Bgc.ViewModels.Signals;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bgc.Controllers
{
	[Authorize]
	[Route("api/[controller]/[action]")]
	public class AccountController : Controller
	{
		private readonly UserManager<AspUser>   _userManager;
		private readonly SignInManager<AspUser> _signInManager;
		private readonly IEmailSender           _emailSender;
		private readonly ILogger                _logger;
		private readonly IJwtFactory            _jwtFactory;
		private readonly IUserImpulse      _userImpulse;
		private readonly JwtIssuerOptions       _jwtOptions;

		public AccountController(
			UserManager<AspUser>       userManager,
			SignInManager<AspUser>     signInManager,
			IEmailSender               emailSender,
			ILogger<AccountController> logger,
			IOptions<JwtIssuerOptions> optionsIssuer,
			IJwtFactory                jwtFactory,
			IUserImpulse          userImpulse
		)
		{
			_userManager      = userManager;
			_signInManager    = signInManager;
			_emailSender      = emailSender;
			_logger           = logger;
			_jwtOptions       = optionsIssuer.Value;
			_jwtFactory       = jwtFactory;
			_userImpulse = userImpulse;
		}

		[TempData]
		public string ErrorMessage { get; set; }


		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login([FromBody] LoginModel model, string returnUrl = null)
		{
			if (!ModelState.IsValid)
				return ValidationFail;

			var user = await _userManager.FindByEmailAsync(model.Email);
			if (user == null)
				return NoSuchUserError;
				
			if (!await _userManager.IsEmailConfirmedAsync(user))
			{
				return Json(new AccountFeedback()
				{
					Message = "You must have a confirmed email to log in."
				});
			}
			// This doesn't count login failures towards account lockout
			// To enable password failures to trigger account lockout, set lockoutOnFailure: true
			var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
			if (result.IsLockedOut)
			{
				_logger.LogWarning("User account locked out.");
				return AccountLockoutError;
			}
			if (result.Succeeded)
			{
				_logger.LogInformation($"User {model.Email} logged in.");
				var notifyState = await _userImpulse.GetFullInteractionsState(user.Id);
				return JwtAuth(user, notifyState);
			}

			return Json(new AccountFeedback()
			{
				Message = "Wrong username or password"
			});
		}

		#region Error properties
		/// <summary>
		/// If we got this far, something failed, send validation error
		/// </summary>
		private JsonResult ValidationFail {
			get
			{
				return Json(new AccountFeedback()
				{
					Message = "Something went wrong. Given values do not satisfy validation rules."
				});
			}
		}

		/// <summary>
		/// If we got this far, something failed, send validation error
		/// </summary>
		private JsonResult NoSuchUserError {
			get
			{
				return Json(new AccountFeedback() {Message = "User doesn't exist."});
			}
		}

		private JsonResult AccountLockoutError {
			get
			{
				return Json(new AccountFeedback() {Message = "Account locked."});
			}
		}
		#endregion

		/// <summary>
		/// Called after Login or Logout to transfer user principal, to generate actual antiforgery token.
		/// Hack: HttpContext.AuthenticateAsync should provide principal asap AuthenticationMiddleware finishes Login etc. but this never happens. I don't have mental strength to investigate it atm.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[AllowAnonymous]
		public JsonResult EnsureAuthTransfer()
		{
			return Json(new {Success = true});
		}

		/// <summary>
		/// Returns Jwt token when user logged successfully.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="notifyState">any push state, messages or notifications</param>
		/// <returns></returns>
		private IActionResult JwtAuth([NotNull] AspUser user, UserImpulseState notifyState)
		{
			var identity = _jwtFactory.GenerateClaimsIdentity(user.UserName, user.Id.ToString());

			return Json(new {
				Result     = FeedResult.Success,
				userId     = user.Id,
				userName   = user.UserName,
				auth_token = _jwtFactory.GenerateEncodedToken(identity),
				expires_in = (int)_jwtOptions.ValidFor.TotalSeconds,
				impulses   = notifyState
			});
		}
		#region 2F/Recovery
		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> LoginWith2fa(bool rememberMe, string returnUrl = null)
		{
			// Ensure the user has gone through the username & password screen first
			var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

			if (user == null)
			{
				throw new ApplicationException($"Unable to load two-factor authentication user.");
			}

			var model = new LoginWith2faViewModel { RememberMe = rememberMe };
			ViewData["ReturnUrl"] = returnUrl;

			return View(model);
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> LoginWith2fa(LoginWith2faViewModel model, bool rememberMe, string returnUrl = null)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
			if (user == null)
			{
				throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			var authenticatorCode = model.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

			var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, model.RememberMachine);

			if (result.Succeeded)
			{
				_logger.LogInformation("User with ID {UserId} logged in with 2fa.", user.Id);
				return RedirectToLocal(returnUrl);
			}
			else if (result.IsLockedOut)
			{
				_logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
				return AccountLockoutError;
			}
			else
			{
				_logger.LogWarning("Invalid authenticator code entered for user with ID {UserId}.", user.Id);
				ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
				return View();
			}
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> LoginWithRecoveryCode(string returnUrl = null)
		{
			// Ensure the user has gone through the username & password screen first
			var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
			if (user == null)
			{
				throw new ApplicationException($"Unable to load two-factor authentication user.");
			}

			ViewData["ReturnUrl"] = returnUrl;

			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoveryCodeViewModel model, string returnUrl = null)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
			if (user == null)
			{
				throw new ApplicationException($"Unable to load two-factor authentication user.");
			}

			var recoveryCode = model.RecoveryCode.Replace(" ", string.Empty);

			var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

			if (result.Succeeded)
			{
				_logger.LogInformation("User with ID {UserId} logged in with a recovery code.", user.Id);
				return RedirectToLocal(returnUrl);
			}
			if (result.IsLockedOut)
			{
				_logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
				return AccountLockoutError;
			}
			else
			{
				_logger.LogWarning("Invalid recovery code entered for user with ID {UserId}", user.Id);
				ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");
				return View();
			}
		}
		#endregion

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<JsonResult> Register([FromBody] RegisterModel model)
		{
			if (!ModelState.IsValid)
				return ValidationFail;
			try
			{
				var user = new AspUser
				{
					UserName = model.Name, 
					Email = model.Email,
					GenderId = (byte)model.GenderId,
				};
				var result = await _userManager.CreateAsync(user, model.Password);
				if (result.Succeeded)
				{
					var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
					code = HttpUtility.UrlEncode(code);
					var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
					await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);

					_logger.LogInformation($"User {user.UserName} created a new account with password.");
					return Json(new AccountFeedback()
					{
						Result = FeedResult.Redirect,
						Message = "Your account has been successfully created. You've been sent an email to activate your account."
					});
				}
				AddErrors(result);
				return ValidationFail;
			}
			catch (Exception)
			{
				return Json(new AccountFeedback()
				{
					Message = "Something went wrong. Make sure you passed valid email."
				});
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			_logger.LogInformation("User logged out.");
			return NoContent();
		}

		#region External login
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public IActionResult ExternalLogin(string provider, string returnUrl = null)
		{
			// Request a redirect to the external login provider.
			var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
			var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
			return Challenge(properties, provider);
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
		{
			if (remoteError != null)
			{
				ErrorMessage = $"Error from external provider: {remoteError}";
				return RedirectToAction(nameof(Login));
			}
			var info = await _signInManager.GetExternalLoginInfoAsync();
			if (info == null)
			{
				return RedirectToAction(nameof(Login));
			}

			// Sign in the user with this external login provider if the user already has a login.
			var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
			if (result.Succeeded)
			{
				_logger.LogInformation("User logged in with {Name} provider.", info.LoginProvider);
				return RedirectToLocal(returnUrl);
			}
			if (result.IsLockedOut)
			{
				return AccountLockoutError;
			}
			else
			{
				// If the user does not have an account, then ask the user to create an account.
				ViewData["ReturnUrl"] = returnUrl;
				ViewData["LoginProvider"] = info.LoginProvider;
				var email = info.Principal.FindFirstValue(ClaimTypes.Email);
				return View("ExternalLogin", new ExternalLoginViewModel { Email = email });
			}
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginViewModel model, string returnUrl = null)
		{
			if (ModelState.IsValid)
			{
				// Get the information about the user from the external login provider
				var info = await _signInManager.GetExternalLoginInfoAsync();
				if (info == null)
				{
					throw new ApplicationException("Error loading external login information during confirmation.");
				}
				var user = new AspUser { UserName = model.Email, Email = model.Email };
				var result = await _userManager.CreateAsync(user);
				if (result.Succeeded)
				{
					result = await _userManager.AddLoginAsync(user, info);
					if (result.Succeeded)
					{
						await _signInManager.SignInAsync(user, isPersistent: false);
						_logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
						return RedirectToLocal(returnUrl);
					}
				}
				AddErrors(result);
			}

			ViewData["ReturnUrl"] = returnUrl;
			return View(nameof(ExternalLogin), model);
		}
		#endregion

		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> ConfirmEmail(string userId, string code)
		{
			if (userId == null || code == null)
			{
				return RedirectToAction(nameof(HomeController.Index), "Home");
			}
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return NoSuchUserError;
			}
			// thanks to angular replacing + with spaces, need to be reversed.
			code = HttpUtility.UrlDecode(code).Replace(' ', '+');
			var result = await _userManager.ConfirmEmailAsync(user, code);
			FeedResult type;
			string message;
			if (result.Succeeded)
			{
				type = FeedResult.Success;
				message = "Email successfully confirmed";
			}
			else
			{
				type = FeedResult.Error;
				message = "An error occurred while trying to confirm an email.";
			}
			return Json(new AccountFeedback() {Result = type, Message = message});
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await _userManager.FindByEmailAsync(model.Email);
				if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
				{
					// Don't reveal that the user does not exist or is not confirmed
					return Ok();
				}

				// For more information on how to enable account confirmation and password reset please
				// visit https://go.microsoft.com/fwlink/?LinkID=532713
				var code = await _userManager.GeneratePasswordResetTokenAsync(user);
				var callbackUrl = Url.ResetPasswordCallbackLink(user.Id, code, Request.Scheme);
				await _emailSender.SendEmailAsync(model.Email, "Reset Password",
				   $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");
				return Ok();
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult ResetPassword(string code = null)
		{
			if (code == null)
			{
				throw new ApplicationException("A code must be supplied for password reset.");
			}
			var model = new ResetPasswordViewModel { Code = code };
			return View(model);
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}
			var user = await _userManager.FindByEmailAsync(model.Email);
			if (user == null)
			{
				// Don't reveal that the user does not exist
				return Ok();
			}
			var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
			if (result.Succeeded)
			{
				return Ok();
			}
			AddErrors(result);
			return View();
		}

		#region Helpers

		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}
		}

		private IActionResult RedirectToLocal(string returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}
			else
			{
				return RedirectToAction(nameof(HomeController.Index), "Home");
			}
		}

		#endregion
	}
}
