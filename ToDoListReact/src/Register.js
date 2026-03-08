import React, { useState } from 'react';
import service from './service.js';

function Register() {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  async function handleRegister(e) {
    e.preventDefault();
    setError("");
    setSuccess("");
    try {
      await service.register(username, password);
      setSuccess("נרשמת בהצלחה! מעביר לדף ההתחברות...");
      setTimeout(() => { window.location.href = "/login"; }, 1500);
    } catch (err) {
      setError("שם המשתמש כבר קיים או שאירעה שגיאה");
    }
  }

  return (
    <section className="todoapp">
      <header className="header">
        <h1>הרשמה</h1>
      </header>
      <section className="main" style={{ display: "block", padding: "20px" }}>
        <form onSubmit={handleRegister}>
          <input
            className="new-todo"
            placeholder="שם משתמש"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            style={{ marginBottom: "10px" }}
          />
          <input
            className="new-todo"
            type="password"
            placeholder="סיסמה"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            style={{ marginBottom: "10px" }}
          />
          {error && <p style={{ color: "red" }}>{error}</p>}
          {success && <p style={{ color: "green" }}>{success}</p>}
          <button type="submit" style={{ padding: "10px 20px", cursor: "pointer" }}>
            הרשמה
          </button>
          <p style={{ marginTop: "15px" }}>
            כבר יש לך חשבון? <a href="/login">התחברות</a>
          </p>
        </form>
      </section>
    </section>
  );
}

export default Register;