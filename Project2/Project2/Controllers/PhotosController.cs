using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Project2.Models;
using Microsoft.AspNetCore.Identity;

namespace Project2.Controllers
{
    public class PhotosController : Controller
    {
        private readonly PhotoManagement _context;

        public PhotosController(PhotoManagement context)
        {
            _context = context;
        }

        // GET: Photos
        public async Task<IActionResult> Index()
        {
            return View(await _context.Photo.ToListAsync());
        }

        // GET: Photos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var photo = await _context.Photo
                .FirstOrDefaultAsync(m => m.PhotoId == id);
            if (photo == null)
            {
                return NotFound();
            }

            return View(photo);
        }

        // GET: Photos/Create
        public IActionResult Edit(int id = 0)
        {
            if (id == 0)
                return View(new Photo());
            else
                return View(_context.Photo.Find(id));
        }

        // POST: Photos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("PhotoId,UserId,Photo_Name,Photo_Format,Photo_Geolocation,Photo_Tags,Photo_CaptureDate")] Photo photo)
        {            
            if (ModelState.IsValid)
            {
                if (photo.PhotoId == 0)
                    _context.Add(photo);
                else
                    _context.Update(photo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(photo);


        }                

        // GET: Photos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var photo = await _context.Photo.FindAsync(id);
            _context.Photo.Remove(photo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Photos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var photo = await _context.Photo.FindAsync(id);
            _context.Photo.Remove(photo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PhotoExists(int id)
        {
            return _context.Photo.Any(e => e.PhotoId == id);
        }

        private string getUser()
        {
            return HttpContext.User.Identity.Name;
        }
    }
}
