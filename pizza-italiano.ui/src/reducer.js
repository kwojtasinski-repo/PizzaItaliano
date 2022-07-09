export const reducer = (state, action) => {
    switch(action.type) {
        case 'login' :
            return { ...state, user: action.user }
        case 'logout' : 
            return { ...state, user: null }
        case 'clear-cart':
            return { ...state, clearedCart: new Date() }
        default :
            throw new Error(`Action ${action.type} doesnt exists.`);
    }
}

export const initialState = {
    user: null,
    currentEvent: '',
    events: []
};