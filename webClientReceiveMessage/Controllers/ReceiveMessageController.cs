using Microsoft.AspNetCore.Mvc;
using System.Net.Sockets;
using System.Net.WebSockets;
namespace webClientReceiveMessage.Controllers
{
    public class ReceiveMessageController : Controller
    {
        public async Task<IActionResult> IndexAsync()
        {
            return View();
        }
    }
}
