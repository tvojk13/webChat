using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebSockets;
using System.Net;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using webChat.Models;
using Microsoft.Extensions.Logging;

namespace webChat.Controllers
{
    [ApiController]
    [Route("api")]
    public class MessagesController : ControllerBase
    {
        private readonly DAL _dal;
        private readonly ILogger<MessagesController> _logger;
        private readonly HttpClient httpClient = new HttpClient();
        private static readonly List<WebSocket> connections = new List<WebSocket>();
        private static int messageNumber = 1;

        public MessagesController(DAL dal, ILogger<MessagesController> logger)
        {
            _dal = dal;
            _logger = logger;
        }

        [HttpPost("send")]
        public async Task<IActionResult> PostMessage(MessageModel message)
        {
            try
            {
                _logger.LogInformation("[SERVER] Received message to send: {MessageText}", message.Text);
                await _dal.AddMessageAsync(message);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SERVER] Error occurred while posting the message.");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Error occurred while sending the message.");
            }
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory(DateTime startDate, DateTime endDate)
        {
            try
            {
                _logger.LogInformation("[SERVER] Fetching message history between {StartDate} and {EndDate}.", startDate, endDate);
                var messages = await _dal.GetMessagesAsync(startDate, endDate);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching message history.");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Error occurred while fetching history.");
            }
        }

        [HttpGet("ws")]
        public async Task AcceptWebSocket()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using WebSocket ws = await HttpContext.WebSockets.AcceptWebSocketAsync();
                connections.Add(ws);
                _logger.LogInformation("[SERVER] WebSocket connection established. Total connections: {ConnectionCount}.", connections.Count);
                await ReceiveMessage(ws);
            }
            else
            {
                _logger.LogWarning("[SERVER] Invalid WebSocket request.");
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
        }

        private async Task ReceiveMessage(WebSocket socket)
        {
            byte[] buffer = new byte[1024 * 4];

            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    DateTime dateTime = DateTime.UtcNow;
                    string formattedMessage = $"{messageNumber} | {dateTime:yyyy-MM-dd HH:mm:ss}: {message}";
                    byte[] msgBuffer = Encoding.UTF8.GetBytes(formattedMessage);

                    _logger.LogInformation("[SERVER] Broadcasting message: {FormattedMessage}", formattedMessage);

                    foreach (var connection in connections)
                    {
                        if (connection.State == WebSocketState.Open)
                        {
                            await connection.SendAsync(new ArraySegment<byte>(msgBuffer), WebSocketMessageType.Text, result.EndOfMessage, CancellationToken.None);
                        }
                    }

                    MessageModel messageToHttps = new MessageModel
                    {
                        Text = message,
                        Timestamp = dateTime,
                    };

                    string jsonContent = JsonSerializer.Serialize(messageToHttps);
                    StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    
                    await httpClient.PostAsync("http://localhost:7028/api/send", content);

                    messageNumber++;
                }
                else if (result.MessageType == WebSocketMessageType.Close || socket.State == WebSocketState.Aborted)
                {
                    connections.Remove(socket);
                    _logger.LogInformation("[SERVER] WebSocket connection closed. Total connections: {ConnectionCount}.", connections.Count);
                    await socket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
                }
            }
        }
    }
}
