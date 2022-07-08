import React, {Suspense, useReducer} from "react";
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
import AuthContext from './context/AuthContext';
import { initialState, reducer } from './reducer';
import ReducerContext from './context/ReducerContext';
import AddOrderProduct from "./pages/Order/AddOrderProduct/AddOrderProduct";
import RequireAuth from "./hoc/RequireAuth";
import Register from "./pages/Auth/Register/Register";
import Login from "./pages/Auth/Login/Login";
import MyOrders from "./pages/MyOrders/MyOrders";

function App() {
  const [state, dispatch] = useReducer(reducer, initialState);

  const header = (
    <Header />
  );

  const menu = (
    <Menu />
  )

  const content = (
    <Suspense fallback={<p>Loading...</p>} >
      <Routes>
          <Route path='/products/details/:id' element = { <RequireAuth> <ViewProduct /> </RequireAuth> } />
          <Route path='/products/edit/:id' element = { <RequireAuth> <EditProduct /> </RequireAuth> } />
          <Route path='/products/add' element = {<RequireAuth> <AddProduct /> </RequireAuth>} />
          <Route path='/releases/:id' element = { <Releases /> } />
          <Route path='/payments' element = { <Payments /> } />
          <Route path='/orders/my' element = { <RequireAuth> <MyOrders/> </RequireAuth> } />
          <Route path='/orders/:id' element = { <Order /> } >
            <Route path='add-product' element = { <AddOrderProduct /> }/>
          </Route>
          <Route path='/cart' element = { <RequireAuth> <Cart /> </RequireAuth> } />
          <Route path='/login' element = {<Login />} />
          <Route path='/register' element = {<Register />} />
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
      <AuthContext.Provider value = {{
          user: state.user,
          login: (user) => dispatch({ type: "login", user }),
          logout: () => dispatch({ type: "logout" })
      }}>
        <ReducerContext.Provider value={{
          state: state,
          dispatch: dispatch
        }} >
          <Layout
              header = {header}
              menu = {menu}
              content = {content}
              footer = {footer}
              />
        </ReducerContext.Provider>
      </AuthContext.Provider>
    </Router>
  );
}

export default App;
