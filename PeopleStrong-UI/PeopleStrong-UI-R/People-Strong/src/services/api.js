// src/services/api.js
import axios from 'axios';

const API = axios.create({
  baseURL: 'http://localhost:5000/api', // Replace with your actual backend URL
});

// Login API function
export const loginUser = async (userData) => {
  try {
    const response = await API.post('/login', userData);
    return response.data;
  } catch (error) {
    throw error.response?.data?.message || "Login failed";
  }
};
