using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface INotificationRepository
    {
        Task<Notification?> GetByIdAsync(int id);
        Task<List<Notification>> GetByEmployeeIdAsync(int employeeId, bool unreadOnly = false);
        Task<int> GetUnreadCountAsync(int employeeId);
        Task AddAsync(Notification notification);
        Task AddRangeAsync(List<Notification> notifications);
        void Update(Notification notification);
        void Delete(Notification notification);
    }
}
