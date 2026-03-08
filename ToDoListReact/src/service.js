import axios from 'axios';

// Config Default
axios.defaults.baseURL = "http://localhost:5119";

// Request Interceptor - הוספת JWT Token לכל בקשה
axios.interceptors.request.use(
  config => {
    const token = localStorage.getItem("token");
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  error => Promise.reject(error)
);

// Response Interceptor - תפיסת שגיאות
axios.interceptors.response.use(
  response => response,
  error => {
    console.error("API error:", error);

    // תפיסת שגיאה 401 - העברה לדף לוגין
    if (error.response?.status === 401) {
      localStorage.removeItem("token");
      window.location.href = "/login";
    }

    return Promise.reject(error);
  }
);

export default {
  // Auth
  register: async (username, password) => {
    const result = await axios.post('/register', { username, password });
    return result.data;
  },

  login: async (username, password) => {
    const result = await axios.post('/login', { username, password });
    const token = result.data.token;
    localStorage.setItem("token", token);
    return token;
  },

  logout: () => {
    localStorage.removeItem("token");
    window.location.href = "/login";
  },

  // Tasks
  getTasks: async () => {
    const result = await axios.get('/items');
    return result.data;
  },

  addTask: async (name) => {
    const result = await axios.post('/items', { Name: name, IsComplete: false });
    return result.data;
  },

  setCompleted: async (id, isComplete) => {
    const result = await axios.put(`/items/${id}?isComplete=${isComplete}`);
    return result.data;
  },

  deleteTask: async (id) => {
    await axios.delete(`/items/${id}`);
  }
};