using System.ComponentModel.DataAnnotations;

namespace AuthorizationServer.Entities
{
    public class Tenant
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? RegistrationNumber { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
