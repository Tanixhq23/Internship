import React from 'react';
import { Navigate } from 'react-router-dom';

const PrivateRoute = ({ children }) => {
  const authToken = localStorage.getItem("authToken");
  
  // If the user has a token, render the child components (the Dashboard)
  if (authToken) {
    return children;
  }
  
  // Otherwise, redirect them to the login page
  return <Navigate to="/login" replace />;
};

export default PrivateRoute;