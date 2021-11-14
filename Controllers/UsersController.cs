using AspNetCoreHero.ToastNotification.Abstractions;
using AuthenticationPlugin;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModulesWeb.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ModulesWeb.Controllers
{
    public class UsersController : Controller
    {
        private readonly ModulesContext _context;
        private readonly INotyfService _notyf;

        public UsersController(ModulesContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Username,Password")] User user)
        {
            if (ModelState.IsValid)
            {
                var hashedPassword = SecurePasswordHasherHelper.Hash(user.Password);
                user.Password = hashedPassword;
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login","Users");
            }
            return View(user);
        }

        //Login 
        public IActionResult Login()
        {
            return View();
        }

        // Login
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var isUserExist = _context.Users.Where(u => u.Username == username).FirstOrDefault();
            if (isUserExist != null)
            {
                if (SecurePasswordHasherHelper.Verify(password, isUserExist.Password))
                {
                    Session.userId = isUserExist.Id;
                    _notyf.Success("Login Successful", 3);
                    return RedirectToAction("Index", "Modules");
                }

                else
                {
                    _notyf.Error("Login Failed", 3);
                    return RedirectToAction("Login", "Users");
                }
            }

            else {
                _notyf.Error("User not found");
                return RedirectToAction("Login", "Users");
            }
            
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Username,Password")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
