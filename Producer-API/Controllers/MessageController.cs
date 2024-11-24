using Microsoft.AspNetCore.Mvc;
using Producer_API.Services;
using System.Text.Json;

namespace Producer_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageController : Controller
    {
        private readonly ILogger<MessageController> _logger;
        private readonly ProducerService _producerService;


        public MessageController(ILogger<MessageController> logger, ProducerService producerService)
        {
            _logger = logger;
            _producerService = producerService;
        }

        [HttpPost]
        public async Task<IActionResult> UpdateInventory([FromBody] string message)
        {
            await _producerService.ProduceAsync("MessageQueue", message);
            return Ok("Message Received Successfully...");
        }
    }
}
