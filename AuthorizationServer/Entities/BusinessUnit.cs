using System.ComponentModel.DataAnnotations;

namespace AuthorizationServer.Entities
{
    public class BusinessUnit
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string TenantCode { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
