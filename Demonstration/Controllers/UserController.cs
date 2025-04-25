using Microsoft.AspNetCore.Mvc;
using Demonstration.Models;
using Demonstration.Data;
using Demonstration.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Serilog;
using System.Threading.Tasks;
using Demonstration.Repository.Interfaces;
using Demonstration.Repository;

namespace Demonstration.Controllers
{
    public class UserController : Controller
    {
        private readonly UserRepository _userRepository;
        public UserController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            ViewBag.Roles = new SelectList(await _userRepository.GetAllRolesAsync(), "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddUserVM viewModel)
        {
            var user = new User
            {
                Name = viewModel.Name,
                Email = viewModel.Email,
                Password = viewModel.Password,
                RoleId = viewModel.RoleId
            };

            await _userRepository.AddAsync(user);
            return RedirectToAction("List", "User");
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var users = await _userRepository.GetAllAsync();
            if (users == null) return NotFound();
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            ViewBag.Roles = new SelectList(await _userRepository.GetAllRolesAsync(), "Id", "Name", user.RoleId);
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(User viewModel)
        {
            var user = await _userRepository.GetByIdAsync(viewModel.Id);

            if (user is not null)
            {
                user.Name = viewModel.Name;
                user.Email = viewModel.Email;
                user.Password = viewModel.Password;
                user.RoleId = viewModel.RoleId;

                await _userRepository.UpdateAsync(user);
            }

            return RedirectToAction("List", "User");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(User viewModel)
        {
            await _userRepository.DeleteAsync(viewModel.Id);
            return RedirectToAction("List", "User");
        }

        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return NotFound();

            var tasks = await _userRepository.GetWorkTasksAsync(id);
            var viewModel = new UserDetailVM
            {
                User = user,
                AssignedTasks = tasks
            };

            return View(viewModel);
        }
    }
}
