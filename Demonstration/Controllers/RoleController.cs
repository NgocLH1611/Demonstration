using Demonstration.Data;
using Demonstration.Models.Entities;
using Demonstration.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Demonstration.Controllers
{
    public class RoleController : Controller
    {
        private readonly ApplicationDbContext _context;
        public RoleController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddUserVM viewModel)
        {
            var role = new Role
            {
                Name = viewModel.Name
            };

            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var roles = await _context.Roles.ToListAsync();
            return View(roles);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var role = await _context.Roles.FindAsync(id);

            if (role is null)
            {
                return NotFound();
            }

            return View(role);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Role viewModel)
        {
            var role = await _context.Roles.FindAsync(viewModel.Id);

            if (role is not null)
            {
                role.Name = viewModel.Name;

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("List", "Role");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Role viewModel)
        {
            var role = await _context.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == viewModel.Id);

            if (role is not null)
            {
                _context.Roles.Remove(viewModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("List", "Role");
        }
    }
}
