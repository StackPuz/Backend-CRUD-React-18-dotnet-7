import http from '../../http'

export default {
  get(orderId, no) {
    if (orderId) {
      return http.get(`/orderDetails/${orderId}/${no}`)
    }
    else {
      return http.get('/orderDetails' + location.search)
    }
  },
  create(data) {
    if (data) {
      return http.post('/orderDetails', data)
    }
    else {
      return http.get('/orderDetails/create')
    }
  },
  edit(orderId, no, data) {
    if (data) {
      return http.put(`/orderDetails/${orderId}/${no}`, data)
    }
    else {
      return http.get(`/orderDetails/${orderId}/${no}/edit`)
    }
  },
  delete(orderId, no, data) {
    if (data) {
      return http.delete(`/orderDetails/${orderId}/${no}`)
    }
    else {
      return http.get(`/orderDetails/${orderId}/${no}/delete`)
    }
  }
}