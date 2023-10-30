//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;
////using Bitirme.Context;
//using Bitirme.Models;

//namespace Bitirme.Controllers
//{
//    public class UsersController : Controller
//    {
//        //private readonly ApplicationDbContext _context;

//        //public UsersController(ApplicationDbContext context)
//        //{
//        //    _context = context;
//        //}

//        // GET: Users
//        public async Task<IActionResult> Index()
//        {
//            return _context.Users != null ?
//                        View(await _context.Users.ToListAsync()) :
//                        Problem("Entity set 'ApplicationDbContext.Users'  is null.");
//        }

//        // GET: Users/Details/5
//        public async Task<IActionResult> Details(int? id)
//        {
//            if (id == null || _context.Users == null)
//            {
//                return NotFound();
//            }

//            var employee = await _context.Users
//                .FirstOrDefaultAsync(m => m.Id == id);
//            if (employee == null)
//            {
//                return NotFound();
//            }

//            return View(employee);
//        }

//        // GET: Users/Create
//        public IActionResult Create()
//        {
//            return View();
//        }

//        // POST: Users/Create
//        // To protect from overposting attacks, enable the specific properties you want to bind to.
//        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create([Bind("Id,Name,Email,Phone,Address,City,Password")] Models.Users employee)
//        {
//            if (ModelState.IsValid)
//            {
//                _context.Add(employee);
//                await _context.SaveChangesAsync();
//                return RedirectToAction(nameof(Index));
//            }
//            return View(employee);
//        }

//        // GET: Users/Edit/5
//        public async Task<IActionResult> Edit(int? id)
//        {
//            if (id == null || _context.Users == null)
//            {
//                return NotFound();
//            }

//            var employee = await _context.Users.FindAsync(id);
//            if (employee == null)
//            {
//                return NotFound();
//            }
//            return View(employee);
//        }

//        // POST: Users/Edit/5
//        // To protect from overposting attacks, enable the specific properties you want to bind to.
//        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Phone,Address,City,Password")] Models.Users employee)
//        {
//            if (id != employee.Id)
//            {
//                return NotFound();
//            }

//            if (ModelState.IsValid)
//            {
//                try
//                {
//                    _context.Update(employee);
//                    await _context.SaveChangesAsync();
//                }
//                catch (DbUpdateConcurrencyException)
//                {
//                    if (!EmployeeExists(employee.Id))
//                    {
//                        return NotFound();
//                    }
//                    else
//                    {
//                        throw;
//                    }
//                }
//                return RedirectToAction(nameof(Index));
//            }
//            return View(employee);
//        }

//        // GET: Users/Delete/5
//        public async Task<IActionResult> Delete(int? id)
//        {
//            if (id == null || _context.Users == null)
//            {
//                return NotFound();
//            }

//            var employee = await _context.Users
//                .FirstOrDefaultAsync(m => m.Id == id);
//            if (employee == null)
//            {
//                return NotFound();
//            }

//            return View(employee);
//        }

//        // POST: Users/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> DeleteConfirmed(int id)
//        {
//            if (_context.Users == null)
//            {
//                return Problem("Entity set 'ApplicationDbContext.Users'  is null.");
//            }
//            var employee = await _context.Users.FindAsync(id);
//            if (employee != null)
//            {
//                _context.Users.Remove(employee);
//            }

//            await _context.SaveChangesAsync();
//            return RedirectToAction(nameof(Index));
//        }

//        private bool EmployeeExists(int id)
//        {
//            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
//        }
//    }
//}
