using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopingAPI.DataLayer.Data;
using ShopingAPI.DataLayer.Models;

namespace ShopingAPI.WebApp.Controllers
{
    public class UserAddressesController : Controller
    {
        private readonly DataContext _context;

        public UserAddressesController(DataContext context)
        {
            _context = context;
        }

        // GET: UserAddresses
        public async Task<IActionResult> Index()
        {
            var dataContext = _context.UserAddresses.Include(u => u.User);
            return View(await dataContext.ToListAsync());
        }

        // GET: UserAddresses/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userAddress = await _context.UserAddresses
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userAddress == null)
            {
                return NotFound();
            }

            return View(userAddress);
        }

        // GET: UserAddresses/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: UserAddresses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,FullName,MobileNumber,Pincode,Address1,Address2,Landmark,City,Country,Id,IP,IsActive,CreatedDateTime,UpdatedDateTime,ModifyBy")] UserAddress userAddress)
        {
            string ipAddress = "";
            if (Dns.GetHostAddresses(Dns.GetHostName()).Length > 0)
            {
                ipAddress = Dns.GetHostAddresses(Dns.GetHostName())[0].ToString();
            }
            userAddress.IP = ipAddress;
            userAddress.Id = Guid.NewGuid().ToString();
            userAddress.UpdatedDateTime = DateTime.Now;
            userAddress.CreatedDateTime = DateTime.Now;
            if (ModelState.IsValid)
            {
                _context.Add(userAddress);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", userAddress.UserId);
            return View(userAddress);
        }

        // GET: UserAddresses/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userAddress = await _context.UserAddresses.FindAsync(id);
            if (userAddress == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", userAddress.UserId);
            return View(userAddress);
        }

        // POST: UserAddresses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("UserId,FullName,MobileNumber,Pincode,Address1,Address2,Landmark,City,Country,Id,IP,IsActive,CreatedDateTime,UpdatedDateTime,ModifyBy")] UserAddress userAddress)
        {
            if (id != userAddress.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userAddress);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserAddressExists(userAddress.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", userAddress.UserId);
            return View(userAddress);
        }

        // GET: UserAddresses/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userAddress = await _context.UserAddresses
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userAddress == null)
            {
                return NotFound();
            }

            return View(userAddress);
        }

        // POST: UserAddresses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var userAddress = await _context.UserAddresses.FindAsync(id);
            _context.UserAddresses.Remove(userAddress);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserAddressExists(string id)
        {
            return _context.UserAddresses.Any(e => e.Id == id);
        }
    }
}
