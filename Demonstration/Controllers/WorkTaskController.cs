using Demonstration.Data;
using Demonstration.Models;
using Demonstration.Models.Entities;
using Demonstration.Services;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Demonstration.Controllers
{
    public class WorkTaskController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        public WorkTaskController(ApplicationDbContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
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

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var tasks = await _context.Tasks.ToListAsync();
            return View(tasks);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
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

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var allEmployees = await _context.Users.ToListAsync();

            var assignedEmployeeIds = await _context.UserTasks
                .Where(ut => ut.TaskId == id)
                .Select(ut => ut.UserId)
                .ToListAsync();

            var unassignedEmployees = allEmployees
                .Where(e => !assignedEmployeeIds.Contains(e.Id))
            .ToList();

            ViewBag.TaskId = id;
            Console.Write(ViewBag.TaskId);
            ViewBag.UnassignedEmployees = new SelectList(unassignedEmployees, "Id", "Name");

            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        [HttpPost]
        public async Task<IActionResult> AssignUser(Guid userId, int taskId)
        {
            var task = await _context.Tasks.FindAsync(taskId);
            if (task == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            if (task.EnrolledParticipants >= task.MaximumParticipants)
            {
                TempData["ErrorMessage"] = "The task have reach maximum participants.";
                return RedirectToAction("Detail", new { id = taskId });
            }

            var existingAssignment = await _context.UserTasks
                .FirstOrDefaultAsync(et => et.TaskId == taskId && et.UserId == userId);

            if (existingAssignment == null)
            {
                var employeeTask = new EmployeeTask
                {
                    TaskId = taskId,
                    UserId = userId
                };

                _context.UserTasks.Add(employeeTask);
                task.EnrolledParticipants++;
                await _context.SaveChangesAsync();

                BackgroundJob.Enqueue(() => SendEmail(user.Email, task.Name));
            }

            return RedirectToAction("Detail", new { id = taskId });
        }

        public void SendEmail(string receiver, string taskName)
        {
            var subject = "Notification about being assigned a task" + taskName;
            var content = "The system would like to notify you that you have been assigned to " + taskName + "." + " Please log in to the system for more details.";

            _emailSender.SendEmailAsync(receiver, subject, content);
        }
    }
}
