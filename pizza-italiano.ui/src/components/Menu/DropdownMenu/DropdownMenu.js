import { useEffect, useState } from 'react';
import { Nav, NavDropdown } from 'react-bootstrap';
import { createGuid } from '../../../helpers/createGuid';

export function DropdownMenu(props) {
    // eslint-disable-next-line no-unused-vars
    const [id, setId] = useState(createGuid('N'));
    const [menuOpened, setMenuOpened] = useState(false);

    const clickMenuHandler = () => {
        setMenuOpened(!menuOpened);
    }
    
    const onClick = (event) => {
        setMenuOpened(false);
    }

    useEffect(() => {
        document.body.addEventListener('click', function(e) {
            if (e.target.id !== id) {
                onClick(e);
            }
        });
    },)
    
    return (
        <Nav className={props.classNameMenu}
                onClick={clickMenuHandler}>
            <NavDropdown
                id={id}
                title={props.mainTitle}
                show={menuOpened}>
                    {props.navItems}
            </NavDropdown>
        </Nav>
    )
}
