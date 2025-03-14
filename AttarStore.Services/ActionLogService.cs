using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Services
{
    public class ActionLogService : IActionLogService
    {
        private readonly IActionLogRepository _actionLogRepository;

        public ActionLogService(IActionLogRepository actionLogRepository)
        {
            _actionLogRepository = actionLogRepository;
        }

        public async Task<IEnumerable<ActionLog>> GetAllActionLogsAsync()
        {
            return await _actionLogRepository.GetAllActionLogsAsync();
        }

        public async Task<ActionLog> GetActionLogByIdAsync(int id)
        {
            return await _actionLogRepository.GetActionLogByIdAsync(id);
        }

        public async Task<IEnumerable<ActionLog>> GetActionLogsByUserIdAsync(int userId)
        {
            return await _actionLogRepository.GetActionLogsByUserIdAsync(userId);
        }

        public async Task LogActionAsync(int userId, string action, string description)
        {
            var actionLog = new ActionLog
            {
                UserId = userId,
                Action = action,
                Description = description,
                Timestamp = DateTime.UtcNow
            };

            await _actionLogRepository.AddActionLogAsync(actionLog);
        }
    }
}
