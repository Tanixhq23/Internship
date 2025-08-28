import React, { useState } from 'react'; 
import '../../styles/attendanceCard.css';


const AttendanceCard = () => {
  const [isPunchedIn, setIsPunchedIn] = useState(false);
  const [punchTime, setPunchTime] = useState(null);
  
  const handlePunchIn = () => {
    if (!isPunchedIn) {
      setIsPunchedIn(true);
      setPunchTime(new Date().toLocaleTimeString());
    }
  };

  const handlePunchOut = () => {
    if (isPunchedIn) {
      setIsPunchedIn(false);
      setPunchTime(new Date().toLocaleTimeString());
    }
  };

  return (
    <div className="card-container attendance-card">
      <h3>Attendance</h3>
      <div className="card-content">
        <p className={`status ${isPunchedIn ? 'punched-in' : 'punched-out'}`}>
          <strong>Status:</strong> {isPunchedIn ? 'Punched In' : 'Punched Out'}
        </p>
        
        {punchTime && (
          <p className="punch-time">
            <strong>Time:</strong> {punchTime}
          </p>
        )}

        <div className="buttons">
          <button
            onClick={handlePunchIn}
            disabled={isPunchedIn} 
            className="punch-button punch-in"
          >
            Punch In
          </button>
          <button
            onClick={handlePunchOut}
            disabled={!isPunchedIn} 
            className="punch-button punch-out"
          >
            Punch Out
          </button>
        </div>
      </div>
    </div>
  );
};

export default AttendanceCard;
       