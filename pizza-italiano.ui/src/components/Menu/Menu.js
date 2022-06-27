import { useEffect, useState } from "react";
import useAuth from "../../hooks/useAuth";
import { getItemsInCartCount } from "../Cart/Cart";
import Link from "../Link/Link";
import style from "./Menu.module.css"

function Menu() {
    const [menuOpened, setMenuOpened] = useState(false);
    const [auth, setAuth] = useAuth();
    const [itemCount, setItemCount] = useState(0);
    
    const onClick = (event) => {
        if (event.detail === 1 && menuOpened > 0) {
            setMenuOpened(false);
        }
    }

    useEffect(() => {
        setItemCount(getItemsInCartCount());
    }, [])

    return (
        <nav className={`${style.menuContainer} navbar navbar-expand-lg navbar-light bg-light`} onClick={onClick} >
            <ul>
                <li className={style.menuItem}>
                    <Link to='/' className={style.menuItem} >
                        Home
                    </Link>
                </li>
                <li className={style.menuItem}>
                    <Link to='/' className={`${style.menuItem}`}>
                        Profile
                    </Link>
                </li>
                {auth ? 
                    <li className={style.menuItem}>
                        <Link to='/' className={`${style.menuItem}`} onClick={() => setAuth(null)}>
                            Logout
                        </Link>
                    </li>
                    : <>
                        <li className={style.menuItem}>
                            <Link to='/' className={`${style.menuItem}`}>
                                Register
                            </Link>
                        </li>
                        <li className={style.menuItem}>
                            <Link to='/' className={`${style.menuItem}`} onClick={() => setAuth({id:1,role:"user"})}>
                                Login
                            </Link>
                        </li>
                    </>
                }
                <li className={style.menuItem}>
                        <span className={`badge badge-light ${style.itemCartCount}`} style={{ color : "#206199"}}>{itemCount}</span>
                        <Link to='/cart' className={`${style.menuItem} ${style.cartPosition}`}>
                            Cart
                        </Link>
                </li>
                <li className={style.menuItem}>
                    <Link to='/' className={`${style.menuItem}`}>
                        My Orders
                    </Link>
                </li>
                {auth && auth.role !== "user" ?
                    <>
                        <li className={style.menuItem}>
                            <Link to='/' className={`${style.menuItem}`}>
                                Orders
                            </Link>
                        </li>
                        <li className={style.menuItem}>
                            <Link to='/' className={`${style.menuItem}`}>
                                Payments
                            </Link>
                        </li>
                        <li className={style.menuItem}>
                            <Link to='/' className={`${style.menuItem}`}>
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