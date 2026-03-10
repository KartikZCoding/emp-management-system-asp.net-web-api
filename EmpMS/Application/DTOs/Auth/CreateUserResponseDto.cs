using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Auth
{
    public class CreateUserResponseDto
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public int? EmployeeId { get; set; }
    }
}
