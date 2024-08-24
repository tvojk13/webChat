using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using webClientGetHistoryFromDB.Models;

namespace webClientGetHistoryFromDB.Controllers
{
    public class GetHistoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
