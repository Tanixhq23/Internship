 import React from 'react'
 import'../../styles/ProfileCard.css';


const ProfileCard = () => {
  return (
    <div className="card-container profile-card">
      <h3>Your Profile</h3>
      <div className="profile-detail">
        <p>Employee ID</p>
        <span>&rarr;</span>
      </div>
      <div className="profile-detail">
        <p>Employee Name</p>
        <span>&rarr;</span>
      </div>
      <div className="profile-detail">
        <p>Employee Department</p>
        <span>&rarr;</span>
      </div>
      <div className="profile-detail">
        <p>Employee Role</p>
        <span>&rarr;</span>
      </div>
    </div>
  );
};

export default ProfileCard;