using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Memstore.Data;
using Memstore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Memstore.Controllers
{
	public class HomeGalleryController : Controller
	{
		private readonly StoreContext _context;
		private const int MAXFRESHRATING = 2;

		public HomeGalleryController(StoreContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			return View(await _context.Memes.Where(m => m.Rating > MAXFRESHRATING).OrderByDescending(m => m.AddTime).ToListAsync());
		}

		[HttpGet]
		public async Task<IActionResult> FreshMemes()
		{
			return View(await _context.Memes.Where(m => m.Rating <= MAXFRESHRATING).OrderByDescending(m => m.AddTime).ToListAsync());
		}

		[HttpGet]
		[AutoValidateAntiforgeryToken]
		public IActionResult AddMeme()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AddMeme([Bind("Title,Url")] GalleryMeme meme)
		{
			try
			{
				if (ModelState.IsValid)
				{
					if (meme.Url == null)
						ModelState.AddModelError("", "No information!");

					else if (await _context.Memes.AnyAsync(m => m.Url == meme.Url) == false)
					{
						meme.AddTime = DateTime.Now;
						_context.Memes.Add(meme);
						await _context.SaveChangesAsync();
						return RedirectToAction("FreshMemes");
					}
					else
					{
						ModelState.AddModelError("AddingExisting", "Meme already exists! Shame on you, reposter!");
					}
				}
			}
			catch (DbException e)
			{
				ModelState.AddModelError("", "Unable to save meme");
			}
			return View(meme);
		}


		public IActionResult Error()
		{
			return View();
		}
	}
}
