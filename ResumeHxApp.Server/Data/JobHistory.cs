namespace ResumeHxApp.Server.Data;

public class JobHistory
{
    public int Id { get; set; }
    public int ResumeId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public string TechStack { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;

    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime? EndDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
