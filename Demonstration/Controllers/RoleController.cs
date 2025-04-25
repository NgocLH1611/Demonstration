using Demonstration.Data;
using Demonstration.Models.Entities;
using Demonstration.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Demonstration.Repository.Interfaces;
using Demonstration.Repository;

namespace Demonstration.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleRepository _roleRepository;
        public RoleController(RoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddRoleVM viewModel)
        {
            var role = new Role { Name = viewModel.Name };
            await _roleRepository.AddAsync(role);
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var role = await _roleRepository.GetAllAsync();
            if (role == null) return NotFound();
            return RedirectToAction("List", "Role");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            return View(role);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Role viewModel)
        {
            var role = await _roleRepository.GetByIdAsync(viewModel.Id);

            if (role is not null)
            {
                role.Name = viewModel.Name;
                await _roleRepository.UpdateAsync(role);
            }

            return RedirectToAction("List", "Role");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Role viewModel)
        {
            await _roleRepository.DeleteAsync(viewModel.Id);
            return RedirectToAction("List", "Role");
        }

        [HttpGet]
        public async Task<IActionResult> Detail (int id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null) return NotFound();
            await _roleRepository.DetailAsync(role);

            return View(role);
        }
    }
}
