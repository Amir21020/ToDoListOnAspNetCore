using Microsoft.EntityFrameworkCore;
using ToDoList.Domain.Enity;

namespace ToDoList.DAL;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : 
    DbContext(options)
{
    public DbSet<TaskEntity> Tasks { get; set; }
}
