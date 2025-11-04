import React from 'react';
import { Link } from 'react-router-dom';
import './Navigation.css';

const Navigation: React.FC = () => {
    return (
        <nav className="navigation">
            <div className="nav-content">
                <div className="nav-brand">
                    <Link to="/">ResumeHx</Link>
                </div>
                <div className="nav-links">
                    <Link to="/" className="nav-link">Home</Link>
                    <Link to="/resume" className="nav-link">Resume</Link>
                    <Link to="/forecast" className="nav-link">Weather Forecast</Link>
                    <Link to="/orders" className="nav-link">Orders</Link>
                    <Link to="/ideas" className="nav-link">Submit an Idea</Link>
                </div>
            </div>
        </nav>
    );
};

export default Navigation;