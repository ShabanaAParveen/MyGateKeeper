using System.ComponentModel.DataAnnotations;

namespace AuthorizationServer.Entities
{
    public class UserAssignment
    {
        [Key]
        public int Id { get; set; }
        public string UserCode { get; set; } = null!;
        public string TenantCode { get; set; } = null!;
        public string? BusinessUnitCode { get; set; }
        public string RoleCode { get; set; } = null!;
    }
}
