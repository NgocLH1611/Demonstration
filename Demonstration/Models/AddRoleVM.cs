using Demonstration.Models.Entities;

namespace Demonstration.Models
{
    public class AddRoleVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
