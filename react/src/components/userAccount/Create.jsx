import React, { useState, useEffect } from 'react'
import { Link } from 'react-router-dom'
import Service from './Service'
import Util from '../../util'

export default function UserAccountCreate(props) {
  
  const [ userAccount, setUserAccount ] = useState({})
  const [ roles, setRoles ] = useState([])
  const [ errors, setErrors ] = useState({})
  
  useEffect(() => {
    get().finally(() => {
      Util.initView(true)
    })
  }, [])
  
  function get() {
    return Service.create().then(response => {
      setRoles(response.data.roles)
    })
  }

  function create(e) {
    e.preventDefault()
    let data = { ...userAccount }
    data.RoleId = Array.from(document.querySelectorAll('[name="RoleId"]:checked')).map(e => e.value)
    data = Util.getFormData(data)
    Service.create(data).then(() => {
      props.history.push(Util.getRef('/userAccount'))
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
    data[e.target.name] = (e.target.type == 'checkbox'? e.target.checked : e.target.value)
    setUserAccount(data)
  }

  return (
    <div className="container">
      <div className="row">
        <div className="col">
          <form method="post" onSubmit={create}>
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
              <div className="form-check col-md-6 col-lg-4">
                <input id="user_account_active" name="Active" className="form-check-input" type="checkbox" onChange={onChange} value="1" checked={userAccount.Active || '' } />
                <label className="form-check-label" htmlFor="user_account_active">Active</label>
                {errors.Active && <span className="text-danger">{errors.Active}</span>}
              </div>
              <div className="col-12">
                <h6>
                </h6><label>Roles</label>
                {roles.map((role, index) =>
                <div key={index} className="form-check">
                  <input id={`user_role_role_id${role.Id}`} name="RoleId" className="form-check-input" type="checkbox" value={role.Id} />
                  <label className="form-check-label" htmlFor={`user_role_role_id${role.Id}`}>{role.Name}</label>
                </div>
                )}
              </div>
              <div className="col-12">
                <Link className="btn btn-sm btn-secondary" to={Util.getRef('/userAccount')}>Cancel</Link>
                <button className="btn btn-sm btn-primary">Submit</button>
              </div>
            </div>
          </form>
        </div>
      </div>
    </div>
  )
}