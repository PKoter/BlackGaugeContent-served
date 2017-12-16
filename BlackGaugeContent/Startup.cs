using System;
using System.Text;
using Bgc.Api;
using Bgc.Data;
using Bgc.Data.Contracts;
using Bgc.Data.Implementations;
using Bgc.Extensions;
using Bgc.Models;
using Bgc.Services;
using Bgc.Services.Signals;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Bgc
{
	public class Startup
	{
		private SecurityKey _signinKey;
		private ILoggerFactory _loggerFactory;

		public Startup(IConfiguration configuration, ILoggerFactory loggerFactory)
		{
			Configuration  = configuration;
			_loggerFactory = loggerFactory;
			_signinKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["SymmetricSecurityKey"]));
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			AddDataServices(services);

			services.AddIdentity<AspUser, AspRole>()
				.AddEntityFrameworkStores<BgcFullContext>()
				.AddDefaultTokenProviders();

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

				options.SignIn.RequireConfirmedEmail = true;
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


			services.AddAntiforgery(options =>
				{
					options.HeaderName = "X-XSRF-TOKEN";
					options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
					options.SuppressXFrameOptionsHeader = false;
					options.Cookie.HttpOnly = false;
				});
			
			services.AddMvc(options =>
				{
					//options.Filters.AddService(typeof(AntiforgeryCookieResultFilter));
				})
				.AddRazorPagesOptions(options =>
				{
					options.Conventions.AuthorizeFolder("/Account/Manage");
					options.Conventions.AuthorizePage("/Account/Logout");
				});

			//services.AddTransient<AntiforgeryCookieResultFilter>();

			services.AddTransient<IEmailSender, EmailSender>();
			services.Configure<MvcOptions>(options =>
				{
					options.Filters.Add(new RequireHttpsAttribute());
					//options.Filters.Add(new ValidateAntiForgeryTokenAttribute());
					options.Filters.Add(new ExceptionFilter(_loggerFactory));
				});
			services.Configure<AuthMessageSenderOptions>(Configuration);

			services.AddAuthorization(options => 
			{
				options.AddPolicy("BgcUser", policy => 
					//policy.RequireClaim(R.AuthTags.Role, R.AuthTags.ApiAccess)
					policy.RequireClaim(R.AuthTags.Role, R.AuthTags.ApiAccess)
				);
			});
			ConfigureJwtAuthServices(services);

			services.AddSignalR();
			services.AddTransient<ISignalDispatcher, SignalDispatcher>();
		}

		/// <summary>
		/// Configures servies to provide data providers for dependency injection.
		/// </summary>
		/// <param name="services"></param>
		private void AddDataServices(IServiceCollection services)
		{
			services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
			services.AddDbContext<BgcFullContext>(options =>
				options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

			services.AddTransient<IBgcMemeRepository, BgcMemeRepo>();
			services.AddTransient<IBgcSessionsRepository, BgcSessionsRepo>();
			services.AddTransient<IUserRepository, UserRepo>();
			services.AddTransient<IComradeRepository, ComradesRepository>();
			services.AddTransient<IUserImpulse, UserImpulse>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, IAntiforgery antiforgery, ILoggerFactory loggingFactory)
		{
			var options = new RewriteOptions().AddRedirectToHttps();
			
			app.UseRewriter(options);
			app.UseHttpStrictTransportSecurity(sts => { sts.MaxAge = TimeSpan.FromDays(90); });
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

			app.UseMiddleware<SignalAuthorizationMiddleware>();
			app.UseAuthentication();
			app.UseMiddleware<AntiforgeryMiddleware>();

			app.UseSignalR(routes =>
			{
				routes.MapHub<SignalHub>("bgcImpulses");
			});

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
				routes.MapSpaFallbackRoute(
					name: "spa-fallback",
					defaults: new { controller = "Home", action = "Index" });
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
