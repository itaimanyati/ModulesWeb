using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Entities;
using ModulesWeb.Data;

namespace ModulesWeb.Controllers
{
    public class StudyHoursController : Controller
    {
        private readonly ModulesContext _context;

        public StudyHoursController(ModulesContext context)
        {
            _context = context;
        }

        // GET: StudyHours
        public async Task<IActionResult> Index()
        {
            if (Session.userId < 1)
                return RedirectToAction("Login", "Users");
            return View(await _context.StudyHours.ToListAsync());
        }

        // GET: StudyHours/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studyHours = await _context.StudyHours
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studyHours == null)
            {
                return NotFound();
            }

            return View(studyHours);
        }

        // GET: StudyHours/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: StudyHours/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ModuleId,Date,Hours")] StudyHours studyHours)
        {
            if (ModelState.IsValid)
            {
                _context.Add(studyHours);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(studyHours);
        }

        // GET: StudyHours/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studyHours = await _context.StudyHours.FindAsync(id);
            if (studyHours == null)
            {
                return NotFound();
            }
            return View(studyHours);
        }

        // POST: StudyHours/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ModuleId,Date,Hours")] StudyHours studyHours)
        {
            if (id != studyHours.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(studyHours);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudyHoursExists(studyHours.Id))
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
            return View(studyHours);
        }

        // GET: StudyHours/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studyHours = await _context.StudyHours
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studyHours == null)
            {
                return NotFound();
            }

            return View(studyHours);
        }

        // POST: StudyHours/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var studyHours = await _context.StudyHours.FindAsync(id);
            _context.StudyHours.Remove(studyHours);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudyHoursExists(int id)
        {
            return _context.StudyHours.Any(e => e.Id == id);
        }
    }
}
