import React, {Suspense} from "react";
import { BrowserRouter as Router, Route, Routes, } from 'react-router-dom';
import "./App.css";
import Footer from "./components/Footer/Footer";
import Header from "./components/Header/Header";
import Layout from "./components/Layout/Layout";
import Menu from "./components/Menu/Menu";
import { error, info, success, warning } from "./components/notifications";
import Home from "./pages/Home/Home";

function App() {
  <Suspense fallback={<p>Ładowanie...</p>} >
    <Routes>
        <Route path="/" end element = {<Home />} />
    </Routes>
  </Suspense>

  const header = (
    <Header />
  );

  const menu = (
    <Menu />
  )

  const content = (
    <div className="App">
      <h1>Notification Test</h1>
      <button className="btn btn-primary me-2" onClick={() => info("message", true)}>Info</button>
      <button className="btn btn-success me-2" onClick={() => success("message", true)}>Success</button>
      <button className="btn btn-warning me-2" onClick={() => warning("message", true)}>Warning</button>
      <button className="btn btn-danger" onClick={() => error("message", true)}>Error</button>
    </div>
  )

  const footer = (
    <Footer />
  )

  return (
    <Router>
      <Layout
          header = {header}
          menu = {menu}
          content = {content}
          footer = {footer}
          />
    </Router>
  );
}

export default App;
