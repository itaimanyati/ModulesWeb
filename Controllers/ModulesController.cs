using AspNetCoreHero.ToastNotification.Abstractions;
using Entities;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModulesWeb.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModulesWeb.Controllers
{
    public class ModulesController : Controller
    {
        private readonly ModulesContext _context;
        private INotyfService _notyf;
        public ModulesController(ModulesContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
        }

        // GET: Modules
        public async Task<IActionResult> Index()
        {
            
            List<Module> modulesList = new List<Module>();

            if (Session.userId < 1)
            {
                _notyf.Warning("Login to proceed",3);
                return RedirectToAction("Login", "Users");
            }
              

            // return View(await _context.Modules.ToListAsync());
            var modules = await _context.Modules.Where(m=>m.UserId == Session.userId).ToListAsync();
            var semesters = await _context.Semesters.ToListAsync();
            var notyfToday = _context.Notifications.Where(n => n.Date.Day == DateTime.Now.Day).ToList();

            // Get number of weeks in current semester
            var numberOfWeeks = (from s in semesters
                                  where s.Id != 0
                                  orderby s.StartDate descending
                                  select s.NumberOfWeeks).FirstOrDefault();

           
            foreach (var item in modules)
            {
                var module = new Module();
                module.Id = item.Id;
                module.Name = item.Name;
                module.Credits = item.Credits;
                module.HoursPerWeek = item.HoursPerWeek;
                module.SelfStudyHours = (module.Credits * 10) / numberOfWeeks - module.HoursPerWeek;
                modulesList.Add(module);

                // Iterate through Notifications to find if there is a set schedule for current module
                foreach (var notyf in notyfToday)
                {
                    if(notyf.Id == module.Id)
                        _notyf.Information(module.Name + "<br> is scheduled for self study today");

                }
            
            }

            return View(modulesList);
        }

        // GET: Modules/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @module = await _context.Modules
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@module == null)
            {
                return NotFound();
            }

            return View(@module);
        }

        // GET: Modules/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Modules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,Name,Credits,HoursPerWeek")] Module @module)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@module);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(@module);
        }

        // GET: Modules/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @module = await _context.Modules.FindAsync(id);
            if (@module == null)
            {
                return NotFound();
            }
            return View(@module);
        }

        // POST: Modules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,Name,Credits,HoursPerWeek")] Module @module)
        {
            if (id != @module.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@module);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModuleExists(@module.Id))
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
            return View(@module);
        }

        // GET: Modules/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @module = await _context.Modules
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@module == null)
            {
                return NotFound();
            }

            return View(@module);
        }

        // POST: Modules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @module = await _context.Modules.FindAsync(id);
            _context.Modules.Remove(@module);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ModuleExists(int id)
        {
            return _context.Modules.Any(e => e.Id == id);
        }
    }
}
