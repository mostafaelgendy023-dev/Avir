using Avir1.Data;
using Avir1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Avir1.Controllers
{
    [Route("users/{userId}/chats")]
    [ApiController]
    public class ChatbotController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ChatbotController(AppDbContext context)
        {
            _context = context;
        }

        // GET /users/{userId}/chats
        [HttpGet]
        public async Task<IActionResult> GetAll(int userId)
        {
            return Ok(await _context.ChatbotInteractions
                .Where(x => x.UserID == userId)
                .OrderBy(x => x.Timestamp)
                .ToListAsync());
        }

        // GET /users/{userId}/chats/session/{sessionId}
        [HttpGet("session/{sessionId}")]
        public async Task<IActionResult> GetSession(int userId, string sessionId)
        {
            var messages = await _context.ChatbotInteractions
                .Where(x => x.UserID == userId && x.SessionId == sessionId)
                .OrderBy(x => x.Timestamp)
                .ToListAsync();

            if (!messages.Any())
                return NotFound("Session not found.");

            return Ok(messages);
        }

        // POST /users/{userId}/chats
        [HttpPost]
        public async Task<IActionResult> SendMessage(int userId, ChatbotInteraction model)
        {
            try
            {
                model.UserID = userId;
                model.Timestamp = DateTime.Now;

                if (string.IsNullOrEmpty(model.SessionId))
                    model.SessionId = Guid.NewGuid().ToString();

                _context.ChatbotInteractions.Add(model);
                await _context.SaveChangesAsync();

                var botReply = GenerateBotReply(model.Message, userId);

                var botMessage = new ChatbotInteraction
                {
                    UserID = userId,
                    SessionId = model.SessionId,
                    Role = "bot",
                    Message = botReply,
                    Timestamp = DateTime.Now
                };

                _context.ChatbotInteractions.Add(botMessage);
                await _context.SaveChangesAsync();

                return Ok(new { userMessage = model, botMessage });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        // DELETE /users/{userId}/chats/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _context.ChatbotInteractions.FindAsync(id);
            if (existing == null) return NotFound();

            _context.ChatbotInteractions.Remove(existing);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // ---- Triage ----

        // GET /users/{userId}/chats/triage
        [HttpGet("triage")]
        public async Task<IActionResult> GetTriageResults(int userId)
        {
            return Ok(await _context.TriageDecisions
                .Where(x => x.UserID == userId)
                .ToListAsync());
        }

        // POST /users/{userId}/chats/triage
        [HttpPost("triage")]
        public async Task<IActionResult> EvaluateTriage(int userId, TriageDecision model)
        {
            model.UserID = userId;
            model.CreatedAt = DateTime.Now;

            // Risk scoring
            int score = 0;
            if (model.Age >= 40) score += 2;
            if (model.Age >= 50) score += 1;
            if (model.FamilyHistory) score += 3;
            if (model.BMI.HasValue && model.BMI > 30) score += 1;

            var symptoms = model.Symptoms?.Split(',') ?? Array.Empty<string>();
            var highRiskKeywords = new[] { "كتلة", "lump", "إفرازات", "discharge", "تغير", "ألم شديد" };
            foreach (var symptom in symptoms)
                if (highRiskKeywords.Any(k => symptom.Contains(k, StringComparison.OrdinalIgnoreCase)))
                    score += 2;

            model.RiskLevel = score switch
            {
                <= 2 => "Low",
                <= 5 => "Medium",
                _ => "High"
            };

            model.Recommendation = model.RiskLevel switch
            {
                "Low" => "لا يوجد ما يدعو للقلق. استمري في الفحص الذاتي الشهري وزوري طبيبك للفحص الدوري.",
                "Medium" => "يُنصح بزيارة طبيب متخصص لإجراء فحص سريري قريباً.",
                "High" => "⚠️ يُوصى بشدة بزيارة طبيب متخصص في أقرب وقت ممكن.",
                _ => ""
            };

            _context.TriageDecisions.Add(model);
            await _context.SaveChangesAsync();
            return Ok(model);
        }

        // ---- Helper ----
        private string GenerateBotReply(string userMessage, int userId)
        {
            if (string.IsNullOrWhiteSpace(userMessage))
                return "كيف يمكنني مساعدتك؟";

            var msg = userMessage.ToLower();

            if (msg.Contains("كتلة") || msg.Contains("lump"))
                return "لاحظتِ كتلة — هذا يستدعي الانتباه. أنصحك بإكمال تقييم الـ triage الآن.";

            if (msg.Contains("ألم") || msg.Contains("pain"))
                return "الألم قد يكون من أعراض عدة حالات. هل يمكنك وصف مكان الألم ومدته؟";

            if (msg.Contains("فحص") || msg.Contains("exam"))
                return "الفحص الذاتي الشهري مهم جداً. هل تريدين الاطلاع على خطوات الفحص الصحيحة؟";

            return "شكراً على مشاركتك. هل تودين البدء بتقييم الأعراض؟";
        }
    }
}