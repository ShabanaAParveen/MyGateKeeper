namespace ResourceServer.Models
{
    public class Application
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string LaunchUrl { get; set; } = null!;
        public string? IconUrl { get; set; }
        public string? Maintainer { get; set; }
        public string? ContactEmail { get; set; }
        public bool IsActive { get; set; } = true;
        public int SortOrder { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime ModifiedAtUtc { get; set; }
    }
}
