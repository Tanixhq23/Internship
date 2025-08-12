import React, { useState } from "react";
import "./App.css";
import LoginPage from "./features/LoginPage";
import RegisterPage from "./features/RegisterPage";
import ForgotPasswordPage from "./features/ForgetPassword";
import ResetPasswordPage from "./features/ResetPasswordPage";

function App() {
  const [activePage, setActivePage] = useState("login");

  const renderPage = () => {
    switch (activePage) {
      case "login":
        return <LoginPage onSwitch={setActivePage} />;
      case "register":
        return <RegisterPage onSwitch={setActivePage} />;
      case "forgot":
        return <ForgotPasswordPage onSwitch={setActivePage} />;
      case "reset":
        return <ResetPasswordPage onSwitch={setActivePage} />;
      default:
        return <LoginPage onSwitch={setActivePage} />;
    }
  };

  return (
    <div className="auth-container">
      <div className="auth-box slide-animation">{renderPage()}</div>
    </div>
  );
}

export default App;




