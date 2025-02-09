using ToDoList.Domain.Enum;

namespace ToDoList.Domain.Enity;

public sealed class TaskEntity
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Priority Priority { get; set; }
}
