import axios from 'axios'
import history from './history'

let http = axios.create({
  baseURL: 'http://localhost:5000/api',
  headers: {
    'Content-type': 'application/json'
  }
})
http.interceptors.request.use(config => {
  let token = localStorage.getItem('dotnet_token')
  config.headers['Authorization'] = `Bearer ${token}`
  return config
})
http.interceptors.response.use(response => response, (error) => {
  if (!error.response.data || (!error.response.data.message && !error.response.data.errors)) {
    error.response.data = {
      message: error.response.statusText
    }
  }
  if (error.response.status == 401 && !location.hash) {
    if (history.location.pathname != '/login' && history.location.pathname != '/logout' && history.location.pathname != '/') {
      history.redirect = history.location.pathname
    }
    history.push('/logout')
  }
  return Promise.reject(error)
})

export default http