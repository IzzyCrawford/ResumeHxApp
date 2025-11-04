using System.ComponentModel.DataAnnotations;

namespace ResumeHxApp.Server.Models;

public class CreateJobHistoryRequest
{
    [Required]
    [MaxLength(150)]
    public string CompanyName { get; set; } = string.Empty;

    [MaxLength(150)]
    public string Location { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string JobTitle { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string TechStack { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string Summary { get; set; } = string.Empty;

    [Required]
    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public List<string>? Responsibilities { get; set; }
}
