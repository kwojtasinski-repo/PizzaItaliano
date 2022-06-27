import React from 'react';

const AuthContext = React.createContext({
    user: null,
    login: () => {},
    logout: () => {}
});

AuthContext.displayName = "AuthContext";

export default AuthContext;