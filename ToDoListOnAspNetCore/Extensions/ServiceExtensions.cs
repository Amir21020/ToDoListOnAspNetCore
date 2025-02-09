using Microsoft.EntityFrameworkCore;
using ToDoList.DAL;
using ToDoList.DAL.Interfaces;
using ToDoList.DAL.Repositories;
using ToDoList.Domain.Enity;
using ToDoList.Service.Implementations;
using ToDoList.Service.Interfaces;

namespace ToDoListOnAspNetCore.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddData(this IServiceCollection services,
        IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(
            config.GetConnectionString("SqlServer"), b => b.MigrationsAssembly("ToDoListOnAspNetCore")));
        services.AddScoped<IBaseRepository<TaskEntity>, TaskRepository>();
        services.AddScoped<ITaskService, TaskService>();
        
        return services;
    }
}
