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

        // Add sample job histories
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
                TechStack = "Visual Studios, Team Foundation Server, Win Forms, WPF, C#/ASP.NET MVC, JQuery, Oracle DB, SQL Server",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new JobHistory
            {
                ResumeId = 1,
                CompanyName = "Database Solution, INC",
                Location = "Mobile, AL",
                JobTitle = "DevOps Support Analyst",
                TechStack = "Visual Studios, Jira, C#/ASP.NET MVC, Ext JS, Mirth Connect, Graylog, SQL Server, Postgres",
                StartDate = new DateTime(2015, 12, 1),
                EndDate = new DateTime(2017, 12, 30),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new JobHistory
            {
                ResumeId = 1,
                CompanyName = "Rural Sourcing, INC",
                Location = "Mobile, AL",
                JobTitle = "Analyst/Consultant promoted to Analyst/Consultant II",
                TechStack = "Visual Studios, Jira, Team Foundation Server, RESTful Service, Entity Framework 6, React, Angular 7, Azure DevOps, CouchDB, C#, ASP.NET MVC, PostgreSQL",
                StartDate = new DateTime(2018, 1, 1),
                EndDate = new DateTime(2020, 9, 1),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new JobHistory
            {
                ResumeId = 1,
                CompanyName = "Trustedi10 - Abeo - Ventra",
                Location = "Dallas, TX - Remote",
                JobTitle = "Senior Software Engineer",
                TechStack = "Visual Studios, Jira, RESTful Services, C#, ASP.NET MVC, MySQL, Python",
                StartDate = new DateTime(2020, 9, 1),
                EndDate = new DateTime(2022, 2, 1),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new JobHistory
            {
                ResumeId = 1,
                CompanyName = "Cuna Mutual Group",
                Location = "Madison, WI - Remote",
                JobTitle = "Full-Stack Developer",
                TechStack = "Visual Studios, Jira, RESTful Service, Entity Framework Core, Azure DevOps, C#, ASP.NET Core, SQLServer, Service Bus",
                StartDate = new DateTime(2022, 2, 1),
                EndDate = new DateTime(2025, 7, 30),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        // Add sample job responsibilities
        var jobResponsibilities = new List<JobResponsibility>
        {
             new JobResponsibility
            {
                JobHistoryId = 1,
                Description = "Supported U.S. Air Force Complex for Enhancement of PDMSS web based automated solutions and reports."
            },
            new JobResponsibility
            {
                JobHistoryId = 1,
                Description = "Provided data mining and analysis of data."
            },
            new JobResponsibility
            {
                JobHistoryId = 1,
                Description = "Utilized problem solving and debugging skills using structured, methodical approach using C#/ASP.NET. "
            },
            new JobResponsibility
            {
                JobHistoryId = 1,
                Description = "Reviewed project plans to coordinate project activities between clients and developers."
            },
            new JobResponsibility
            {
                JobHistoryId = 1,
                Description = "Communicated major and minor bug fixes and upgrades."
            },
            new JobResponsibility
            {
                JobHistoryId = 2,
                Description = "Upgraded clients in production to the latest version of the software."
            },
            new JobResponsibility
            {
                JobHistoryId = 2,
                Description = "Utilized C#/ASP.NET skills to confirm bugs across multiple versions of software for enhancement."
            },
            new JobResponsibility
            {
                JobHistoryId = 2,
                Description = "Assisted with the development of products to resolve bugs."
            },
            new JobResponsibility
            {
                JobHistoryId = 2,
                Description = "Provided data mining using SQL Server 2012 and Postgres. "
            },
            new JobResponsibility
            {
                JobHistoryId = 2,
                Description = "Conferred with organizational members to accomplish work activities and escalate issues to vendors for additional support."
            },
            new JobResponsibility
            {
                JobHistoryId = 2,
                Description = "Reviewed project plans to coordinate project activities between clients and developers."
            },
            new JobResponsibility
            {
                JobHistoryId = 2,
                Description = "Arranged deployments of minor and major fixes to database systems to increase the productivity of clients."
            },
            new JobResponsibility
            {
                JobHistoryId = 3,
                Description = "Assigned to work multiple client and internal roles as a developer for companies like UPS, Cayuse, DealerRater, SSI Group."
            },
            new JobResponsibility
            {
                JobHistoryId = 3,
                Description = "Prototyped and evaluated alternative designs in light of security, reliability, continuity and functional completeness."
            },
            new JobResponsibility
            {
                JobHistoryId = 3,
                Description = "Developed full-stack web applications which processed, analyzed, and rendered data visually for the Tech Tuesday App and the Project Managers Dashboards App."
            },
            new JobResponsibility
            {
                JobHistoryId = 3,
                Description = "Conferred with organizational members to accomplish work activities and establish priorities for features. "
            },
            new JobResponsibility
            {
                JobHistoryId = 3,
                Description = "Used Entity objects such as Data Reader, Dataset and Data Adapter, for consistent access to SQL data sources."
            },
            new JobResponsibility
            {
                JobHistoryId = 3,
                Description = "Planned, wrote, and debugged web applications and software with complete accuracy."
            },
            new JobResponsibility
            {
                JobHistoryId = 3,
                Description = "Implement improvements in the design of existing software architecture. "
            },
            new JobResponsibility
            {
                JobHistoryId = 3,
                Description = "Performed extensive development for applications Unit Testing/Validation Testing."
            },
            new JobResponsibility
            {
                JobHistoryId = 3,
                Description = "Coached junior developers in weekly one-on-one sessions, setting professional goals and providing constructive feedback to enhance their problem-solving abilities and code quality."
            },
            new JobResponsibility
            {
                JobHistoryId = 3,
                Description = "Performed extensive development for applications Unit Testing/Validation Testing."
            },
            new JobResponsibility
            {
                JobHistoryId = 4,
                Description = "Gathered requirements, getting sign-off from the business users and prepared system requirements specification for the solution."
            },
            new JobResponsibility
            {
                JobHistoryId = 4,
                Description = "Assisting with development of the ETL Engine used to process/strip data out of pdf files for patient medical records. "
            },
            new JobResponsibility
            {
                JobHistoryId = 4,
                Description = "Assisting with development of the used to token medical records to process and find key terms to help Medical billers/coders."
            },
            new JobResponsibility
            {
                JobHistoryId = 5,
                Description = "Assisted with development and deployment for TruStage Products. "
            },
            new JobResponsibility
            {
                JobHistoryId = 5,
                Description = "Assisted with the retirement of BizTalk application from PXA workflow."
            },
            new JobResponsibility
            {
                JobHistoryId = 5,
                Description = "Conducted expert-level analysis and design to translate business needs into technical specifications, enhancing solution delivery."
            },
            new JobResponsibility
            {
                JobHistoryId = 5,
                Description = "Guided the development of multiple concurrent business systems, providing leadership and architectural guidance to Scrum teams."
            },
            new JobResponsibility
            {
                JobHistoryId = 5,
                Description = "Proactively identified and communicated process improvements, resulting in increased efficiency across IT and business operations."
            },
            new JobResponsibility
            {
                JobHistoryId = 5,
                Description = "Developed and maintained high-quality business systems solutions, ensuring adherence to established methodologies and practices."
            },
            new JobResponsibility
            {
                JobHistoryId = 5,
                Description = "Enhanced application development practices by acquiring and sharing knowledge of new technologies and methodologies."
            },
            new JobResponsibility
            {
                JobHistoryId = 5,
                Description = "Conducted root cause analysis and problem resolution, ensuring timely responses to customer requests."
            },
            new JobResponsibility
            {
                JobHistoryId = 5,
                Description = "Collaborated with IT and business teams to drive impactful changes across organizational units."
            }
        };

        context.Resumes.AddRange(resumes);
        context.JobHistories.AddRange(jobHistories);
        context.JobResponsibilities.AddRange(jobResponsibilities);
        context.SaveChanges();
    }
}
