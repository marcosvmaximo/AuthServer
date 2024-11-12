using Microsoft.EntityFrameworkCore;

namespace AuthServer.Users;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<User>> GetAll()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User?> GetById(int id)
    {
        return await _context.Users
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<User> Insert(User user)
    {
        await _context.AddAsync(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User> Update(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task Delete(User user)
    {
        _context.Remove(user);
        await _context.SaveChangesAsync();
    }

    public async Task<User?> FindByEmail(string userEmail)
    {
        return await _context.Users.Include(x => x.Roles).FirstOrDefaultAsync(x => x.Email == userEmail);
    }
}

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAll();
    Task<User?> GetById(int id);
    Task<User> Insert(User user);
    Task<User> Update(User user);
    Task Delete(User user);
    Task<User?> FindByEmail(string userEmail);
}