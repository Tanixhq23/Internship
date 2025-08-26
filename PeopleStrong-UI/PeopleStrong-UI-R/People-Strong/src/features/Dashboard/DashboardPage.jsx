import React from "react";

function DashboardPage({ onLogout }) {
  return (
    <div style={{ textAlign: "center", padding: "2rem" }}>
      <h1>Welcome to PeopleStrong Dashboard 🎉</h1>
      <p>This is your main application area after login.</p>
      
      <button 
        style={{
          marginTop: "20px",
          padding: "10px 20px",
          backgroundColor: "#7E6A52",
          color: "#F6F0EC",
          border: "none",
          borderRadius: "8px",
          cursor: "pointer"
        }}
        onClick={onLogout}
      >
        Logout
      </button>
    </div>
  );
}

export default DashboardPage;