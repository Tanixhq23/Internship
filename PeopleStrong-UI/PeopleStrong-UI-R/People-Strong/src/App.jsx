import React, { useState } from "react";
import "./App.css";
import LoginPage from "./features/LoginPage";
import RegisterPage from "./features/RegisterPage";
import ForgotPasswordPage from "./features/ForgetPassword";
import ResetPasswordPage from "./features/ResetPasswordPage";
import DashboardPage from "./features/Dashboard/DashboardPage";


function App() {
  const [activePage, setActivePage] = useState("login"); 
  const [isAuthenticated, setIsAuthenticated] = useState(false); // ✅ check login status

  // ✅ Handle login success
  const handleLoginSuccess = () => {
    setIsAuthenticated(true);
  };

  // ✅ If user is logged in → show dashboard
  if (isAuthenticated) {
    return <DashboardPage />;
  }

  // ✅ If user is NOT logged in → show auth pages
  const renderPage = () => {
    switch (activePage) {
      case "login":
        return <LoginPage onSwitch={setActivePage} onLoginSuccess={handleLoginSuccess} />;
      case "register":
        return <RegisterPage onSwitch={setActivePage} />;
      case "forgot":
        return <ForgotPasswordPage onSwitch={setActivePage} />;
      case "reset":
        return <ResetPasswordPage onSwitch={setActivePage} />;
      default:
        return <LoginPage onSwitch={setActivePage} onLoginSuccess={handleLoginSuccess} />;
    }
  };

  return (
    <div className="auth-container">
      <div className="auth-box slide-animation">{renderPage()}</div>
    </div>
  );
}

export default App;