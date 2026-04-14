using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AuthServer.Entity
{
    public class UserAccount
    {
        [Key]
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string PwdHash { get; set; } = null!;
        public bool IsActive { get; set; } = true;
    }
}
