import React, { useState } from "react";
import { registerUser } from "../services/api";

export default function RegisterPage({ onSwitch }) {
  // State variables for each field
  const [userName, setUserName] = useState("");
  const [fullName, setFullName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const handleRegister = async () => {
    try {
      // DTO keys exactly match backend
      await registerUser({
        userName: userName,
        fullName: fullName,
        email: email,
        password: password,
      });

      alert("Registration successful!");
      onSwitch("login"); 
    } catch (error) {
      alert(error.message);
    }
  };

  return (
    <div className="form-container">
      <h1 className="project-name">PeopleStrong</h1>
      <h2>Create Account</h2>

      <input
        type="text"
        placeholder="Username"
        value={userName}
        onChange={(e) => setUserName(e.target.value)}
      />

      <input
        type="text"
        placeholder="Full Name (optional)"
        value={fullName}
        onChange={(e) => setFullName(e.target.value)}
      />

      <input
        type="email"
        placeholder="Email Address"
        value={email}
        onChange={(e) => setEmail(e.target.value)}
      />

      <input
        type="password"
        placeholder="Password"
        value={password}
        onChange={(e) => setPassword(e.target.value)}
      />

      <button className="btn" onClick={handleRegister}>
        Register
      </button>

      <p className="muted-text">
        Already have an account?{" "}
        <span className="link" onClick={() => onSwitch("login")}>
          Back to Login
        </span>
      </p>
    </div>
  );
}
