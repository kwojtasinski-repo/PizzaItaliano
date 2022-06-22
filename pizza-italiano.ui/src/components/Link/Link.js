import { useState } from 'react';
import style from './Link.module.css';
import { NavLink } from 'react-router-dom';

export default function Link(props) {
    const [onHover, setOnHover] = useState('');

    const onMouseEnter = () => {
        setOnHover(style.onHover)
    };

    const onMouseLeave = () => setOnHover('');

    return (
        <NavLink onMouseEnter={onMouseEnter} onMouseLeave={onMouseLeave} to={props.to} className={`${props.className} ${onHover}`} onClick={props.onClick} >
            {props.children}
        </NavLink>
    );
}