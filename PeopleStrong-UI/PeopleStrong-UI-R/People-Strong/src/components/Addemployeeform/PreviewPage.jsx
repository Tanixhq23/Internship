 import React from 'react';

const PreviewPage = ({ previewData, handleGoBackToForm, handleSendDetailsToMail }) => {
  return (
    <>
      <div className="flex justify-between items-center mb-6">
        <h2 className="text-2xl sm:text-3xl font-bold text-gray-800">Review Employee Details</h2>
        <button
          onClick={handleGoBackToForm}
          className="px-4 py-2 bg-gray-600 text-white rounded-md hover:bg-gray-700 focus:outline-none focus:ring-2 focus:ring-gray-500 focus:ring-opacity-50 transition duration-200 ease-in-out"
        >
          Edit Details
        </button>
      </div>

      {previewData && (
        <div className="space-y-4 text-gray-700">
          <p><strong>First Name:</strong> {previewData.firstName}</p>
          <p><strong>Last Name:</strong> {previewData.lastName}</p>
          <p><strong>Personal Email:</strong> {previewData.email}</p>
          <p><strong>Office Email:</strong> {previewData.officeEmail}</p>
          <p><strong>Department:</strong> {previewData.department}</p>
          <p><strong>Role:</strong> {previewData.role}</p>
          <p><strong>Manager:</strong> {previewData.manager}</p>
          <p><strong>Shift:</strong> {previewData.shift}</p>

          <div className="pt-6">
            <button
              onClick={handleSendDetailsToMail}
              className="w-full flex justify-center py-2 px-4 border border-transparent rounded-md shadow-sm text-lg font-medium text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 transition duration-200 ease-in-out"
            >
              Send Details to Personal Mail
            </button>
          </div>
        </div>
      )}
    </>
  );
};

export default PreviewPage;
