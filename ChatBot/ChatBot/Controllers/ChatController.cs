using ChatBot.Services;
using Microsoft.AspNetCore.Mvc;
using ChatBot.Models;

namespace ChatBot.Controllers
{
    public class ChatController : Controller
    {
        private readonly OpenAIService _aiService;

        public ChatController(OpenAIService aiService)
        {
            _aiService = aiService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new ChatViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Index(ChatViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.UserMessage))
                return View(model);

            model.AIResponse = await _aiService.GetAIResponseAsync(model.UserMessage);

            return View(model);
        
    }
    }
}
