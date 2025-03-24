namespace Demonstration.Models
{
    public class AddTaskVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int MaximumParticipants { get; set; }
        public int EnrolledParticipants { get; set; }
    }
}
