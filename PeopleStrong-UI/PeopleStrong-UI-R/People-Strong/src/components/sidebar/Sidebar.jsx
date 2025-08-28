// src/components/sidebar/Sidebar.jsx

import React from 'react';
import'../../styles/Sidebar.css';
import { Link } from 'react-router-dom'; // Use Link for navigation
// import './Sidebar.css'; // Make sure you import your sidebar styles here

// The Sidebar component now receives an onLogout prop
const Sidebar = ({ onLogout }) => {
  return (
    <div className="sidebar">
      {/* Top section with profile icon and name */}
      <div className="sidebar-profile">
        <div className="profile-icon">SP</div>
        <span>Profile</span>
      </div>

      {/* Navigation links */}
      <div className="sidebar-nav">
        <ul>
          <li><Link to="/dashboard">Home</Link></li>
          <li><Link to="/dashboard/attendance">Attendance</Link></li>
          <li><Link to="/dashboard/add-employee">Add Employee</Link></li>
          <li><Link to="/dashboard/job-openings">Job Openings</Link></li>
          <li><Link to="/dashboard/leaves">Leaves</Link></li>
        </ul>
      </div>

      {/* Logout button at the bottom */}
      <div className="sidebar-logout">
        {/* Call the onLogout function passed from the parent */}
        <button onClick={onLogout}>Logout</button>
      </div>
    </div>
  );
};

export default Sidebar;