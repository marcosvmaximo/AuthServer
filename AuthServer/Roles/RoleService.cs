namespace AuthServer.Roles;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _repository;

    public RoleService(IRoleRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<Role> Insert(CreateRoleRequest toRole)
    {
        var role = new Role()
        {
            Name = toRole.Name,
            Description = toRole.Description
        };
        
        await _repository.Insert(role);
        return role;
    }

    public async Task<IEnumerable<Role>> FindAll()
    {
        return await _repository.GetAll();
    }
}


public interface IRoleService
{
    Task<Role> Insert(CreateRoleRequest toRole);
    Task<IEnumerable<Role>> FindAll();
}