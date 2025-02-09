using Microsoft.EntityFrameworkCore;
using ToDoList.DAL;

namespace ToDoListOnAspNetCore.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddData(this IServiceCollection services,
        IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(
            config.GetConnectionString("SqlServer")));
        return services;
    }
}
