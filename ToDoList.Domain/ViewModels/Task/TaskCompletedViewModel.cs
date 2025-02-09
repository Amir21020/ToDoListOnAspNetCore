using System.ComponentModel.DataAnnotations;

namespace ToDoList.Domain.ViewModels.Task;

public sealed class TaskCompletedViewModel
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}
