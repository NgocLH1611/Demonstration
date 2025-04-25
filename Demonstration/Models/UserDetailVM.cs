using Demonstration.Models.Entities;

namespace Demonstration.Models
{
    public class UserDetailVM
    {
        public User User { get; set; }
        public List<WorkTask> AssignedTasks { get; set; }
    }
}
