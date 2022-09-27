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
import { PaymentByOrderId } from "./pages/Payments/Payment/PaymentByOrderId";
import { ReleasesByOrderId } from "./pages/Releases/ReleasesByOrderId";
import Orders from "./pages/Orders/Orders";
import Profile from "./pages/Profile/Profile";
import ProfileDetails from "./pages/Profile/ProfileDetails";
import Users from "./pages/Users/Users";
import EditUser from "./pages/Users/EditUser";
import Products from "./pages/Product/Products";

function App() {
  const [state, dispatch] = useReducer(reducer, initialState);

  const header = (
    <Header />
  );

  const menu = (
    <Menu />
  )

  // TODO: Profile for user and management users
  const content = (
    <Suspense fallback={<p>Loading...</p>} >
      <Routes>
          <Route path='/products/details/:id' element = { <RequireAuth> <ViewProduct /> </RequireAuth> } />
          <Route path='/products/edit/:id' element = { <RequireAuth> <EditProduct /> </RequireAuth> } />
          <Route path='/products/add' element = {<RequireAuth> <AddProduct /> </RequireAuth>} />
          <Route path='/products' element = { <RequireAuth> <Products /> </RequireAuth> } />
          <Route path='/releases/by-order/:id' element = { <RequireAuth> <ReleasesByOrderId /> </RequireAuth> } />
          <Route path='/releases' element = { <RequireAuth> <Releases /> </RequireAuth> } />
          <Route path='/payments/by-order/:id' element = { <RequireAuth> <PaymentByOrderId /> </RequireAuth> } />
          <Route path='/payments' element = { <RequireAuth> <Payments /> </RequireAuth> } />
          <Route path='/orders/my' element = { <RequireAuth> <MyOrders/> </RequireAuth> } />
          <Route path='/orders' element = { <RequireAuth> <Orders /> </RequireAuth> } />
          <Route path='/orders/:id' element = { <RequireAuth> <Order /> </RequireAuth> } >
            <Route path='add-product' element = { <RequireAuth> <AddOrderProduct /> </RequireAuth> }/>
          </Route>
          <Route path="user-management/edit/:id" element = { <RequireAuth> <EditUser /> </RequireAuth> }  />
          <Route path="user-management" element = { <RequireAuth> <Users/> </RequireAuth> }  />
          <Route path="profile" element = { <RequireAuth> <Profile/> </RequireAuth> }  >
            <Route path="" element = { <RequireAuth> <ProfileDetails/> </RequireAuth> } />
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
