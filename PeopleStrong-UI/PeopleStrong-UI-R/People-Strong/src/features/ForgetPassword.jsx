import React from "react";

export default function ForgotPasswordPage({ onSwitch }) {
  return (
    <div className="form-container">
      <h1 className="project-name">PeopleStrong</h1>
      <h2>Forgot Password</h2>
      <input type="email" placeholder="Enter your email" />
      <button className="btn">Send Reset Link</button>

      <p className="muted-text" onClick={() => onSwitch("login")}>
        Back to Login
      </p>
    </div>
  );
}



