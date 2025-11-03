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
                Summary = "Army veteran and results-driven software engineer and analyst with 10+ years of experience developing, testing, and optimizing software solutions while analyzing complex systems to improve performance and functionality. Skilled in full-stack development, data analysis, and software lifecycle management with a proven record of delivering scalable applications that enhance efficiency and reduce costs. Adept at collaborating with cross-functional teams, implementing agile methodologies, and ensuring solutions align with organizational objectives. Seeking to leverage technical expertise, analytical background, and leadership skills to excel as a Senior Software Engineer.",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        context.Resumes.AddRange(resumes);
        context.SaveChanges(); // Save resumes first to get their IDs
        
        var resume = context.Resumes.First();
        
        // Add sample job histories
        var jobHistories = new List<JobHistory>
        {
            new JobHistory
            {
                ResumeId = resume.Id,
                CompanyName = "Robbins Gioia",
                Location = "Hill Afb, UT",
                JobTitle = "Systems Analyst",
                StartDate = new DateTime(2014, 3, 1, 0, 0, 0, DateTimeKind.Utc),
                EndDate = new DateTime(2015, 7, 30, 0, 0, 0, DateTimeKind.Utc),
                TechStack = "Visual Studios, Team Foundation Server, Win Forms, WPF, C#/ASP.NET MVC, JQuery, Oracle DB, SQL Server",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new JobHistory
            {
                ResumeId = resume.Id,
                CompanyName = "Database Solution, INC",
                Location = "Mobile, AL",
                JobTitle = "DevOps Support Analyst",
                TechStack = "Visual Studios, Jira, C#/ASP.NET MVC, Ext JS, Mirth Connect, Graylog, SQL Server, Postgres",
                StartDate = new DateTime(2015, 12, 1, 0, 0, 0, DateTimeKind.Utc),
                EndDate = new DateTime(2017, 12, 30, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new JobHistory
            {
                ResumeId = resume.Id,
                CompanyName = "Rural Sourcing, INC",
                Location = "Mobile, AL",
                JobTitle = "Analyst/Consultant promoted to Analyst/Consultant II",
                TechStack = "Visual Studios, Jira, Team Foundation Server, RESTful Service, Entity Framework 6, React, Angular 7, Azure DevOps, CouchDB, C#, ASP.NET MVC, PostgreSQL",
                StartDate = new DateTime(2018, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                EndDate = new DateTime(2020, 9, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new JobHistory
            {
                ResumeId = resume.Id,
                CompanyName = "Trustedi10 - Abeo - Ventra",
                Location = "Dallas, TX - Remote",
                JobTitle = "Senior Software Engineer",
                TechStack = "Visual Studios, Jira, RESTful Services, C#, ASP.NET MVC, MySQL, Python",
                StartDate = new DateTime(2020, 9, 1, 0, 0, 0, DateTimeKind.Utc),
                EndDate = new DateTime(2022, 2, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new JobHistory
            {
                ResumeId = resume.Id,
                CompanyName = "Cuna Mutual Group",
                Location = "Madison, WI - Remote",
                JobTitle = "Application Analyst IV",
                TechStack = "Visual Studios, Jira, RESTful Service, Entity Framework Core, Azure DevOps, C#, ASP.NET Core, SQLServer, Service Bus",
                StartDate = new DateTime(2022, 2, 1, 0, 0, 0, DateTimeKind.Utc),
                EndDate = new DateTime(2025, 7, 30, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        context.JobHistories.AddRange(jobHistories);
        context.SaveChanges();

        // Create job responsibilities linked by JobHistoryId
        var responsibilities = new List<JobResponsibility>();
        foreach (var job in context.JobHistories.Where(j => j.ResumeId == resume.Id).ToList())
        {
            if (job.CompanyName == "Robbins Gioia")
            {
                responsibilities.AddRange(new[]
                {
                    new JobResponsibility { JobHistoryId = job.Id, Description = "Supported USAF PDMSS web-based solutions for Hill AFB." },
                    new JobResponsibility { JobHistoryId = job.Id, Description = "Integrated multiple legacy systems with modern enterprise platforms to streamline data exchange and improve organizational workflow." },
                    new JobResponsibility { JobHistoryId = job.Id, Description = "Consolidated reporting processes by linking disparate databases, increasing data accessibility and efficiency for cross-departmental projects." },
                    new JobResponsibility { JobHistoryId = job.Id, Description = "Upgraded system integration protocols at Robbins Gioia, enabling seamless interoperability between critical business applications and reducing process redundancies acrossdepartments." },
                    new JobResponsibility { JobHistoryId = job.Id, Description = "Performed data mining and analysis and Debugged C#/ASP.NET apps using structured methods." }
                });
            }
            else if (job.CompanyName == "Database Solution, INC")
            {
                responsibilities.AddRange(new[]
                {
                    new JobResponsibility { JobHistoryId = job.Id, Description = "Troubleshoot AccuReg workflow from client hospitals to vendors, resolving data issues and providing network connectivity for HL7 interfaces." },
                    new JobResponsibility { JobHistoryId = job.Id, Description = "Data analysis for client and vendor EDI connections including EDI 837, EDI 835, EDI 270/271, EDI 275, EDI 276/277, and EDI 278." },
                    new JobResponsibility { JobHistoryId = job.Id, Description = "Streamlined deployment pipelines by configuring automation scripts and implementing solutions, facilitating reliable and efficient software releases across development environments." },
                    new JobResponsibility { JobHistoryId = job.Id, Description = "Upgraded monitoring and alerting frameworks within revenue cycle management solutions, deploying automated remediation scripts to minimize system disruptions and uphold high availability for mission-critical healthcare applications." },
                    new JobResponsibility { JobHistoryId = job.Id, Description = "Authored advanced SQL queries for data analysis. (SQL Server/Postgres)" }
                });
            }
            else if (job.CompanyName == "Rural Sourcing, INC")
            {
                responsibilities.AddRange(new[]
                {
                    new JobResponsibility { JobHistoryId = job.Id, Description = "Task as a mid-level and senior developer on various client projects, clients included UPS, DealerRater, Cayuse, SSI Group, and Internal Projects." },
                    new JobResponsibility { JobHistoryId = job.Id, Description = "Built full-stack web applications for multiple client projects." },
                    new JobResponsibility { JobHistoryId = job.Id, Description = "Evaluated client systems and workflows, delivering targeted process enhancements that strengthened operational reliability and supported measurable business outcomes." },
                    new JobResponsibility { JobHistoryId = job.Id, Description = "Interpreted complex datasets to identify process gaps, providing actionable recommendations that strengthened project outcomes and enhanced client satisfaction." },
                    new JobResponsibility { JobHistoryId = job.Id, Description = "Mapped large-scale data processes and documented workflow dependencies, enabling more accurate forecasting and prioritization for diverse client portfolios." },
                    new JobResponsibility { JobHistoryId = job.Id, Description = "Compiled and synthesized cross-departmental data sources, delivering unified analytical models that supported real-time decision-making for high-impact client projects." },
                    new JobResponsibility { JobHistoryId = job.Id, Description = "Engineered interactive dashboards using web development tools (JavaScript, HTML5, CSS3) to streamline real-time visualization for 5+ client-facing reports monthly." },
                    new JobResponsibility { JobHistoryId = job.Id, Description = "Developed and integrated RESTful API solutions using C#, and Postman to automate data exchange across 5+ client systems." },
                    new JobResponsibility { JobHistoryId = job.Id, Description = "Integrated automated data validation routines into existing workflows, elevating report reliability and minimizing manual intervention for client deliverables." }
                });
            }
            else if (job.CompanyName == "Trustedi10 - Abeo - Ventra")
            {
                responsibilities.AddRange(new[]
                {
                    new JobResponsibility { JobHistoryId = job.Id, Description = "Developed the ETL Engine for processing patient medical records from pdf files, ensuring data accuracy and efficiency." },
                    new JobResponsibility { JobHistoryId = job.Id, Description = "Tokenized medical records to identify key terms for medical billers/coders, improving workflow efficiency." },
                    new JobResponsibility { JobHistoryId = job.Id, Description = "Refined existing ETL and NLP components by integrating modular design principles, boosting maintainability and enabling swift adaptation to complex patient data processing demands at TRUSTEDi10." },
                    new JobResponsibility { JobHistoryId = job.Id, Description = "Participate in collaborative software development practices, particularly performing merge request reviews, providing design feedback, etc." },
                    new JobResponsibility { JobHistoryId = job.Id, Description = "Devised automated data validation routines that increased the accuracy and consistency of patient records processed through TRUSTEDi10's ETL workflows." },
                    new JobResponsibility { JobHistoryId = job.Id, Description = "Integrated robust logging and monitoring solutions to track data pipeline health and facilitate rapid issue diagnosis, contributing to increased operational reliability at TRUSTEDi10." }
                });
            }
            else if (job.CompanyName == "Cuna Mutual Group")
            {
                responsibilities.AddRange(new[]
                {
                    new JobResponsibility { JobHistoryId = job.Id, Description = "Team Contact for TruStage Products Execution Area." },
                    new JobResponsibility { JobHistoryId = job.Id, Description = "Unified legacy and modernized applications by integrating APIs and middleware solutions, reinforcing scalability, security, and interoperability across critical business platforms." },
                    new JobResponsibility { JobHistoryId = job.Id, Description = "Refined data migration strategies by leveraging advanced ETL tools (ADF) to ensure seamless transitions between legacy and cloud-based systems, minimizing business disruption and ensuring data integrity for mission-critical applications." },
                    new JobResponsibility { JobHistoryId = job.Id, Description = "Advanced system reliability by deploying real-time monitoring solutions, enabling immediate detection and resolution of critical application incidents across enterprise environments." },
                    new JobResponsibility { JobHistoryId = job.Id, Description = "Retired BizTalk integration in PXA workflows" },
                });
            }
        }

        if (responsibilities.Count > 0)
        {
            context.JobResponsibilities.AddRange(responsibilities);
            context.SaveChanges();
        }
    }
}
