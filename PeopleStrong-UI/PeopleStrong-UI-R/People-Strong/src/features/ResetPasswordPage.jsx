import React from "react";

export default function ResetPasswordPage({ onSwitch }) {
  return (
    <div className="form-container">
      <h1 className="project-name">PeopleStrong</h1>
      <h2>Reset Password</h2>
      <input type="password" placeholder="New Password" />
      <input type="password" placeholder="Confirm Password" />
      <button className="btn">Reset Password</button>

      <p className="muted-text" onClick={() => onSwitch("login")}>
        Back to Login
      </p>
    </div>
  );
}
