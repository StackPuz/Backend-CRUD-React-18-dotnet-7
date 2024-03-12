import http from '../../http'

export default {
  get(id) {
    if (id) {
      return http.get(`/brands/${id}`)
    }
    else {
      return http.get('/brands' + location.search)
    }
  },
  create(data) {
    if (data) {
      return http.post('/brands', data)
    }
    else {
      return http.get('/brands/create')
    }
  },
  edit(id, data) {
    if (data) {
      return http.put(`/brands/${id}`, data)
    }
    else {
      return http.get(`/brands/${id}/edit`)
    }
  },
  delete(id, data) {
    if (data) {
      return http.delete(`/brands/${id}`)
    }
    else {
      return http.get(`/brands/${id}/delete`)
    }
  }
}