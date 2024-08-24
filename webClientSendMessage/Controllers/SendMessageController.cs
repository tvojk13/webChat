using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using webClientSendMessage.Models;

namespace webClientSendMessage.Controllers
{
    public class SendMessageController : Controller
    {
        public async Task<IActionResult> IndexAsync()
        {
            return View();
        }
    }
}
