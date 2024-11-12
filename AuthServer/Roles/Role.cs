using System.ComponentModel.DataAnnotations;

namespace AuthServer.Roles;

public class Role
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    public string Description { get; set; }
}