import React, { useState, useEffect } from 'react'
import http from '../../http'

export default function ResetPassword(props) {
  const [ user, setUser ] = useState({})
  const [ result, setResult ] = useState({})

  function onChange(e) {
    let data = { ...user }
    data[e.target.name.toLowerCase()] = e.target.value
    setUser(data)
  }
  function resetPassword(e) {
    e.preventDefault()
    http.post('/resetPassword', user).then(response => {
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
                  <h3>Reset Password</h3>
                </div>
                <div className="card-body">
                  <i className="login fa fa-user-circle"></i>
                  <form method="post" onSubmit={resetPassword}>
                    <div className="row">
                      <div className="form-group col-12">
                        <label htmlFor="user_account_email">Email</label>
                        <input id="user_account_email" name="Email" className="form-control form-control-sm" onChange={onChange} value={user.email || '' } type="email" required maxLength="50" />
                      </div>
                      <div className="col-12">
                        <button className="btn btn-sm btn-secondary btn-block">Reset Password</button>
                      </div>
                    </div>
                  </form>
                  { result.success && <span className="text-success">A reset password link has been sent to your email</span> }
                  { result.error && <span className="text-danger">Email not found!</span> }
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}