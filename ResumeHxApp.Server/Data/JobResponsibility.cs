namespace ResumeHxApp.Server.Data;

public class JobResponsibility
{
    public int Id { get; set; }
    public int JobHistoryId { get; set; }
    public string Description { get; set; } = string.Empty;
}