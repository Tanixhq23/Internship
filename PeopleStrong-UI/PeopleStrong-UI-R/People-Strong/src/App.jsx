// src/App.jsx
import React from "react";
import { BrowserRouter as Router, Routes, Route, Navigate } from "react-router-dom";
import "./App.css";
import AddEmployeeForm from "./components/Addemployeeform/AddEmployeesform";
import PreviewPage from "./components/Addemployeeform/PreviewPage";

// Import all your page components
import LoginPage from "./features/LoginPage";
import RegisterPage from "./features/RegisterPage";
import ForgotPasswordPage from "./features/ForgetPassword";
import ResetPasswordPage from "./features/ResetPasswordPage";
import DashboardPage from "./features/dashboard/DashboardPage";
import PrivateRoute from "./components/PrivateRoute";

function App() {
  return (
    <Router>
      <div className="auth-container">
        <div className="auth-box slide-animation">
          <Routes>
            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />} />
            <Route path="/forgot-password" element={<ForgotPasswordPage />} />
            <Route path="/reset-password" element={<ResetPasswordPage />} />
            <Route path="/dashboard" element={<DashboardPage />} />
            
            {/* Catch-all route to redirect to login if the URL doesn't match */}
            <Route path="*" element={<Navigate to="/login" replace />} />
          </Routes>
        </div>
      </div>
    </Router>
  );
}

export default App;