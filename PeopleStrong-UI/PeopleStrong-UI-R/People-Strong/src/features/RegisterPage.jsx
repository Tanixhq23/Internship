import React from "react";

export default function RegisterPage({ onSwitch }) {
  return (
    <div className="form-container">
      <h1 className="project-name">PeopleStrong</h1>
      <h2>Create Account</h2>
      <input type="text" placeholder="Name" />
      <input type="email" placeholder="Email" />
      <input type="password" placeholder="Password" />
      <button className="btn">Register</button>

      <p className="muted-text">
        Already have an account?{" "}
        <span className="link" onClick={() => onSwitch("login")}>
          Back to Login
        </span>
      </p>
    </div>
  );
}




