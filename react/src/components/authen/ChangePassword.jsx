import React, { useState, useEffect } from 'react'
import Util from '../../util'
import http from '../../http'

export default function changePassword(props) {
  const [ user, setUser ] = useState({})
  const [ result, setResult ] = useState({})
  let { history } = props

  useEffect(() => {
    http.get('/changePassword/' + props.match.params.token, user).catch((e) => {
      alert('Token not found!')
      history.push('/login')
    })
  }, [ history ])
  function onChange(e) {
    let data = { ...user }
    data[e.target.name.toLowerCase()] = e.target.value
    setUser(data)
  }
  function changePassword(e) {
    e.preventDefault()
    if (!Util.validateForm()) {
      return
    }
    http.post('/changePassword/' + props.match.params.token, user).then(response => {
      setResult({ success: true })
    }).catch((e) => {
      setResult({ error: true })
    })
  }

  return (
    <div className="container">
      <div className="row">
        <div className="col">
          <div className="center-container">
            <div className="row justify-content-center">
              <div className="card card-width">
                <div className="card-header">
                  <h3>Change Password</h3>
                </div>
                <div className="card-body">
                  <i className="login fa fa-user-circle"></i>
                  <form method="post" onSubmit={changePassword}>
                    <div className="row">
                      <div className="form-group col-12">
                        <label htmlFor="user_account_password">Password</label>
                        <input id="user_account_password" name="Password" className="form-control form-control-sm" onChange={onChange} value={user.password || '' } type="password" required maxLength="100" />
                      </div>
                      <div className="form-group col-12">
                        <label htmlFor="user_account_password2">Confirm password</label>
                        <input data-match="user_account_password" id="user_account_password2" name="Password2" className="form-control form-control-sm" type="password" required maxLength="100" />
                      </div>
                      <div className="col-12">
                        <button className="btn btn-sm btn-secondary btn-block">Change Password</button>
                      </div>
                    </div>
                  </form>
                  { result.success && <span className="text-success">Change password done</span> }
                  { result.error && <span className="text-danger">Token not found!</span> }
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}