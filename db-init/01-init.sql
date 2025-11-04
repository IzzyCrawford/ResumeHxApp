-- Initialize additional database and user for ResumeHxApp backend
CREATE USER resumehxuser WITH PASSWORD 'resumehxpass';
CREATE DATABASE resumehxdb OWNER resumehxuser;