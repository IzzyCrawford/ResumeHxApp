import React, { useEffect, useMemo, useState } from 'react';
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
    const [showModal, setShowModal] = useState(false);
    const [submitting, setSubmitting] = useState(false);
    const [formError, setFormError] = useState<string | null>(null);

    const initialForm = useMemo(() => ({
        companyName: '',
        jobTitle: '',
        location: '',
        techStack: '',
        summary: '',
        startDate: '',
        endDate: ''
    }), []);
    const [form, setForm] = useState(initialForm);
    const [responsibilities, setResponsibilities] = useState<string[]>([]);
    const [responsibilityInput, setResponsibilityInput] = useState('');

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
                            <div className="section-actions">
                                <button className="add-experience-btn" onClick={() => setShowModal(true)}>+ Add Work Experience</button>
                            </div>
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
            {showModal && (
                <div className="modal-overlay" role="dialog" aria-modal="true">
                    <div className="modal">
                        <div className="modal-header">
                            <h3>Add Work Experience</h3>
                            <button className="icon-button" onClick={handleClose}>✕</button>
                        </div>
                        <div className="modal-body">
                            {formError && <div className="form-error">{formError}</div>}
                            <div className="form-grid">
                                <div className="form-group">
                                    <label htmlFor="companyName">Company Name<span className="req">*</span></label>
                                    <input id="companyName" type="text" placeholder="Acme Corp" value={form.companyName} onChange={e => setForm({ ...form, companyName: e.target.value })} />
                                </div>
                                <div className="form-group">
                                    <label htmlFor="jobTitle">Job Title<span className="req">*</span></label>
                                    <input id="jobTitle" type="text" placeholder="Senior Developer" value={form.jobTitle} onChange={e => setForm({ ...form, jobTitle: e.target.value })} />
                                </div>
                                <div className="form-group">
                                    <label htmlFor="location">Location</label>
                                    <input id="location" type="text" placeholder="City, State" value={form.location} onChange={e => setForm({ ...form, location: e.target.value })} />
                                </div>
                                <div className="form-group">
                                    <label htmlFor="techStack">Tech Stack</label>
                                    <input id="techStack" type="text" placeholder="React, .NET, Postgres" value={form.techStack} onChange={e => setForm({ ...form, techStack: e.target.value })} />
                                </div>
                                <div className="form-group">
                                    <label htmlFor="startDate">Start Date<span className="req">*</span></label>
                                    <input id="startDate" type="date" placeholder="YYYY-MM" value={form.startDate} onChange={e => setForm({ ...form, startDate: e.target.value })} />
                                </div>
                                <div className="form-group">
                                    <label htmlFor="endDate">End Date</label>
                                    <input id="endDate" type="date" placeholder="YYYY-MM" value={form.endDate} onChange={e => setForm({ ...form, endDate: e.target.value })} />
                                </div>
                                <div className="form-group full">
                                    <label htmlFor="summary">Summary</label>
                                    <textarea id="summary" rows={2} placeholder="Brief overview of the role" value={form.summary} onChange={e => setForm({ ...form, summary: e.target.value })} />
                                </div>
                                <div className="form-group full">
                                    <label htmlFor="responsibilityInput">Responsibilities</label>
                                    <div className="chip-input-container">
                                        <input
                                            id="responsibilityInput"
                                            type="text"
                                            placeholder="e.g., Delivered feature X"
                                            value={responsibilityInput}
                                            onChange={e => setResponsibilityInput(e.target.value)}
                                            onKeyDown={e => {
                                                if (e.key === 'Enter') {
                                                    e.preventDefault();
                                                    handleAddResponsibility();
                                                }
                                            }}
                                        />
                                        <button type="button" className="btn chip-add-btn" onClick={handleAddResponsibility}>Add</button>
                                    </div>
                                    {responsibilities.length > 0 && (
                                        <div className="chip-list">
                                            {responsibilities.map((resp, idx) => (
                                                <div key={idx} className="chip">
                                                    <span>{resp}</span>
                                                    <button type="button" className="chip-remove" onClick={() => handleRemoveResponsibility(idx)}>✕</button>
                                                </div>
                                            ))}
                                        </div>
                                    )}
                                </div>
                            </div>
                        </div>
                        <div className="modal-actions">
                            <button className="btn secondary" onClick={handleClose} disabled={submitting}>Cancel</button>
                            <button className="btn primary" onClick={handleSubmit} disabled={submitting}>{submitting ? 'Saving…' : 'Save'}</button>
                        </div>
                    </div>
                </div>
            )}
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

    function handleClose() {
        if (submitting) return;
        setShowModal(false);
        setForm(initialForm);
        setResponsibilities([]);
        setResponsibilityInput('');
        setFormError(null);
    }

    function handleAddResponsibility() {
        const trimmed = responsibilityInput.trim();
        if (!trimmed) return;
        setResponsibilities([...responsibilities, trimmed]);
        setResponsibilityInput('');
    }

    function handleRemoveResponsibility(index: number) {
        setResponsibilities(responsibilities.filter((_, i) => i !== index));
    }

    async function handleSubmit() {
        if (!resume) return;
        setFormError(null);
        // Basic validation
        if (!form.companyName.trim() || !form.jobTitle.trim() || !form.startDate) {
            setFormError('Company, Job Title, and Start Date are required.');
            return;
        }

        const startDateIso = new Date(form.startDate + 'T00:00:00Z').toISOString();
        const endDateIso = form.endDate ? new Date(form.endDate + 'T00:00:00Z').toISOString() : null;

        const payload = {
            companyName: form.companyName.trim(),
            jobTitle: form.jobTitle.trim(),
            location: form.location.trim(),
            techStack: form.techStack.trim(),
            summary: form.summary.trim(),
            startDate: startDateIso,
            endDate: endDateIso,
            responsibilities
        };

        try {
            setSubmitting(true);
            const res = await fetch(`/api/resumes/${resume.id}/jobs`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(payload)
            });
            if (!res.ok) {
                const text = await res.text();
                throw new Error(`Save failed: ${res.status} ${text}`);
            }
            const created: JobHistory = await res.json();
            const updated = [created, ...jobHistories];
            const sorted = [...updated].sort((a, b) => {
                const endA = a.endDate ? new Date(a.endDate).getTime() : Number.POSITIVE_INFINITY;
                const endB = b.endDate ? new Date(b.endDate).getTime() : Number.POSITIVE_INFINITY;
                if (endA !== endB) return endB - endA;
                const startA = new Date(a.startDate).getTime();
                const startB = new Date(b.startDate).getTime();
                return startB - startA;
            });
            setJobHistories(sorted);
            handleClose();
        } catch (err: any) {
            console.error(err);
            setFormError(err.message || 'Something went wrong.');
        } finally {
            setSubmitting(false);
        }
    }
};

export default Resume;