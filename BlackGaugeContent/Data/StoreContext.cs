using Memstore.Models;
using Microsoft.EntityFrameworkCore;

namespace Memstore.Data
{
	public class StoreContext : DbContext
	{
		public StoreContext(DbContextOptions<StoreContext> options) : base(options)
		{
		}
		public DbSet<GalleryMeme> Memes {get; set;}

	}
}
