// src/features/dashboard/DashboardPage.jsx
import React from 'react';
import { useNavigate } from 'react-router-dom';
import Sidebar from '../../components/sidebar/Sidebar.jsx';
import ProfileCard from '../../components/profileCard/ProfileCard.jsx';
import LeavesCard from '../../components/leaveCard/LeavesCard.jsx';
import AttendanceCard from '../../components/attendanceCard/AttendanceCard.jsx';
 import '../../styles/DashboardPage.css'; 


const DashboardPage = () => {
  const navigate = useNavigate();

  const handleLogout = () => {
    localStorage.removeItem("authToken");
    navigate("/login");
  };

  return (
    <div className="dashboard-container">
      {/* Sidebar is on the left */}
      <Sidebar onLogout={handleLogout} /> {/* Pass the logout function to the sidebar */}
      
      {/* Main content area on the right */}
      <div className="main-content">
        {/* The Cards are now rendered directly within the main-content */}
        <ProfileCard />
        <AttendanceCard />
        <LeavesCard />
      </div>
    </div>
  );
};

export default DashboardPage;