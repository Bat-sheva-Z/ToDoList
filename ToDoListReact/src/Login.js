import React, { useState } from 'react';
import service from './service.js';

function Login() {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  async function handleLogin(e) {
    e.preventDefault();
    setError("");
    try {
      await service.login(username, password);
      window.location.href = "/"; // מעבר לדף הראשי אחרי התחברות
    } catch (err) {
      setError("שם משתמש או סיסמה שגויים");
    }
  }

  return (
    <section className="todoapp">
      <header className="header">
        <h1>התחברות</h1>
      </header>
      <section className="main" style={{ display: "block", padding: "20px" }}>
        <form onSubmit={handleLogin}>
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
          <button type="submit" style={{ padding: "10px 20px", cursor: "pointer" }}>
            התחבר
          </button>
          <p style={{ marginTop: "15px" }}>
            אין לך חשבון? <a href="/register">הרשמה</a>
          </p>
        </form>
      </section>
    </section>
  );
}

export default Login;