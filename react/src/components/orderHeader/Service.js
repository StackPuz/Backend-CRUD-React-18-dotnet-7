import http from '../../http'

export default {
  get(id) {
    if (id) {
      return http.get(`/orderHeaders/${id}`)
    }
    else {
      return http.get('/orderHeaders' + location.search)
    }
  },
  create(data) {
    if (data) {
      return http.post('/orderHeaders', data)
    }
    else {
      return http.get('/orderHeaders/create')
    }
  },
  edit(id, data) {
    if (data) {
      return http.put(`/orderHeaders/${id}`, data)
    }
    else {
      return http.get(`/orderHeaders/${id}/edit`)
    }
  },
  delete(id, data) {
    if (data) {
      return http.delete(`/orderHeaders/${id}`)
    }
    else {
      return http.get(`/orderHeaders/${id}/delete`)
    }
  }
}