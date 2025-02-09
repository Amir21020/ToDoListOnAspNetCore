﻿using ToDoList.Domain.Enum;

namespace ToDoList.Domain.Filters.Task;

public sealed class TaskFilter
{
    public string? Name { get; set; }
    public Priority? Priority { get; set; } 
}
