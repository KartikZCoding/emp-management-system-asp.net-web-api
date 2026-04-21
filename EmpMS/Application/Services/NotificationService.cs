using Application.DTOs.Notification;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<NotificationService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public NotificationService(
            INotificationRepository notificationRepository,
            IEmployeeRepository employeeRepository,
            IMapper mapper,
            ILogger<NotificationService> logger,
            IUnitOfWork unitOfWork)
        {
            _notificationRepository = notificationRepository;
            _employeeRepository = employeeRepository;
            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<NotificationResultDto> GetMyNotificationsAsync(string email, bool unreadOnly)
        {
            var employee = await _employeeRepository.GetByEmailAsync(email);
            if (employee == null) throw new NotFoundException("Employee profile not found!");

            var notifications = await _notificationRepository.GetByEmployeeIdAsync(employee.Id, unreadOnly);
            var unreadCount = await _notificationRepository.GetUnreadCountAsync(employee.Id);

            return new NotificationResultDto
            {
                Notifications = _mapper.Map<List<NotificationResponseDto>>(notifications),
                UnreadCount = unreadCount
            };
        }

        public async Task MarkAsReadAsync(int notificationId, string email)
        {
            var employee = await _employeeRepository.GetByEmailAsync(email);
            if (employee == null) throw new NotFoundException("Employee profile not found!");

            var notification = await _notificationRepository.GetByIdAsync(notificationId);
            if (notification == null) throw new NotFoundException("Notification not found!");

            if (notification.EmployeeId != employee.Id)
                throw new UnauthorizedException("You are not authorized to access this notification!");

            if (!notification.IsRead)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.Now;
                _notificationRepository.Update(notification);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<int> MarkAllAsReadAsync(string email)
        {
            var employee = await _employeeRepository.GetByEmailAsync(email);
            if (employee == null) throw new NotFoundException("Employee profile not found!");

            var unreadNotifications = await _notificationRepository.GetByEmployeeIdAsync(employee.Id, unreadOnly: true);
            
            if (unreadNotifications.Count > 0)
            {
                foreach (var notif in unreadNotifications)
                {
                    notif.IsRead = true;
                    notif.ReadAt = DateTime.Now;
                    _notificationRepository.Update(notif);
                }
                await _unitOfWork.SaveChangesAsync();
            }

            return unreadNotifications.Count;
        }

        public async Task DeleteNotificationAsync(int notificationId, string email)
        {
            var employee = await _employeeRepository.GetByEmailAsync(email);
            if (employee == null) throw new NotFoundException("Employee profile not found!");

            var notification = await _notificationRepository.GetByIdAsync(notificationId);
            if (notification == null) throw new NotFoundException("Notification not found!");

            if (notification.EmployeeId != employee.Id)
                throw new UnauthorizedException("You are not authorized to delete this notification!");

            _notificationRepository.Delete(notification);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> BroadcastAsync(BroadcastNotificationDto dto, string createdBy)
        {
            var employees = await _employeeRepository.GetAllAsync(1, int.MaxValue, null, null);
            var activeEmployees = employees.Where(e => e.IsActive).ToList();

            var notifications = new List<Notification>();

            foreach (var emp in activeEmployees)
            {
                notifications.Add(new Notification
                {
                    EmployeeId = emp.Id,
                    Title = dto.Title,
                    Message = dto.Message,
                    Type = "Broadcast",
                    CreatedAt = DateTime.Now,
                    CreatedBy = createdBy
                });
            }

            if (notifications.Count > 0)
            {
                await _notificationRepository.AddRangeAsync(notifications);
                await _unitOfWork.SaveChangesAsync();
            }

            _logger.LogInformation("Broadcast sent to {Count} active employees by {User}", notifications.Count, createdBy);

            return notifications.Count;
        }
    }
}
