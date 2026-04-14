using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResourceServer.DatabaseContext;

namespace ResourceServer.Controllers
{
    [ApiController]
    [Route("resource")]
    public class ApplicationsController(ResourceDBContext context) : ControllerBase
    {
        private readonly ResourceDBContext _context = context;

        [HttpGet("applications")]
        public async Task<IActionResult> GetApplications([FromQuery] string[] codes)
        {
            if (codes.Length == 0)
            {
                return Ok(Array.Empty<ApplicationCatalogItemDto>());
            }

            var normalizedCodes = codes
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToList();

            var applications = await _context.Applications
                .Where(x => normalizedCodes.Contains(x.Code) && x.IsActive)
                .OrderBy(x => x.SortOrder)
                .Select(x => new ApplicationCatalogItemDto
                {
                    Id = x.Code,
                    Name = x.Name,
                    Description = x.Description,
                    LaunchUrl = x.LaunchUrl,
                    IconUrl = x.IconUrl,
                    Maintainer = x.Maintainer,
                    ContactEmail = x.ContactEmail
                })
                .ToListAsync();

            return Ok(applications);
        }
    }

    public class ApplicationCatalogItemDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string LaunchUrl { get; set; } = string.Empty;
        public string? IconUrl { get; set; }
        public string? Maintainer { get; set; }
        public string? ContactEmail { get; set; }
    }
}
