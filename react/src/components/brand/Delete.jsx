import React, { useState, useEffect } from 'react'
import { Link } from 'react-router-dom'
import Service from './Service'
import Util from '../../util'

export default function BrandDelete(props) {
  
  const [ brand, setBrand ] = useState({})
  
  useEffect(() => {
    get().finally(() => {
      Util.initView(true)
    })
  }, [ props.match.params.id ])
  
  function get() {
    return Service.delete(props.match.params.id).then(response => {
      setBrand(response.data.brand)
    })
  }

  function remove(e) {
    e.preventDefault()
    Service.delete(props.match.params.id, brand).then(() => {
      props.history.push(Util.getRef('/brand'))
    }).catch((e) => {
      alert(e.response.data.message)
    })
  }

  return (
    <div className="container">
      <div className="row">
        <div className="col">
          <form method="post" onSubmit={remove}>
            <div className="row">
              <div className="form-group col-md-6 col-lg-4">
                <label htmlFor="brand_id">Id</label>
                <input readOnly id="brand_id" name="Id" className="form-control form-control-sm" value={brand.Id || '' } type="number" required />
              </div>
              <div className="form-group col-md-6 col-lg-4">
                <label htmlFor="brand_name">Name</label>
                <input readOnly id="brand_name" name="Name" className="form-control form-control-sm" value={brand.Name || '' } required maxLength="50" />
              </div>
              <div className="col-12">
                <Link className="btn btn-sm btn-secondary" to={Util.getRef('/brand')}>Cancel</Link>
                <button className="btn btn-sm btn-danger">Delete</button>
              </div>
            </div>
          </form>
        </div>
      </div>
    </div>
  )
}