using AuthServer.Users;
using Microsoft.EntityFrameworkCore;

namespace AuthServer.Roles;

public class RoleRepository : IRoleRepository
{
    private readonly AppDbContext _context;

    public RoleRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<User>> GetByRole(string role)
    {
        return _context.Users.Where(x => x.Roles.Any(r => r.Name == role)).ToList();
    }

    public async Task<Role?> GetByName(string roleName)
    {
        return await _context.Roles.FirstOrDefaultAsync(x => x.Name == roleName.ToUpper());
    }

    public async Task<IEnumerable<Role>> GetAll()
    {
        return await _context.Roles.ToListAsync();
    }

    public async Task<Role> Insert(Role role)
    {
        role.Name = role.Name.ToUpper();
        _context.Add(role);
        await _context.SaveChangesAsync();
        return role;
    }
}

public interface IRoleRepository
{
    Task<List<User>> GetByRole(string role);
    Task<Role?> GetByName(string roleName);
    Task<IEnumerable<Role>> GetAll();
    Task<Role> Insert(Role role);
}