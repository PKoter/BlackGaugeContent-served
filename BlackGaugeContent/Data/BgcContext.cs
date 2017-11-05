using Microsoft.EntityFrameworkCore;

namespace Bgc.Models
{
	public partial class BgcContext : DbContext
	{
		public virtual DbSet<Blacklist>   Blacklists   { get; set; }
		public virtual DbSet<Exclusive>   Exclusive    { get; set; }
		public virtual DbSet<Gender>      Genders      { get; set; }
		public virtual DbSet<MemeComment> MemeComments { get; set; }
		public virtual DbSet<Meme>        Memes        { get; set; }
		public virtual DbSet<AspUser>     Users        { get; set; }

		public BgcContext(DbContextOptions<BgcContext> options) : base(options) {}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Blacklist>(entity =>
			{
				entity.ToTable("Blacklists", "Community");

				entity.Property(e => e.Id)
					.HasColumnName("ID")
					.ValueGeneratedNever();

				entity.Property(e => e.BlacklistedId).HasColumnName("BlacklistedID");

				entity.Property(e => e.Since).HasColumnType("datetime");

				entity.Property(e => e.UserId).HasColumnName("UserID");

				entity.HasOne(d => d.Blacklisted)
					.WithMany(p => p.Blacklisted)
					.HasForeignKey(d => d.BlacklistedId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_Blacklists_BlockedUsers");

				entity.HasOne(d => d.User)
					.WithMany(p => p.BlacklistsUser)
					.HasForeignKey(d => d.UserId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_Blacklists_Users");
			});

			modelBuilder.Entity<Exclusive>(entity =>
			{
				entity.ToTable("Exclusive", "BGC");

				entity.Property(e => e.Id)
					.HasColumnName("ID")
					.ValueGeneratedNever();

				entity.Property(e => e.OwnerId).HasColumnName("OwnerID");

				entity.Property(e => e.Title)
					.IsRequired()
					.HasColumnType("nchar(128)");

				entity.HasOne(d => d.Owner)
					.WithMany(p => p.Exclusive)
					.HasForeignKey(d => d.OwnerId)
					.HasConstraintName("FK_Exclusive_Users");
			});

			modelBuilder.Entity<Gender>(entity =>
			{
				entity.ToTable("Genders", "Community");

				entity.Property(e => e.Id)
					.HasColumnName("ID")
					.HasColumnType("tinyint")
					.ValueGeneratedNever();

				entity.Property(e => e.Description)
					.IsRequired()
					.HasColumnType("nchar(256)");

				entity.Property(e => e.GenderName)
					.IsRequired()
					.HasColumnName("Gender")
					.HasColumnType("nchar(128)");
			});

			modelBuilder.Entity<MemeComment>(entity =>
			{
				entity.ToTable("MemeComments", "Community");

				entity.Property(e => e.Id)
					.HasColumnName("ID")
					.ValueGeneratedNever();

				entity.Property(e => e.Comment)
					.IsRequired()
					.HasColumnType("nchar(256)");

				entity.Property(e => e.MemeId).HasColumnName("MemeID");

				entity.Property(e => e.UserId).HasColumnName("UserID");

				entity.HasOne(d => d.Meme)
					.WithMany(p => p.MemeComments)
					.HasForeignKey(d => d.MemeId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_MemeComments_Memes");

				entity.HasOne(d => d.User)
					.WithMany(p => p.MemeComments)
					.HasForeignKey(d => d.UserId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_MemeComments_Users");
			});

			modelBuilder.Entity<Meme>(entity =>
			{
				entity.ToTable("Memes", "BGC");

				entity.Property(e => e.Id)
					.HasColumnName("ID")
					.ValueGeneratedNever();

				entity.Property(e => e.AddTime).HasColumnType("datetime");

				entity.Property(e => e.Title)
					.HasMaxLength(256)
					.IsUnicode(false);

				entity.Property(e => e.Base64)
					.IsRequired()
					.HasMaxLength(int.MaxValue)
					.IsUnicode(false);
			});

			modelBuilder.Entity<AspUser>(entity =>
			{
				entity.ToTable("Users", "dbo");

				entity.Property(e => e.Id)
					.ValueGeneratedNever();

				entity.Property(e => e.Respek)
					.HasColumnName("Respek").IsRequired();

				entity.Property(e => e.UserName)
					.HasColumnType("nvarchar(128)")
					.HasColumnName("Name").IsRequired();

				entity.Property(e => e.Motto)
					.HasColumnType("nvarchar(256)");

				entity.Property(e => e.Email)
					.HasColumnType("nvarchar(256)").IsRequired();

				entity.Property(e => e.DogeCoins).IsRequired();

				entity.HasOne(d => d.Gender)
					.WithMany(p => p.Users)
					.HasForeignKey(d => d.GenderId)
					.HasConstraintName("FK_Users_Genders");

			});
		}
	}
}
