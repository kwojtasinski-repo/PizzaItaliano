import { useState } from "react";
import { Nav } from "react-bootstrap";
import Link from "../Link/Link";
import { DropdownMenu } from "./DropdownMenu/DropdownMenu";
import style from "./Menu.module.css"

function Menu() {
    const [menuOpened, setMenuOpened] = useState(false);
    
    const onClick = (event) => {
        if (event.detail === 1 && menuOpened > 0) {
            setMenuOpened(false);
        }
    }

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
                <li className={style.menuItem}>
                    <Link to='/' className={`${style.menuItem}`}>
                        Register
                    </Link>
                </li>
                <li className={style.menuItem}>
                    <Link to='/' className={`${style.menuItem}`}>
                        Login
                    </Link>
                </li>
                <li className={style.menuItem}>
                    <Link to='/' className={`${style.menuItem}`}>
                        My Orders
                    </Link>
                    <DropdownMenu classNameMenu = {style.menuItem}
                        mainTitle = "My Orders"
                        navItems = {
                            <>
                            <Nav.Link as={Link} to="/" className={`${style.menuItemDropdown}`}>
                                Cart
                            </Nav.Link>
                            <Nav.Link as={Link} end to="/" className={`${style.menuItemDropdown}`}>
                                Ordered
                            </Nav.Link>
                            </>
                        } />
                </li>
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
            </ul>
        </nav>
    )
}

export default Menu;