import React from 'react';
import './Home.css';

const Home: React.FC = () => {
    return (
        <div className="home">
            <h1>Welcome to ResumeHx</h1>
            <p>This is a modern application built with React and ASP.NET Core.</p>
            <div className="home-columns">
                <div className="home-content">
                    <div className="features">
                        <h2>This Apps Features</h2>
                        <ul>
                            <li>React Frontend</li>
                            <li>.NET Backend</li>
                            <li>TypeScript</li>
                            <li>Postgres</li>
                            <li>Docker Support</li>
                            <li>Weather Forecast Demo</li>
                        </ul>
                    </div>
                </div>
                <div className="home-content-intro">
                <p>
                    Welcome! I’m thrilled you’re here. With over a decade of experience as an Application Analyst and Software Engineer, I’ve worked on optimizing business workflows, developing scalable software solutions, and modernizing legacy systems like BizTalk. My journey includes full-stack development, API design, Azure DevOps, SQL Server, and deploying innovative products that improve operational efficiency. As an Army veteran and collaborative team player, I enjoy solving complex problems, implementing agile practices, and aligning technology with business goals to create secure, high-performing applications. Take a look around to see the projects and skills I bring to every challenge!
                </p>
            </div>
            </div>
        </div>
    );
};

export default Home;