using System.ComponentModel.DataAnnotations;

namespace AuthServer.Roles;

public class CreateRoleRequest
{
    [Required]
    public string Name { get; set; }
    public string Description { get; set; }
}