using Demonstration.Data;
using Demonstration.Models.Entities;
using Demonstration.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demonstration.Repository
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ApplicationDbContext _context;

        public RoleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<Role?> GetByIdAsync(int id)
        {
            return await _context.Roles.FindAsync(id);
        }

        public async Task AddAsync(Role role)
        {
            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();
        }
         
        public async Task DeleteAsync(int id)
        {
            var role = await _context.Roles.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (role is not null)
            {
                _context.Roles.Remove(role);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(Role role)
        {
            _context.Roles.Update(role);
            await _context.SaveChangesAsync();
        }

        public async Task DetailAsync(Role role)
        {
            await _context.Entry(role).Collection(r => r.Users).LoadAsync();
        }
    }
}
