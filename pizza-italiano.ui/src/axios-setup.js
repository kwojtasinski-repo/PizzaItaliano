import axios from 'axios';
import { mapExceptionToResponse } from './helpers/mapExceptionToResponse';

function getToken() {
    const auth = JSON.parse(window.localStorage.getItem('token-data'));
    return auth ? auth.accessToken : null;
}

const instance = axios.create({
    //baseURL: `${process.env.REACT_APP_BACKEND_URL}`
    baseURL: window._env_?.REACT_APP_BACKEND_URL ? window._env_.REACT_APP_BACKEND_URL : process.env.REACT_APP_BACKEND_URL
});

instance.interceptors.request.use((req) => {
    const token = getToken();
    if (token) {
        req.headers.Authorization = `Bearer ${token}`;
    }

    return req;
})

instance.interceptors.response.use(function (response) {
    return Promise.resolve(response);
  }, function (error) {
    const message = mapExceptionToResponse(error);
    console.log(message);
    return Promise.reject(message);
  });

export default instance;