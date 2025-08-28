 import React, { useState } from 'react';

// Props will be passed from App.jsx to handle navigation and data
const AddEmployeeForm = ({ onFormSubmit, onBackToDashboard, message, messageType }) => {
  const [employeeDetails, setEmployeeDetails] = useState({
    firstName: '',
    lastName: '',
    email: '', // Personal email
    department: '',
    role: '',
    manager: '',
    shift: '',
    officeEmail: '',
  });

  // Handle input changes
  const handleChange = (e) => {
    const { name, value } = e.target;
    setEmployeeDetails((prevDetails) => ({
      ...prevDetails,
      [name]: value,
    }));
  };

  // Handles the submission of the Add Employee form
  const handleSubmit = (e) => {
    e.preventDefault();

    // Basic client-side validation for all required fields
    if (
      !employeeDetails.firstName ||
      !employeeDetails.lastName ||
      !employeeDetails.email ||
      !employeeDetails.department ||
      !employeeDetails.role ||
      !employeeDetails.manager ||
      !employeeDetails.shift ||
      !employeeDetails.officeEmail
    ) {
      // Pass error message back to parent (App.jsx)
      onFormSubmit(null, 'Please fill in all fields.', 'error');
      return;
    }

    // If validation passes, pass the data to the parent component (App.jsx)
    onFormSubmit(employeeDetails, 'Please review the employee details before sending.', 'info');
    // Optionally reset the form fields after successful submission to preview
    setEmployeeDetails({
      firstName: '', lastName: '', email: '', department: '', role: '',
      manager: '', shift: '', officeEmail: '',
    });
  };

  return (
    <>
      <div className="flex justify-between items-center mb-6">
        <h2 className="text-2xl sm:text-3xl font-bold text-gray-800">Add New Employee</h2>
        <button
          onClick={onBackToDashboard}
          className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-opacity-50 transition duration-200 ease-in-out"
        >
          Back to Dashboard
        </button>
      </div>

      {message && (
        <div
          className={`p-3 rounded-md mb-4 text-sm ${
            messageType === 'success' ? 'bg-green-100 text-green-700' :
            messageType === 'error' ? 'bg-red-100 text-red-700' :
            'bg-blue-100 text-blue-700'
          }`}
        >
          {message}
        </div>
      )}

      <form onSubmit={handleSubmit} className="space-y-5">
        {/* Employee Information */}
        <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
          <div>
            <label htmlFor="firstName" className="block text-sm font-medium text-gray-700">First Name</label>
            <input
              type="text"
              id="firstName"
              name="firstName"
              value={employeeDetails.firstName}
              onChange={handleChange}
              className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
              required
            />
          </div>
          <div>
            <label htmlFor="lastName" className="block text-sm font-medium text-gray-700">Last Name</label>
            <input
              type="text"
              id="lastName"
              name="lastName"
              value={employeeDetails.lastName}
              onChange={handleChange}
              className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
              required
            />
          </div>
        </div>

        <div>
          <label htmlFor="email" className="block text-sm font-medium text-gray-700">Personal Email Address</label>
          <input
            type="email"
            id="email"
            name="email"
            value={employeeDetails.email}
            onChange={handleChange}
            className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
            required
          />
        </div>

        <div>
          <label htmlFor="officeEmail" className="block text-sm font-medium text-gray-700">Office Email Address</label>
          <input
            type="email"
            id="officeEmail"
            name="officeEmail"
            value={employeeDetails.officeEmail}
            onChange={handleChange}
            className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
            required
          />
        </div>

        <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
          <div>
            <label htmlFor="department" className="block text-sm font-medium text-gray-700">Department</label>
            <input
              type="text"
              id="department"
              name="department"
              value={employeeDetails.department}
              onChange={handleChange}
              className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
              required
            />
          </div>
          <div>
            <label htmlFor="role" className="block text-sm font-medium text-gray-700">Role</label>
            <input
              type="text"
              id="role"
              name="role"
              value={employeeDetails.role}
              onChange={handleChange}
              className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
              required
            />
          </div>
        </div>

        <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
          <div>
            <label htmlFor="manager" className="block text-sm font-medium text-gray-700">Manager</label>
            <input
              type="text"
              id="manager"
              name="manager"
              value={employeeDetails.manager}
              onChange={handleChange}
              className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
              required
            />
          </div>
          <div>
            <label htmlFor="shift" className="block text-sm font-medium text-gray-700">Shift</label>
            <input
              type="text"
              id="shift"
              name="shift"
              value={employeeDetails.shift}
              onChange={handleChange}
              className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
              required
            />
          </div>
        </div>

        <div>
          <button
            type="submit"
            className="w-full flex justify-center py-2 px-4 border border-transparent rounded-md shadow-sm text-lg font-medium text-white bg-green-600 hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500 transition duration-200 ease-in-out"
          >
            Add Employee
          </button>
        </div>
      </form>
    </>
  );
};

export default AddEmployeeForm;