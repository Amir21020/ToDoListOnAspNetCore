using Microsoft.AspNetCore.Mvc;
using ToDoList.Domain.ViewModels.Task;
using ToDoList.Service.Interfaces;

namespace ToDoListOnAspNetCore.Controllers;

public class TaskController
    (ITaskService taskService): Controller
{

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTaskViewModel model)
    {
        var response = await taskService.Create(model);
        if(response.StatusCode == ToDoList.Domain.Enum.StatusCode.Ok)
        {
            return Ok(new { description = response.Description });
        }
        return BadRequest(new { description = response.Description });
    }

    [HttpPost]
    public async Task<IActionResult> TaskHandler()
    {
        var response = await taskService.GetTasks();
        return Json(new {data = response.Data});
    }
}
