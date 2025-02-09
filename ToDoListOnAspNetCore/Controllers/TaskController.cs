using Microsoft.AspNetCore.Mvc;

namespace ToDoListOnAspNetCore.Controllers;

public class TaskController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
