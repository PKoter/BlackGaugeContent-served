using System;
using System.Text;
using Bgc.Api;
using Bgc.Data;
using Bgc.Models;
using Bgc.Services;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Bgc
{
	public class Startup
	{
		private SecurityKey _signinKey;

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
			_signinKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["SymmetricSecurityKey"]));
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
			services.AddDbContext<BgcContext>(options =>
				options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

			services.AddIdentity<AspUser, AspRole>(config => config.SignIn.RequireConfirmedEmail = true)
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddDefaultTokenProviders();

			services.AddMvc()
				.AddRazorPagesOptions(options =>
				{
					options.Conventions.AuthorizeFolder("/Account/Manage");
					options.Conventions.AuthorizePage("/Account/Logout");
				});

			services.Configure<IdentityOptions>(options =>
			{
				// Password settings
				options.Password.RequireDigit = true;
				options.Password.RequiredLength = 8;
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequireUppercase = true;
				options.Password.RequireLowercase = false;
				options.Password.RequiredUniqueChars = 6;

				// Lockout settings
				options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
				options.Lockout.MaxFailedAccessAttempts = 10;
				options.Lockout.AllowedForNewUsers = true;

				// User settings
				options.User.RequireUniqueEmail = true;
			});

			services.ConfigureApplicationCookie(options =>
			{
				// Cookie settings
				options.Cookie.HttpOnly = true;
				options.Cookie.Expiration = TimeSpan.FromDays(150);
				options.LoginPath = "/Account/Login"; // If the LoginPath is not set here, ASP.NET Core will default to /Account/Login
				options.LogoutPath = "/Account/Logout"; // If the LogoutPath is not set here, ASP.NET Core will default to /Account/Logout
				options.AccessDeniedPath = "/Account/AccessDenied"; // If the AccessDeniedPath is not set here, ASP.NET Core will default to /Account/AccessDenied
				options.SlidingExpiration = true;
			});

			services.AddAntiforgery(options => options.HeaderName = "X-XSRF-Token");

			// Register no-op EmailSender used by account confirmation and password reset during development
			// For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=532713
			services.AddTransient<IEmailSender, EmailSender>();

			services.Configure<MvcOptions>(options => options.Filters.Add(new RequireHttpsAttribute()));
			services.Configure<AuthMessageSenderOptions>(Configuration);

			services.AddAuthorization(options => 
			{
				options.AddPolicy("BgcUser", policy => 
					//policy.RequireClaim(R.AuthTags.Role, R.AuthTags.ApiAccess)
					policy.RequireClaim(R.AuthTags.Role, R.AuthTags.ApiAccess)
				);
			});
			ConfigureJwtAuthServices(services);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, IAntiforgery antiforgery)
		{
			ConfigureAntiforgery(app, antiforgery);
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseBrowserLink();
				app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
				{
					HotModuleReplacement = true
				});
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}

			app.UseStaticFiles();

			app.UseAuthentication();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
				routes.MapSpaFallbackRoute(
					name: "spa-fallback",
					defaults: new { controller = "Home", action = "Index" });
			});

			/*var options = new RewriteOptions().AddRedirectToHttps();
			app.UseRewriter(options);*/
		}

		private void ConfigureAntiforgery(IApplicationBuilder app, IAntiforgery antiforgery)
		{
			app.Use(next => context => 
			{
				if (context.Request.Path == "/")
				{
					//send the request token as a JavaScript-readable cookie, and Angular will use it by default
					var tokens = antiforgery.GetAndStoreTokens(context);
					context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken, new CookieOptions {HttpOnly = false});
				}
				return next(context);
			});
		}

		/// <summary>
		/// Configures authentication and authorization using Json Web Tokens
		/// </summary>
		/// <param name="services"></param>
		private void ConfigureJwtAuthServices(IServiceCollection services)
		{
			services.AddSingleton<IJwtFactory, JwtFactory>();
			var jwtOptionsConfig = Configuration.GetSection(nameof(JwtIssuerOptions));

			services.Configure<JwtIssuerOptions>(options => 
			{
				options.Issuer   = jwtOptionsConfig[nameof(JwtIssuerOptions.Issuer)];
				options.Audience = jwtOptionsConfig[nameof(JwtIssuerOptions.Audience)];
				options.SigningCredentials = new SigningCredentials(_signinKey, SecurityAlgorithms.HmacSha256);
			});

			var tokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidIssuer = jwtOptionsConfig[nameof(JwtIssuerOptions.Issuer)],

				ValidateAudience = true,
				ValidAudience = jwtOptionsConfig[nameof(JwtIssuerOptions.Audience)],

				ValidateIssuerSigningKey = true,
				IssuerSigningKey = _signinKey,

				RequireExpirationTime = false,
				ValidateLifetime = false,
				ClockSkew = TimeSpan.Zero
			};

			services.AddAuthorization(options => 
			{
				options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme).RequireAuthenticatedUser().Build();
			});
			services.AddAuthentication(options => 
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(options => options.TokenValidationParameters = tokenValidationParameters);
		}
	}
}
