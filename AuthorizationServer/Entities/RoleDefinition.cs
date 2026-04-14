using System.ComponentModel.DataAnnotations;

namespace AuthorizationServer.Entities
{
    public class RoleDefinition
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string ScopeType { get; set; } = null!; // GLOBAL or BU_SCOPED
        public bool IsActive { get; set; } = true;
    }
}
