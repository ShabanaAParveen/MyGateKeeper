using System.ComponentModel.DataAnnotations;

namespace AuthorizationServer.Entities
{
    public class RoleApplicationControlGrant
    {
        [Key]
        public int Id { get; set; }
        public string RoleCode { get; set; } = null!;
        public string ApplicationCode { get; set; } = null!;
        public string ControlCode { get; set; } = null!;
        public bool IsActive { get; set; } = true;
    }
}
