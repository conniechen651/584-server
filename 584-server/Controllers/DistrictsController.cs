using Microsoft.AspNetCore.Mvc;

namespace _584_server;

public class DistrictsController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}