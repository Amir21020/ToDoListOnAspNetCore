using ToDoList.DAL.Interfaces;
using ToDoList.Domain.Enity;

namespace ToDoList.DAL.Repositories;

public sealed class TaskRepository(AppDbContext context)
    : IBaseRepository<TaskEntity>
{
    public async Task Create(TaskEntity entity)
    {
        await context.Tasks.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task Delete(TaskEntity entity)
    {
        context.Tasks.Remove(entity);
        await context.SaveChangesAsync();
    }

    public IQueryable<TaskEntity> GetAll()
    {
        return context.Tasks;
    }

    public async Task<TaskEntity> Update(TaskEntity entity)
    {
        context.Tasks.Update(entity);
        await context.SaveChangesAsync();
        return entity;
    }
}
