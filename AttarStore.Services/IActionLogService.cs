using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Services
{
    public interface IActionLogService
    {
        Task<IEnumerable<ActionLog>> GetAllActionLogsAsync();
        Task<ActionLog> GetActionLogByIdAsync(int id);
        Task<IEnumerable<ActionLog>> GetActionLogsByUserIdAsync(int userId);
        Task LogActionAsync(int userId, string action, string description);
    }
}
