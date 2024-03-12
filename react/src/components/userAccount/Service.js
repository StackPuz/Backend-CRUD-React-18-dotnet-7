import http from '../../http'

export default {
  get(id) {
    if (id) {
      return http.get(`/userAccounts/${id}`)
    }
    else {
      return http.get('/userAccounts' + location.search)
    }
  },
  create(data) {
    if (data) {
      return http.post('/userAccounts', data)
    }
    else {
      return http.get('/userAccounts/create')
    }
  },
  edit(id, data) {
    if (data) {
      return http.put(`/userAccounts/${id}`, data)
    }
    else {
      return http.get(`/userAccounts/${id}/edit`)
    }
  },
  delete(id, data) {
    if (data) {
      return http.delete(`/userAccounts/${id}`)
    }
    else {
      return http.get(`/userAccounts/${id}/delete`)
    }
  }
}