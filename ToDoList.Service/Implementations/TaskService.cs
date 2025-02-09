using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ToDoList.DAL.Interfaces;
using ToDoList.Domain.Enity;
using ToDoList.Domain.Response;
using ToDoList.Domain.ViewModels.Task;
using ToDoList.Service.Interfaces;

namespace ToDoList.Service.Implementations;

public sealed class TaskService
    (IBaseRepository<TaskEntity> taskRepository,
    ILogger<TaskService> logger)
    : ITaskService
{
    public async Task<IBaseResponse<TaskEntity>> Create(CreateTaskViewModel model)
    {
        try
        {
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

            };
        }
    }
}
