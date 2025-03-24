namespace Demonstration.Models.Entities
{
    public class WorkTask
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int MaximumParticipants { get; set; }
        public int EnrolledParticipants { get; set; }

        public ICollection<EmployeeTask> EmployeeTasks { get; set; } = new List<EmployeeTask>();
    }
}
