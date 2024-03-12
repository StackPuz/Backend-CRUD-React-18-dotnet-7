import React, { useState, useEffect } from 'react'
import http from '../http'
import Util from '../util'

export default function Profile(props) {
  
  const [ userAccount, setUserAccount ] = useState({})
  const [ errors, setErrors ] = useState({})
  
  useEffect(() => {
    get().finally(() => {
      Util.initView(true)
    })
  }, [])
  
  function get() {
    return http.get('/profile').then(response => {
      setUserAccount(response.data.userAccount)
    })
  }

  function edit(e) {
    e.preventDefault()
    if (!Util.validateForm()) {
      return
    }
    http.post('/updateProfile', userAccount).then(() => {
      props.history.push('/home')
    }).catch((e) => {
      if (e.response.data.errors) {
        setErrors(e.response.data.errors)
      }
      else {
        alert(e.response.data.message)
      }
    })
  }

  function onChange(e) {
    let data = { ...userAccount }
    data[e.target.name] = e.target.value
    setUserAccount(data)
  }

  return (
    <div className="container">
      <div className="row">
        <div className="col">
          <form method="post" onSubmit={edit}>
            <div className="row">
              <div className="form-group col-md-6 col-lg-4">
                <label htmlFor="user_account_name">Name</label>
                <input id="user_account_name" name="Name" className="form-control form-control-sm" onChange={onChange} value={userAccount.Name || '' } required maxLength="50" />
                {errors.Name && <span className="text-danger">{errors.Name}</span>}
              </div>
              <div className="form-group col-md-6 col-lg-4">
                <label htmlFor="user_account_email">Email</label>
                <input id="user_account_email" name="Email" className="form-control form-control-sm" onChange={onChange} value={userAccount.Email || '' } type="email" required maxLength="50" />
                {errors.Email && <span className="text-danger">{errors.Email}</span>}
              </div>
              <div className="form-group col-md-6 col-lg-4">
                <label htmlFor="user_account_password">Password</label>
                <input id="user_account_password" name="Password" className="form-control form-control-sm" onChange={onChange} value={userAccount.Password || '' } type="password" placeholder="New password" maxLength="100" />
                {errors.Password && <span className="text-danger">{errors.Password}</span>}
              </div>
              <div className="form-group col-md-6 col-lg-4">
                <label htmlFor="user_account_password2">Confirm password</label>
                <input data-match="user_account_password" id="user_account_password2" name="Password2" className="form-control form-control-sm" type="password" placeholder="New password" maxLength="100" />
                {errors.Password && <span className="text-danger">{errors.Password}</span>}
              </div>
              <div className="col-12">
                <button className="btn btn-sm btn-primary">Submit</button>
              </div>
            </div>
          </form>
        </div>
      </div>
    </div>
  )
}