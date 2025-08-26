
import React, { useState } from "react";
import { loginUser } from "../services/api"; 

export default function LoginPage({ onSwitch, onLoginSuccess }) {
  const [credentials, setCredentials] = useState({
    email: "",
    password: "",
  });

  const [loading, setLoading] = useState(false);

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

     
      if (res.token) {
        localStorage.setItem("token", res.token); 
        onLoginSuccess(); 
      } else {
        alert("Invalid response, try again!");
      }
    } catch (err) {
      alert(err.message || "Login failed");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="form-container">
      <h1 className="project-name">PeopleStrong</h1>
      <h2>Login</h2>

      <input
        type="email"
        name="email"
        placeholder="Email"
        value={credentials.email}
        onChange={handleChange}
      />

      <input
        type="password"
        name="password"
        placeholder="Password"
        value={credentials.password}
        onChange={handleChange}
      />

      <button className="btn" onClick={handleLogin} disabled={loading}>
        {loading ? "Logging in..." : "Login"}
      </button>

      <p className="muted-text">
        Don’t have an account?{" "}
        <span className="link" onClick={() => onSwitch("register")}>
          Create Account
        </span>
      </p>

      <p className="muted-text">
        <span className="link" onClick={() => onSwitch("forgot")}>
          Forgot Password?
        </span>
      </p>
    </div>
  );
}
