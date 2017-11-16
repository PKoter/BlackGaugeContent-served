using Bgc.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bgc.Data
{
	public class ApplicationDbContext : IdentityDbContext
														 <AspUser, 
														 AspRole, 
														 int, 
														 AspUserClaim, 
														 AspUserRole, 
														 AspUserLogin, 
														 AspRoleClaim, 
														 AspUserToken>
	{
		public ApplicationDbContext(DbContextOptions<BgcFullContext> options)
			: base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
			builder.Entity<AspUser>().ToTable("Users");
			builder.Entity<AspRole>().ToTable("AspRoles");
			builder.Entity<AspUserRole>().ToTable("AspUserRoles");
			builder.Entity<AspUserToken>().ToTable("AspUserTokens");
			builder.Entity<AspUserLogin>().ToTable("AspUserLogins");
			builder.Entity<AspUserClaim>().ToTable("AspUserClaims");
			builder.Entity<AspRoleClaim>().ToTable("AspRoleClaims");
			if (Database.IsSqlite())
			{
				builder.Entity<AspUser>().Property(b => b.Id)
					//.ValueGeneratedOnAdd();
					.ValueGeneratedNever();

				builder.Entity<AspRole>().Property(b => b.Id)
					//.ValueGeneratedOnAdd();
					.ValueGeneratedNever();

				builder.Entity<AspUserRole>().Property(b => b.RoleId)
					//.ValueGeneratedOnAdd();
					.ValueGeneratedNever();
				builder.Entity<AspUserRole>().Property(b => b.UserId)
					//.ValueGeneratedOnAdd();
					.ValueGeneratedNever();

				builder.Entity<AspUserToken>().Property(b => b.UserId)
					//.ValueGeneratedOnAdd();
					.ValueGeneratedNever();

				builder.Entity<AspUserLogin>().Property(b => b.UserId)
					//.ValueGeneratedOnAdd();
					.ValueGeneratedNever();

				builder.Entity<AspUserClaim>().Property(b => b.Id)
					//.ValueGeneratedOnAdd();
					.ValueGeneratedNever();

				builder.Entity<AspRoleClaim>().Property(b => b.Id)
					//.ValueGeneratedOnAdd();
					.ValueGeneratedNever();
			}
			builder.Entity<AspUser>(b => b.Property(p => p.UserName).HasColumnName("Name"));
			// Customize the ASP.NET Identity model and override the defaults if needed.
			// For example, you can rename the ASP.NET Identity table names and more.
			// Add your customizations after calling base.OnModelCreating(builder);
		}
	}
}
