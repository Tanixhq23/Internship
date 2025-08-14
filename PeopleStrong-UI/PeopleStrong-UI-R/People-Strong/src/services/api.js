// src/api.js

const API_BASE_URL = "https://localhost:7106/api"; // 🔹 Change to your backend URL

// Common POST request function
async function post(endpoint, data) {
  const response = await fetch(`${API_BASE_URL}/${endpoint}`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      ...(localStorage.getItem("token") && {
        Authorization: `Bearer ${localStorage.getItem("token")}`
      })
    },
    body: JSON.stringify(data),
  });

  // Agar response 2xx nahi hai toh error throw karo
  if (!response.ok) {
    let errorMsg = "API request failed";
    try {
      const errorData = await response.json();
      errorMsg = errorData.message || errorMsg;
    } catch {}
    throw new Error(errorMsg);
  }

  return await response.json();
}

// ✅ Register API
export function registerUser(userData) {
  return post("Auth/register", userData);
}

// ✅ Login API
export function loginUser(loginData) {
  return post("Auth/login", loginData);
}
