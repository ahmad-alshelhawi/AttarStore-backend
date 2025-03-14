using AttarStore.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AttarStore.Api.Controllers
{
    [ApiController]
    [Route("api/actionlogs")]
    public class ActionLogController : ControllerBase
    {
        private readonly IActionLogService _actionLogService;

        public ActionLogController(IActionLogService actionLogService)
        {
            _actionLogService = actionLogService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActionLogs()
        {
            var logs = await _actionLogService.GetAllActionLogsAsync();
            return Ok(logs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetActionLogById(int id)
        {
            var log = await _actionLogService.GetActionLogByIdAsync(id);
            if (log == null) return NotFound();
            return Ok(log);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetActionLogsByUserId(int userId)
        {
            var logs = await _actionLogService.GetActionLogsByUserIdAsync(userId);
            return Ok(logs);
        }

        [HttpPost]
        public async Task<IActionResult> LogAction([FromBody] ActionLog actionLog)
        {
            if (actionLog == null) return BadRequest();

            await _actionLogService.LogActionAsync(actionLog.UserId, actionLog.Action, actionLog.Description);
            return Ok();
        }
    }

}
