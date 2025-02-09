using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Globalization;
using ToDoList.DAL.Interfaces;
using ToDoList.Domain.Enity;
using ToDoList.Domain.Extensions;
using ToDoList.Domain.Filters.Task;
using ToDoList.Domain.Response;
using ToDoList.Domain.ViewModels.Task;
using ToDoList.Service.Interfaces;

namespace ToDoList.Service.Implementations;

public sealed class TaskService
    (IBaseRepository<TaskEntity> taskRepository,
    ILogger<TaskService> logger)
    : ITaskService
{
    public async Task<IBaseResponse<IEnumerable<TaskViewModel>>> CalculateCompletedTasks()
    {
        try
        {
            var tasks = await taskRepository.GetAll()
                .Where(x => x.Created.Date == DateTime.Today)
                .Select(x => new TaskViewModel
                {
                    Name = x.Name,
                    Description = x.Description,
                    Priority = x.Priority.ToString(),
                    Created = x.Created.ToString(CultureInfo.InvariantCulture),
                    Id = x.Id,
                    IsDone = x.IsDone == true ? "Готова" : "Не готова"
                }).ToListAsync();

            return new BaseResponse<IEnumerable<TaskViewModel>>
            {
                Data = tasks,
                StatusCode = Domain.Enum.StatusCode.Ok
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[TaskService.CalculateCompletedTasks]: {ex.Message}");
            return new BaseResponse<IEnumerable<TaskViewModel>>
            {
                StatusCode = Domain.Enum.StatusCode.InternalServerError,
                Description = ex.Message
            };
        }
    }

    public async Task<IBaseResponse<TaskEntity>> Create(CreateTaskViewModel model)
    {
        try
        {
            model.Validate();
            logger.LogInformation($"Запрос на создание задачи - {model.Name}");
            var task = await taskRepository.GetAll()
                .Where(x => x.Created.Date == DateTime.Today)
                .FirstOrDefaultAsync(x => x.Name == model.Name);
            if(task != null)
            {
                return new BaseResponse<TaskEntity>
                {
                    Description = "Задача с таким названием уже есть",
                    StatusCode =  Domain.Enum.StatusCode.TaskIsHasAlready
                };
            }

            task = new TaskEntity()
            {
                Name = model.Name,
                Description = model.Description,
                IsDone = false,
                Priority = model.Priority,
                Created = DateTime.Now,
            };

            await taskRepository.Create(task);

            logger.LogInformation($"Задача создалась: {task.Name} {task.Created}");

            return new BaseResponse<TaskEntity>
            {
                Description = "Задача создалась",
                StatusCode = Domain.Enum.StatusCode.Ok,
            };
        }
        catch(Exception ex)
        {
            logger.LogError(ex,$"[TaskService.Create]: {ex.Message}");
            return new BaseResponse<TaskEntity>
            {
                StatusCode = Domain.Enum.StatusCode.InternalServerError,
                Description = ex.Message
            };
        }
    }

    public async Task<IBaseResponse<bool>> EndTask(long id)
    {
        try
        {
            var task = await taskRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
            if(task is null)
            {
                return new BaseResponse<bool>
                {
                    StatusCode = Domain.Enum.StatusCode.TaskNotFound,
                    Description = "Задача не найдена"
                };
            }

            task.IsDone = true;

            await taskRepository.Update(task);

            return new BaseResponse<bool>
            {
                Description = "Задача завершена",
                StatusCode = Domain.Enum.StatusCode.Ok
            };
        }
        catch(Exception ex )
        {
            logger.LogError(ex, $"[TaskService.EndTask]: {ex.Message}");
            return new BaseResponse<bool>
            {
                Description = $"{ex.Message }",
                StatusCode = Domain.Enum.StatusCode.InternalServerError
            };
        }
    }

    public async Task<IBaseResponse<IEnumerable<TaskCompletedViewModel>>> GetCompletedTasks()
    {
        try
        {
            var tasks = await taskRepository.GetAll()
                .Where(x => x.IsDone)
                .Where(x => x.Created.Date == DateTime.Today)
                .Select(x => new TaskCompletedViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description.Substring(0, 5),
                })
                .ToListAsync();

            return new BaseResponse<IEnumerable<TaskCompletedViewModel>>
            {
                Data = tasks,
                StatusCode = Domain.Enum.StatusCode.Ok
            };
        }
        catch(Exception ex)
        {
            logger.LogError(ex, $"[TaskService.GetCompletedTasks]: {ex.Message}");
            return new BaseResponse<IEnumerable<TaskCompletedViewModel>>
            {
                Description = $"{ex.Message}",
                StatusCode = Domain.Enum.StatusCode.InternalServerError,
            };
        }
    }

    public async Task<DataTableResult> GetTasks(TaskFilter filter)
    {
        try
        {
            var tasks = await taskRepository.GetAll()
                .Where(x => !x.IsDone)
                .WhereIf(!string.IsNullOrWhiteSpace(filter.Name), 
                x => x.Name == filter.Name)
                .WhereIf(filter.Priority.HasValue,x => x.Priority == filter.Priority)
                .Select(x => new TaskViewModel
                {
                    Name = x.Name,
                    Description = x.Description,
                    Priority = x.Priority.GetDisplayName(),
                    Created = x.Created.ToLongDateString(),
                    Id = x.Id,
                    IsDone = x.IsDone == true ? "Готова" : "Не готова"
                })
                .Skip(filter.Skip)
                .Take(filter.PageSize)
                .ToListAsync();

            var count = taskRepository.GetAll().Count(x => !x.IsDone);

            return new DataTableResult
            {
                Data = tasks,
                Total = count
            };
        }
        catch(Exception ex)
        {
            logger.LogError($"[TaskService.GetTasks]: {ex.Message}");
            return new DataTableResult
            {
                Data = null,
                Total = 0
            };
        }
    }
}
