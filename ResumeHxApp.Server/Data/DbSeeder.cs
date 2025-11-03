using ResumeHxApp.Server.Data;

namespace ResumeHxApp.Server.Data;

public static class DbSeeder
{
    public static void SeedDatabase(ApplicationDbContext context)
    {
        // Check if we already have data
        if (context.Resumes.Any())
        {
            return; // Database has been seeded
        }

        // Add sample resumes
        var resumes = new List<Resume>
        {
            new Resume
            {
                FullName = "Israel Crawford",
                Email = "israel.crawford2011@gmail.com",
                Phone = "(801) 309-6983",
                LinkedInProfile = "https://linkedin.com/in/israel-crawford-b9874960",
                Summary = "Experienced Full-Stack Developer with 10+ years building scalable web applications. Proficient in React, .NET Core, and PostgreSQL. Strong focus on clean code and agile methodologies.",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        var jobHistories = new List<JobHistory>
        {
            new JobHistory
            {
                ResumeId = 1,
                CompanyName = "Robbins Gioia",
                Location = "Hill Afb, UT",
                JobTitle = "Systems Analyst",
                StartDate = new DateTime(2014, 3, 1),
                EndDate = new DateTime(2015, 7, 30),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new JobHistory
            {
                ResumeId = 1,
                CompanyName = "Web Innovations LLC",
                Location = "San Francisco, CA",
                JobTitle = "Full-Stack Developer",
                StartDate = new DateTime(2014, 3, 1),
                EndDate = new DateTime(2018, 4, 30),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new JobHistory
            {
                ResumeId = 1,
                CompanyName = "Web Innovations LLC",
                Location = "San Francisco, CA",
                JobTitle = "Full-Stack Developer",
                StartDate = new DateTime(2014, 3, 1),
                EndDate = new DateTime(2015, 7, 30),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new JobHistory
            {
                ResumeId = 1,
                CompanyName = "Web Innovations LLC",
                Location = "San Francisco, CA",
                JobTitle = "Full-Stack Developer",
                StartDate = new DateTime(2014, 3, 1),
                EndDate = new DateTime(2018, 4, 30),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        context.Resumes.AddRange(resumes);
        context.JobHistories.AddRange(jobHistories);
        context.SaveChanges();
    }
}
