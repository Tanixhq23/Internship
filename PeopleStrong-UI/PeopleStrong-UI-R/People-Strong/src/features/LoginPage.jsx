import React from "react";

export default function LoginPage({ onSwitch }) {
  return (
    <div className="form-container">
      <h1 className="project-name">PeopleStrong</h1>
      <h2>Welcome Back</h2>
      <input type="email" placeholder="Email" />
      <input type="password" placeholder="Password" />
      <button className="btn">Login</button>

      <p className="muted-text" onClick={() => onSwitch("forgot")}>
        Forgot Password?
      </p>
      <p className="muted-text">
        Don't have an account?{" "}
        <span className="link" onClick={() => onSwitch("register")}>
          Register
        </span>
      </p>
    </div>
  );
}


