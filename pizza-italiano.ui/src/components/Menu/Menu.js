import { useContext, useEffect, useState } from "react";
import ReducerContext from "../../context/ReducerContext";
import useAuth from "../../hooks/useAuth";
import { getItemsInCartCount } from "../Cart/Cart";
import Link from "../Link/Link";
import { info } from "../notifications";
import style from "./Menu.module.css"

function Menu() {
    const [menuOpened, setMenuOpened] = useState(false);
    const [auth, setAuth] = useAuth();
    const [itemCount, setItemCount] = useState(0);
    const context = useContext(ReducerContext);
    
    const onClick = (event) => {
        if (event.detail === 1 && menuOpened > 0) {
            setMenuOpened(false);
        }
    }

    const logout = (event) => {
        event.preventDefault();
        setAuth();
        info("Logout", true);
    }

    useEffect(() => {
        setItemCount(getItemsInCartCount());
    }, [context.state.clearedCart])

    return (
        <nav className={`${style.menuContainer} navbar navbar-expand-lg navbar-light bg-light`} onClick={onClick} >
            <ul>
                <li className={style.menuItem}>
                    <Link to='/' className={style.menuItem} >
                        Home
                    </Link>
                </li>
                {auth ?
                    <li className={style.menuItem}>
                        <Link to='/profile' className={`${style.menuItem}`}>
                            Profile
                        </Link>
                    </li>
                : null}
                {auth ? 
                    <li className={style.menuItem}>
                        <Link to='/' className={`${style.menuItem}`} onClick={logout}>
                            Logout
                        </Link>
                    </li>
                    : <>
                        <li className={style.menuItem}>
                            <Link to='/register' className={`${style.menuItem}`}>
                                Register
                            </Link>
                        </li>
                        <li className={style.menuItem}>
                            <Link to='/login' className={`${style.menuItem}`}>
                                Login
                            </Link>
                        </li>
                    </>
                }
                {auth ? 
                    <>
                        <li className={style.menuItem}>
                                <span className={`badge badge-light ${style.itemCartCount}`} style={{ color : "#206199"}}>{itemCount}</span>
                                <Link to='/cart' className={`${style.menuItem} ${style.cartPosition}`}>
                                    Cart
                                </Link>
                        </li>
                        <li className={style.menuItem}>
                            <Link to='/orders/my' className={`${style.menuItem}`}>
                                My Orders
                            </Link>
                        </li>
                    </> 
                : null}
                {auth && auth.role !== "user" ?
                    <>
                        <li className={style.menuItem}>
                            <Link to='/orders' className={`${style.menuItem}`}>
                                Orders
                            </Link>
                        </li>
                        <li className={style.menuItem}>
                            <Link to='/payments' className={`${style.menuItem}`}>
                                Payments
                            </Link>
                        </li>
                        <li className={style.menuItem}>
                            <Link to='/releases' className={`${style.menuItem}`}>
                                Releases
                            </Link>
                        </li>
                    </>
                : null}
            </ul>
        </nav>
    )
}

export default Menu;