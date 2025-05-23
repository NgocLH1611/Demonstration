﻿namespace Demonstration.Models.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }

        public ICollection<EmployeeTask> EmployeeTasks { get; set; } = new List<EmployeeTask>();
    }
}
