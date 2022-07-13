import { NavLink, Outlet } from "react-router-dom";

function Profile(props) {
    return (
        <div className="card">
            <div className="card-header">
                <h2>Profile</h2>
            </div>
            <div className="card-body">
                <ul className="nav nav-tabs">
                    <li className="nav-item">
                        <NavLink className="nav-link" end to="/profile" >Profile</NavLink>
                    </li>
                </ul>

                <div className="pt-4">
                    <Outlet />
                </div>
            </div>
        </div>
    )
}

export default Profile;