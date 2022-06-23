import React, {Suspense} from "react";
import { BrowserRouter as Router, Route, Routes, } from 'react-router-dom';
import "./App.css";
import Footer from "./components/Footer/Footer";
import Header from "./components/Header/Header";
import Layout from "./components/Layout/Layout";
import Menu from "./components/Menu/Menu";
import Home from "./pages/Home/Home";

function App() {
  const header = (
    <Header />
  );

  const menu = (
    <Menu />
  )

  const content = (
    <Suspense fallback={<p>Loading...</p>} >
      <Routes>
          <Route path="/" end element = {<Home />} />
      </Routes>
    </Suspense>
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
