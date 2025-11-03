import React, { useEffect, useState } from 'react';
import './Resume.css';

interface JobHistory {
    id: number;
    companyName: string;
    location: string;
    jobTitle: string;
    techStack: string;
    summary: string;
    startDate: string;
    endDate: string | null;
    responsibilities?: JobResponsibility[];
}

interface JobResponsibility {
    id: number;
    jobHistoryId: number;
    description: string;
}

interface Resume {
    id: number;
    fullName: string;
    email: string;
    phone: string;
    summary: string;
    linkedInProfile: string;
    createdAt: string;
    updatedAt: string;
    jobHistories?: JobHistory[];
}

const Resume: React.FC = () => {
    const [resume, setResume] = useState<Resume | null>(null);
    const [jobHistories, setJobHistories] = useState<JobHistory[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        populateResumeData();
    }, []);

    const formatDate = (dateString: string | null) => {
        if (!dateString) return 'Present';
        const date = new Date(dateString);
        return date.toLocaleDateString('en-US', { year: 'numeric', month: 'short' });
    };

    const contents = loading
        ? <p className="loading"><em>Loading resume data...</em></p>
        : error
            ? <p className="error">{error}</p>
            : resume && (
                <div className="resume-container">
                    <div className="resume-sidebar">
                        <div className="profile-photo">
                            <div className="photo-placeholder">
                                {resume.fullName.split(' ').map(n => n[0]).join('')}
                            </div>
                        </div>

                        <div className="sidebar-section">
                            <h2 className="sidebar-heading">CONTACT</h2>
                            <div className="contact-item">
                                <span className="icon">📧</span>
                                <span className="contact-text">{resume.email}</span>
                            </div>
                            <div className="contact-item">
                                <span className="icon">📱</span>
                                <span className="contact-text">{resume.phone}</span>
                            </div>
                            {resume.linkedInProfile && (
                                <div className="contact-item">
                                    <span className="icon">💼</span>
                                    <a href={resume.linkedInProfile} target="_blank" rel="noopener noreferrer" className="contact-link">
                                        LinkedIn Profile
                                    </a>
                                </div>
                            )}
                        </div>

                        <div className="sidebar-section">
                            <h2 className="sidebar-heading">SKILLS</h2>
                            <ul className="skills-list">
                                <li>React & TypeScript</li>
                                <li>.NET Core</li>
                                <li>PostgreSQL</li>
                                <li>Docker</li>
                                <li>Entity Framework</li>
                                <li>REST APIs</li>
                                <li>Git & DevOps</li>
                                <li>Agile Methodologies</li>
                            </ul>
                        </div>
                    </div>

                    <div className="resume-main">
                        <div className="resume-header">
                            <h1 className="resume-name">{resume.fullName}</h1>
                            <p className="resume-title">SOFTWARE ENGINEER</p>
                        </div>

                        <div className="main-section">
                            <h2 className="section-heading">PROFILE</h2>
                            <p className="profile-text">{resume.summary}</p>
                        </div>

                        <div className="main-section">
                            <h2 className="section-heading">WORK EXPERIENCE</h2>
                            {jobHistories.length > 0 ? (
                                jobHistories.map(job => (
                                    <div key={job.id} className="job-entry">
                                        <div className="job-header">
                                            <div className="job-title-company">
                                                <h3 className="job-title">{job.jobTitle}</h3>
                                                <p className="company-name">{job.companyName}</p>
                                            </div>
                                            <div className="job-dates">
                                                {formatDate(job.startDate)} - {formatDate(job.endDate)}
                                                {job.location && (
                                                    <div className="job-location">{job.location}</div>
                                                )}
                                            </div>
                                        </div>
                                        {job.responsibilities && job.responsibilities.length > 0 ? (
                                            <div className="job-responsibilities">
                                                <h4>Responsibilities:</h4>
                                                <ul>
                                                    {job.responsibilities.map((r) => (
                                                        <li key={r.id}>{r.description}</li>
                                                    ))}
                                                </ul>
                                            </div>
                                        ) : (
                                            job.summary && (
                                                <div className="job-summary">
                                                    <p>{job.summary}</p>
                                                </div>
                                            )
                                        )}
                                        {job.techStack && (
                                            <p className="tech-stack">
                                                <strong>Tech Stack:</strong> {job.techStack}
                                            </p>
                                        )}
                                    </div>
                                ))
                            ) : (
                                <p className="no-data">No work experience data available.</p>
                            )}
                        </div>
                    </div>
                </div>
            );

    return (
        <div className="resume-page">
            {contents}
        </div>
    );

    async function populateResumeData() {
        try {
            // Fetch the first resume
            const resumeResponse = await fetch('/api/resumes');
            if (!resumeResponse.ok) {
                throw new Error(`Failed to fetch resume: ${resumeResponse.status}`);
            }
            const resumes = await resumeResponse.json();
            
            if (resumes && resumes.length > 0) {
                const resumeData = resumes[0];
                setResume(resumeData);
                
                // Set job histories from the resume data (now included via API)
                if (resumeData.jobHistories && resumeData.jobHistories.length > 0) {
                    const sorted = [...resumeData.jobHistories].sort((a, b) => {
                        const endA = a.endDate ? new Date(a.endDate).getTime() : Number.POSITIVE_INFINITY;
                        const endB = b.endDate ? new Date(b.endDate).getTime() : Number.POSITIVE_INFINITY;
                        if (endA !== endB) return endB - endA; // Desc by end date (ongoing first)
                        const startA = new Date(a.startDate).getTime();
                        const startB = new Date(b.startDate).getTime();
                        return startB - startA; // Then desc by start date
                    });
                    setJobHistories(sorted);
                }
            } else {
                setError('No resume data found.');
            }
            
            setLoading(false);
        } catch (error) {
            console.error('Error fetching resume data:', error);
            setError('Failed to load resume data. Please try again later.');
            setLoading(false);
        }
    }
};

export default Resume;