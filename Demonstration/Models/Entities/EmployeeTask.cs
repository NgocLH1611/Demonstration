namespace Demonstration.Models.Entities
{
    public class EmployeeTask
    {
        public Guid UserId { get; set; }
        public User User { get; set; }

        public int TaskId { get; set; }
        public WorkTask Task { get; set; }
    }
}
