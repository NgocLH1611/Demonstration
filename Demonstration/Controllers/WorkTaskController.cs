using Demonstration.Data;
using Demonstration.Models;
using Demonstration.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Demonstration.Controllers
{
    public class WorkTaskController : Controller
    {
        private readonly ApplicationDbContext _context;
        public WorkTaskController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddTaskVM viewModel)
        {
            var task = new WorkTask
            {
                Name = viewModel.Name,
                Description = viewModel.Description,
                MaximumParticipants = viewModel.MaximumParticipants
            };

            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();

            return RedirectToAction("List", "Task");
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var tasks = await _context.Tasks.ToListAsync();
            return View(tasks);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, int taskId)
        {
            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(WorkTask viewModel)
        {
            var task = await _context.Tasks.FindAsync(viewModel.Id);

            if (task is not null)
            {
                task.Name = viewModel.Name;
                task.Description = viewModel.Description;
                task.MaximumParticipants = viewModel.MaximumParticipants;

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("List", "Task");
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id, int taskId)
        {
            var allEmployees = await _context.Users.ToListAsync();

            var assignedEmployeeIds = await _context.UserTasks
                .Where(ut => ut.TaskId == taskId)
                .Select(ut => ut.UserId)
                .ToListAsync();

            var unassignedEmployees = allEmployees
                .Where(e => !assignedEmployeeIds.Contains(e.Id))
            .ToList();

            ViewBag.TaskId = taskId;
            ViewBag.UnassignedEmployees = new SelectList(unassignedEmployees, "Id", "Name");

            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(WorkTask viewModel)
        {
            var task = await _context.Tasks
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == viewModel.Id);

            if (task is not null)
            {
                _context.Tasks.Remove(viewModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("List", "Task");
        }
    }
}
