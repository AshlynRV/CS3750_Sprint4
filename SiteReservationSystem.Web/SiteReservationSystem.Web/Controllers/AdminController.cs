using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiteReservationSystem.Web.Data;
using SiteReservationSystem.Web.Models;
using SiteReservationSystem.Web.Filters;
using SiteReservationSystem.Web.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SiteReservationSystem.Web.Controllers
{
    public class AdminController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        // GET: Admin dashboard - Admin only
        public IActionResult Index()
        {
            var redirect = RequireAdmin();
            if (redirect != null)
                return redirect;
            return View();
        }

        // GET: Admin/Employees - Admin OR ManageEmployees
        [Authorize(AccessPermissions.ManageEmployees)]
        public async Task<IActionResult> Employees()
        {
            var employees = await _context.Employees
                .Include(e => e.User)
                .ToListAsync();
            return View(employees);
        }

        // GET: Admin/CreateEmployee - Admin OR ManageEmployees
        [Authorize(AccessPermissions.ManageEmployees)]
        public IActionResult CreateEmployee()
        {
            var model = new EmployeeViewModel();
            return View(model);
        }

        // POST: Admin/CreateEmployee
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(AccessPermissions.ManageEmployees)]
        public async Task<IActionResult> CreateEmployee(EmployeeViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = new User
            {
                Email = model.Email,
                PasswordHash = model.Password,
                Role = UserRole.Employee,
                IsActive = true,
                DateCreated = DateTime.UtcNow
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var employee = new Employee
            {
                UserID = user.UserID,
                FirstName = model.FirstName,
                LastName = model.LastName,
                AccessPermissions = model.SelectedPermissions.Any()
                    ? model.SelectedPermissions.Aggregate(AccessPermissions.None, (acc, perm) => acc | perm)
                    : 0,
                IsLockedOut = false,
                DateHired = DateTime.UtcNow,
                IsActive = true
            };
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Employees));
        }

        // GET: Admin/EditEmployee/5 - Admin OR ManageEmployees
        [Authorize(AccessPermissions.ManageEmployees)]
        public async Task<IActionResult> EditEmployee(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.EmployeeID == id);

            if (employee == null) return NotFound();

            var model = new EmployeeViewModel
            {
                EmployeeID = employee.EmployeeID,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.User.Email,
                IsLockedOut = employee.IsLockedOut,
                SelectedPermissions = Enum.GetValues(typeof(AccessPermissions))
                    .Cast<AccessPermissions>()
                    .Where(p => p != AccessPermissions.None && (employee.AccessPermissions & p) == p)
                    .ToList()
            };

            return View(model);
        }

        // POST: Admin/EditEmployee/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(AccessPermissions.ManageEmployees)]
        public async Task<IActionResult> EditEmployee(EmployeeViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var employee = await _context.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.EmployeeID == model.EmployeeID);

            if (employee == null) return NotFound();

            employee.FirstName = model.FirstName;
            employee.LastName = model.LastName;
            employee.IsLockedOut = model.IsLockedOut;
            employee.User.Email = model.Email;

            employee.AccessPermissions = model.SelectedPermissions.Any()
                ? model.SelectedPermissions.Aggregate(AccessPermissions.None, (acc, perm) => acc | perm)
                : 0;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Employees));
        }

        // GET: Admin/DeleteEmployee/5 - Admin OR ManageEmployees
        [Authorize(AccessPermissions.ManageEmployees)]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.EmployeeID == id);

            if (employee == null) return NotFound();

            return View(employee);
        }

        // POST: Admin/DeleteEmployeeConfirmed/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(AccessPermissions.ManageEmployees)]
        public async Task<IActionResult> DeleteEmployeeConfirmed(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.EmployeeID == id);

            if (employee == null) return NotFound();

            if (employee.User != null)
                _context.Users.Remove(employee.User);

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Employees));
        }

        // POST: Admin/ToggleLock/5 - Admin OR ManageEmployees
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(AccessPermissions.ManageEmployees)]
        public async Task<IActionResult> ToggleLock(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.EmployeeID == id);

            if (employee == null) return NotFound();

            employee.IsLockedOut = !employee.IsLockedOut;
            employee.IsActive = !employee.IsLockedOut;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Employees));
        }
    }
}