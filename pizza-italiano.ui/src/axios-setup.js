import axios from 'axios';

function getToken() {
    const auth = JSON.parse(window.localStorage.getItem('token-data'));
    return auth ? auth.token : null;
}

const instance = axios.create({
    baseURL: `${process.env.REACT_APP_BACKEND_URL}`
});

instance.interceptors.request.use((req) => {
    const token = getToken();
    if (token) {
        req.headers.Authorization = `Bearer ${token}`;
    }

    return req;
})

axios.interceptors.response.use(function (response) {
    if (response.headers.location) {
        return axios.get(response.headers.location);
    }

    return Promise.resolve(response);
  }, function (error) {
    return Promise.reject(error);
  });

export default instance;