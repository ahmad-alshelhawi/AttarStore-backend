using AttarStore.Services;
using Microsoft.EntityFrameworkCore;

namespace AttarStore.Services
{

    public class ActionLogRepository : IActionLogRepository
    {
        private readonly AppDbContext _dbContext;

        public ActionLogRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }
        public async Task<IEnumerable<ActionLog>> GetAllActionLogsAsync()
        {
            return await _dbContext.ActionLogs.Include(al => al.User).ToListAsync();
        }

        public async Task<ActionLog> GetActionLogByIdAsync(int id)
        {
            return await _dbContext.ActionLogs.Include(al => al.User).FirstOrDefaultAsync(al => al.Id == id);
        }

        public async Task<IEnumerable<ActionLog>> GetActionLogsByUserIdAsync(int userId)
        {
            return await _dbContext.ActionLogs.Where(al => al.UserId == userId).ToListAsync();
        }

        public async Task AddActionLogAsync(ActionLog actionLog)
        {
            await _dbContext.ActionLogs.AddAsync(actionLog);
            await _dbContext.SaveChangesAsync();
        }
    }
}