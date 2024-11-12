
using AuthServer.Errors;
using AuthServer.Roles;
using AuthServer.Security;
using AuthServer.Users.Requests;
using AuthServer.Users.Responses;

namespace AuthServer.Users;
public class UserService : IUserService
{
        private readonly IUserRepository _repository;
        private readonly IRoleRepository _roleRepository;
        private readonly IJwtService _jwt;
        private readonly ILogger<UserService> _log;

        public UserService(IUserRepository repository, IRoleRepository roleRepository, IJwtService jwt, ILogger<UserService> log)
        {
            _repository = repository;
            _roleRepository = roleRepository;
            _jwt = jwt;
            _log = log;
        }

        public async Task<User> Insert(CreateUserRequest request)
        {
            if (await _repository.FindByEmail(request.Email) != null)
                throw new BadRequestException($"Usuário com email {request.Email} já existe!");

            var user = new User(request);
            return await _repository.Insert(user);
        }

        public async Task<User> Insert(User user)
        {
            if (await _repository.FindByEmail(user.Email) != null)
                throw new BadRequestException($"Usuário com email {user.Email} já existe!");

            return await _repository.Insert(user);
        }

        public async Task<List<User>> List(SortDir? sortDir, string? role = null)
        {
            if (!string.IsNullOrEmpty(role))
            {
                role = role.ToUpper();
                return await _roleRepository.GetByRole(role);
            }

            return sortDir == SortDir.ASC 
                ? _repository.GetAll().Result.ToList()
                : _repository.GetAll().Result.OrderByDescending(u => u.Id).ToList();
        }

        public async Task<User?> FindByIdOrNull(int id)
        {
            return await _repository.GetById(id);
        }

        public async Task<User> Delete(int id)
        {
            var user = await _repository.GetById(id);
            if (user == null) return null;

            if (user.Roles.Any(r => r.Name == "ADMIN") && _roleRepository.GetByRole("ADMIN").Result.Count == 1)
            {
                throw new BadRequestException("Não é possível excluir o último administrador!");
            }

            await _repository.Delete(user);
            return user;
        }

        public async Task<User> Update(int id, string name)
        {
            var user = await _repository.GetById(id);
            if (user == null)
                throw new NotFoundException($"Usuário {id} não encontrado!");

            if (user.Name == name) return null;

            user.Name = name;
            return await _repository.Update(user);
        }

        public async Task<bool> AddRole(int id, string roleName)
        {
            var user = await _repository.GetById(id);
            if (user == null)
                throw new NotFoundException("Usuário não encontrado");

            if (user.Roles != null && user.Roles.Any(r => r.Name == roleName)) return false;

            var role = await _roleRepository.GetByName(roleName);
            if (role == null)
                throw new BadRequestException("Invalid role name!");

            user.Roles!.Add(role);
            await _repository.Update(user);
            return true;
        }

        public async Task<LoginResponse> Login(string email, string password)
        {
            var user = await _repository.FindByEmail(email);
            if (user == null || user.Password != password) return null;

            _log.LogInformation("User logged in. id={0} name={1}", user.Id, user.Name);
            return new LoginResponse
            {
                Token = _jwt.CreateToken(user),
                User = new UserResponse(user)
            };
        }
}

public interface IUserService
{
    Task<User> Insert(CreateUserRequest user);
    Task<User> Insert(User user);
    Task<List<User>> List(SortDir? sortDir, string? role = null);
    Task<User?> FindByIdOrNull(int id);
    Task<User> Delete(int id);
    Task<User> Update(int id, string name);
    Task<bool> AddRole(int id, string roleName);
    Task<LoginResponse> Login(string email, string password);
}