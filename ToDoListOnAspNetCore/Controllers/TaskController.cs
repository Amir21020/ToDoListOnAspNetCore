using Microsoft.AspNetCore.Mvc;
using ToDoList.Domain.Filters.Task;
using ToDoList.Domain.Utlils;
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
    public async Task<IActionResult> TaskHandler(TaskFilter filter)
    {
        var start = Request.Form["start"].FirstOrDefault();
        var length = Request.Form["length"].FirstOrDefault();

        var pageSize = length != null ? Convert.ToInt32(length) : 0;
        var skip = start != null ? Convert.ToInt32(start) : 0;

        filter.Skip = skip;
        filter.PageSize = pageSize;

        var response = await taskService.GetTasks(filter);
        return Json(new { recordsFiltered = response.Total, recordsTotal = response.Total,  data = response.Data });
    }

    [HttpPost]
    public async Task<IActionResult> EndTask(long id)
    {
        var response = await taskService.EndTask(id);
        if(response.StatusCode == ToDoList.Domain.Enum.StatusCode.Ok)
        {
            return Ok(new { description = response.Description });
        }
        return BadRequest(new { description = response.Description });
    }

    [HttpPost]
    public async Task<IActionResult> CalculateCompletedTasks()
    {
        var response = await taskService.CalculateCompletedTasks();
        if(response.StatusCode == ToDoList.Domain.Enum.StatusCode.Ok)
        {
            var csvService = new CsvBaseService<IEnumerable<TaskViewModel>>();
            var uploadFile = csvService.UploadFile(response.Data);
            return File(uploadFile, "text/csv", $"Статистика за {DateTime.Now.ToLongDateString()}.csv");
        }
        return BadRequest(new {description = response.Description});
    }

    public async Task<IActionResult> GetCompletedTasks()
    {
        var result = await taskService.GetCompletedTasks();
        return Json( new{ data =  result.Data });
    }


}
