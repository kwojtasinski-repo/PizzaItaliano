import axios from "../axios-setup";

export default async function sendHttpRequest(url, method, payload) {
  let data = null;
  let error = '';

  try {
    const response = await axios.request({
        data: payload,
        method,
        url,
    });
    data = response.data;
  } catch (exception) {
    error = exception;
  }

  return [ data, error ];
}