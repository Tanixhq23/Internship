// src/features/LoginPage.jsx
import React, { useState } from "react";
import { loginUser } from "../services/api";
import { useNavigate } from "react-router-dom";
import "../styles/LoginPage.css";

export default function LoginPage() {
  const [credentials, setCredentials] = useState({
    email: "",
    password: "",
  });

  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  const handleChange = (e) => {
    setCredentials({
      ...credentials,
      [e.target.name]: e.target.value,
    });
  };

  const handleLogin = async () => {
    setLoading(true);
    try {
      const res = await loginUser(credentials);
      alert("Login successful!");
      console.log("Token:", res.token);
      // Navigate to dashboard after successful login
      navigate("/dashboard");
    } catch (err) {
      alert(err.message || "Login failed");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="form-container">
      <div className="login-box">
        <h1 className="project-name">PeopleStrong</h1>
        <h2>Login</h2>

        <form
          onSubmit={(e) => {
            e.preventDefault();
            handleLogin();
          }}
        >
          <input
            type="email"
            name="email"
            placeholder="Email"
            value={credentials.email}
            onChange={handleChange}
            required
          />

          <input
            type="password"
            name="password"
            placeholder="Password"
            value={credentials.password}
            onChange={handleChange}
            required
          />

          <button className="login-btn" type="submit" disabled={loading}>
            {loading ? "Logging in..." : "Login"}
          </button>
        </form>

        <p className="muted-text">
          Don’t have an account?{" "}
          <span className="link" onClick={() => navigate("/register")}>
            Create Account
          </span>
        </p>

        <p className="muted-text">
          <span className="link" onClick={() => navigate("/forgot-password")}>
            Forgot Password?
          </span>
        </p>
      </div>
    </div>
  );
}
