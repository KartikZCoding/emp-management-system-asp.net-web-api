using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AppDbContext _appDbContext;

        public NotificationRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<Notification?> GetByIdAsync(int id)
        {
            return await _appDbContext.Notifications
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<List<Notification>> GetByEmployeeIdAsync(int employeeId, bool unreadOnly = false)
        {
            var query = _appDbContext.Notifications
                .Where(n => n.EmployeeId == employeeId);

            if (unreadOnly)
            {
                query = query.Where(n => !n.IsRead);
            }

            return await query
                .OrderByDescending(n => n.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync(int employeeId)
        {
            return await _appDbContext.Notifications
                .CountAsync(n => n.EmployeeId == employeeId && !n.IsRead);
        }

        public async Task AddAsync(Notification notification)
        {
            await _appDbContext.Notifications.AddAsync(notification);
        }

        public async Task AddRangeAsync(List<Notification> notifications)
        {
            await _appDbContext.Notifications.AddRangeAsync(notifications);
        }

        public void Update(Notification notification)
        {
            _appDbContext.Notifications.Update(notification);
        }

        public void Delete(Notification notification)
        {
            _appDbContext.Notifications.Remove(notification);
        }
    }
}
