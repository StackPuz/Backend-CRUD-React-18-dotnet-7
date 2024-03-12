import React, { useState, useEffect } from 'react'
import { Link } from 'react-router-dom'
import http from '../../http'

export default function Login(props) {
  const [ user, setUser ] = useState({})
  const [ error, setError ] = useState(null)
  let { user: loginUser, history } = props

  useEffect(() => {
    if (loginUser) {
      history.push(history.redirect || '/home')
    }
  }, [ loginUser, history ])
  function onChange(e) {
    let data = { ...user }
    data[e.target.name.toLowerCase()] = e.target.value
    setUser(data)
  }
  function login(e) {
    e.preventDefault()
    http.post('/login', user).then(response => {
      props.setUser(response.data.user)
      localStorage.setItem('dotnet_token', response.data.token)
    }).catch((e) => {
      setError(e.response.data)
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
                  <h3>Login</h3>
                </div>
                <div className="card-body">
                  <i className="login fa fa-user-circle"></i>
                  <form method="post" onSubmit={login}>
                    <div className="row">
                      <div className="form-group col-12">
                        <label htmlFor="user_account_name">User Name</label>
                        <input id="user_account_name" name="Name" className="form-control form-control-sm" onChange={onChange} value={user.name || '' } required maxLength="50" />
                      </div>
                      <div className="form-group col-12">
                        <label htmlFor="user_account_password">Password</label>
                        <input id="user_account_password" name="Password" className="form-control form-control-sm" onChange={onChange} value={user.password || '' } type="password" required maxLength="100" />
                      </div>
                      <div className="col-12">
                        <button className="btn btn-sm btn-secondary btn-block">Login</button>
                        <Link to="/resetPassword">Forgot Password?</Link>
                      </div>
                    </div>
                  </form>
                  { error && <span className="text-danger">{ error.message }</span> }
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}