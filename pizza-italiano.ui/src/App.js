import React, {Suspense} from "react";
import { BrowserRouter as Router, Route, Routes, } from 'react-router-dom';
import "./App.css";
import Footer from "./components/Footer/Footer";
import Header from "./components/Header/Header";
import Layout from "./components/Layout/Layout";
import Menu from "./components/Menu/Menu";
import NotFound from "./pages/404/NotFound";
import Cart from "./pages/Cart/Cart";
import Home from "./pages/Home/Home";
import Order from "./pages/Order/Order";
import { Payments } from "./pages/Payments/Payments";
import AddProduct from "./pages/Product/AddProduct";
import EditProduct from "./pages/Product/EditProduct";
import ViewProduct from "./pages/Product/ViewProduct";
import { Releases } from "./pages/Releases/Releases";

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
          <Route path='/products/details/:id' element = { <ViewProduct /> } />
          <Route path='/products/edit/:id' element = { <EditProduct /> } />
          <Route path='/products/add' element = { <AddProduct /> } />
          <Route path='/releases/:id' element = { <Releases /> } />
          <Route path='/payments' element = { <Payments /> } />
          <Route path='/orders/:id' element = { <Order /> } />
          <Route path='/cart' element = { <Cart /> } />
          <Route path="/" end element = {<Home />} />
          <Route path="*" element = {<NotFound/>} />
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