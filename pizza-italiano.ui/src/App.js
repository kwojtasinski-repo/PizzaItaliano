import React from "react";
import "./App.css";
import { error, info, success, warning } from "./notifications";

function App() {
  return (
    <div className="App">
      <h1>Notification Test</h1>
      <button className="btn btn-primary me-2" onClick={() => info("message", true)}>Info</button>
      <button className="btn btn-success me-2" onClick={() => success("message", true)}>Success</button>
      <button className="btn btn-warning me-2" onClick={() => warning("message", true)}>Warning</button>
      <button className="btn btn-danger" onClick={() => error("message", true)}>Error</button>
    </div>
  );
}

export default App;
