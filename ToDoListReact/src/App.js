import React, { useEffect, useState } from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import service from './service.js';
import Login from './Login.js';
import Register from './Register.js';

// =========================================
// דף הבית - רשימת המשימות
// =========================================
function TodoApp() {
  const [newTodo, setNewTodo] = useState("");
  const [todos, setTodos] = useState([]);

  async function getTodos() {
    const todos = await service.getTasks();
    setTodos(todos);
  }

  async function createTodo(e) {
    e.preventDefault();
    await service.addTask(newTodo);
    setNewTodo("");
    await getTodos();
  }

  async function updateCompleted(todo, isComplete) {
    await service.setCompleted(todo.id, isComplete);
    await getTodos();
  }

  async function deleteTodo(id) {
    await service.deleteTask(id);
    await getTodos();
  }

  useEffect(() => {
    getTodos();
  }, []);

  return (
    <section className="todoapp">
      <header className="header">
        <h1>todos</h1>
        <button
          onClick={service.logout}
          style={{ float: "left", padding: "5px 10px", cursor: "pointer" }}
        >
          התנתק
        </button>
        <form onSubmit={createTodo}>
          <input
            className="new-todo"
            placeholder="Well, let's take on the day"
            value={newTodo}
            onChange={(e) => setNewTodo(e.target.value)}
          />
        </form>
      </header>
      <section className="main" style={{ display: "block" }}>
        <ul className="todo-list">
          {todos.map(todo => (
            <li className={todo.isComplete ? "completed" : ""} key={todo.id}>
              <div className="view">
                <input
                  className="toggle"
                  type="checkbox"
                  defaultChecked={todo.isComplete}
                  onChange={(e) => updateCompleted(todo, e.target.checked)}
                />
                <label>{todo.name}</label>
                <button className="destroy" onClick={() => deleteTodo(todo.id)}></button>
              </div>
            </li>
          ))}
        </ul>
      </section>
    </section>
  );
}

// =========================================
// Protected Route - מגן על דפים מאחורי התחברות
// =========================================
function ProtectedRoute({ children }) {
  const token = localStorage.getItem("token");
  return token ? children : <Navigate to="/login" />;
}

// =========================================
// App עם ניתוב
// =========================================
function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />
        <Route
          path="/"
          element={
            <ProtectedRoute>
              <TodoApp />
            </ProtectedRoute>
          }
        />
      </Routes>
    </BrowserRouter>
  );
}

export default App;