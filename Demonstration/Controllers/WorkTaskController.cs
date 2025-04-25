using Demonstration.Models;
using Demonstration.Models.Entities;
using Demonstration.Services;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text.Json;
using Demonstration.Repository.Interfaces;

namespace Demonstration.Controllers
{
    public class WorkTaskController : Controller
    {
        private readonly IWorkTaskRepository _workTaskRepository;
        private readonly IEmailSender _emailSender;
        private readonly HttpClient _httpClient;
        private string _ApiUrl;

        public WorkTaskController(IEmailSender emailSender, HttpClient httpClient, IConfiguration config, IWorkTaskRepository workTaskRepository)
        {
            _workTaskRepository = workTaskRepository;
            _emailSender = emailSender;
            _httpClient = httpClient;
            _ApiUrl = config["ApiConfiguration"];
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

            await _workTaskRepository.AddTaskAsync(task);
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var tasks = await _workTaskRepository.GetAllTasksAsync();
            return View(tasks);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var task = await _workTaskRepository.GetTaskByIdAsync(id);

            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(WorkTask viewModel)
        {
            var task = await _workTaskRepository.GetTaskByIdAsync(viewModel.Id);

            if (task is not null)
            {
                task.Name = viewModel.Name;
                task.Description = viewModel.Description;
                task.MaximumParticipants = viewModel.MaximumParticipants;

                await _workTaskRepository.UpdateTaskAsync(task);
            }

            return RedirectToAction("List", "WorkTask");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(WorkTask viewModel)
        {
            var task = await _workTaskRepository.GetTaskByIdAsync(viewModel.Id);

            if (task is not null)
            {
                await _workTaskRepository.DeleteTaskAsync(task);
            }

            return RedirectToAction("List", "WorkTask");
        }

        [HttpGet]
        public async Task<IActionResult> DownloadFile(int taskId, string fileName)
        {
            string apiUrl = $"{_ApiUrl}download/{taskId}/{fileName}";
            var response = await _httpClient.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Find not found.";
                return NotFound();
            }

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(content);
            var signedUrl = json.RootElement.GetProperty("url").GetString();

            var fileResponse = await _httpClient.GetAsync(signedUrl);

            if (!fileResponse.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Error(s) from Firebase.";
                return NotFound();
            }

            var fileBytes = await fileResponse.Content.ReadAsByteArrayAsync();
            var contentType = fileResponse.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";

            return File(fileBytes, contentType, fileName);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var response = await _httpClient.GetAsync($"{_ApiUrl}{id}");

            List<string> fileNames = new();
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                fileNames = JsonConvert.DeserializeObject<List<string>>(responseString);
            }
            ViewBag.Files = fileNames;

            var unassignedEmployees = await _workTaskRepository.GetUnassignedEmployeesAsync(id);
            ViewBag.UnassignedEmployees = new SelectList(unassignedEmployees, "Id", "Name");

            var assignedEmployee = await _workTaskRepository.GetAssignedEmployeeAsync(id);
            ViewBag.AssignedEmployees = new SelectList(assignedEmployee, "Id", "Name");

            var task = await _workTaskRepository.GetTaskByIdAsync(id);

            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        [HttpPost]
        public async Task<IActionResult> AssignUser(Guid userId, int taskId)
        {
            var result = await _workTaskRepository.AssignUserToTaskAsync(userId, taskId);

            switch (result.Status)
            {
                case AssignStatus.TaskNotFound:
                case AssignStatus.UserNotFound:
                    return NotFound();

                case AssignStatus.ReachMax:
                    TempData["ErrorMessage"] = "The task has reach its maximum participants";
                    break;

                case AssignStatus.Failed:
                    TempData["ErrorMessage"] = "Unexpected error! Please try again later.";
                    break;

                case AssignStatus.Sucess:
                    BackgroundJob.Enqueue(() => SendEmail(result.UserEmail, result.TaskName));
                    TempData["SuccessMessage"] = "User assigned successfully.";
                    break;
            }

            return RedirectToAction("Detail", new { id = taskId });
        }

        public async Task<IActionResult> RemoveEmployeeFromTask(Guid userId, int taskId)
        {
            await _workTaskRepository.RemoveEmployeeFromTask(userId, taskId);
            return RedirectToAction("Detail", new { id = taskId });
        }

        public void SendEmail(string receiver, string taskName)
        {
            var subject = "Notification about being assigned a task" + taskName;
            var content = "The system would like to notify you that you have been assigned to " + taskName + " task." + " Please log in to the system for more details.";

            _emailSender.SendEmailAsync(receiver, subject, content);
        }
    }
}
