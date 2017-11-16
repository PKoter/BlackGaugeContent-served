using System;
using System.Linq;
using Bgc.Data;
using Bgc.Models;

namespace Testing
{
	public static class DbDataSeeder
	{
		public static BgcFullContext SeedMemes(this BgcFullContext context)
		{
			var random = new Random();
			var date = DateTime.Now;
			for (int i = 1; i <= EFInMemoryDbCreator.MemeCount; i++)
			{
				context.Memes.Add(new Meme()
				{
					AddTime = date,
					Base64 = Enumerable.Range(i+ 5, 20)
						.Select(n => n.ToString("x"))
						.Aggregate("", (a, b)=> a+ b),
					CommentCount = random.Next(0, 10),
					//Id = i,
					Rating= random.Next(-5, 5),
					Title = random.Next(0, 20) % 2 == 0? i.ToString() : null
				});
				//date = date.AddDays(-1);
				context.SaveChanges();
			}
			return context;
		}

		public static BgcFullContext SeedGenders(this BgcFullContext context, int count)
		{
			for (int i = 1; i <= count; i++)
			{
				context.Genders.Add(new Gender()
				{
					Id = (byte)i,
					Description = GetRandomText(i, 6),
					GenderName = GetRandomText(i, 4)
				});
				//date = date.AddDays(-1);
				context.SaveChanges();
			}
			return context;
		}

		public static BgcFullContext SeedUsers(this BgcFullContext context, int count)
		{
			context.SeedGenders(3);
			for (int i = 1; i <= count; i++)
			{
				context.Users.Add(new AspUser()
				{
					Id = i,
					EmailConfirmed = true,
					UserName = GetRandomText(i, 5),
					LockoutEnabled = false,
					AccessFailedCount = 0,
					PhoneNumberConfirmed = true,
					TwoFactorEnabled = false,
					Email = GetRandomText(i, 4)+"@gtest.com",
					GenderId = 1
				});
				//date = date.AddDays(-1);
				context.SaveChanges();
			}
			return context;
		}

		private static string GetRandomText(int i, int length)
		{
			return Enumerable.Range(i + 5, length)
				.Select(n => n.ToString("x"))
				.Aggregate("", (a, b) => a + b);
		}
	}
}