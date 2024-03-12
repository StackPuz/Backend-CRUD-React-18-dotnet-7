import React, { useState, useEffect } from 'react'
import { Link } from 'react-router-dom'
import Service from './Service'
import Util from '../../util'

export default function ProductDelete(props) {
  
  const [ product, setProduct ] = useState({})
  
  useEffect(() => {
    get().finally(() => {
      Util.initView(true)
    })
  }, [ props.match.params.id ])
  
  function get() {
    return Service.delete(props.match.params.id).then(response => {
      setProduct(response.data.product)
    })
  }

  function remove(e) {
    e.preventDefault()
    Service.delete(props.match.params.id, product).then(() => {
      props.history.push(Util.getRef('/product'))
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
                <label htmlFor="product_id">Id</label>
                <input readOnly id="product_id" name="Id" className="form-control form-control-sm" value={product.Id || '' } type="number" required />
              </div>
              <div className="form-group col-md-6 col-lg-4">
                <label htmlFor="product_name">Name</label>
                <input readOnly id="product_name" name="Name" className="form-control form-control-sm" value={product.Name || '' } required maxLength="50" />
              </div>
              <div className="form-group col-md-6 col-lg-4">
                <label htmlFor="product_price">Price</label>
                <input readOnly id="product_price" name="Price" className="form-control form-control-sm" value={product.Price || '' } type="number" step="0.1" required />
              </div>
              <div className="form-group col-md-6 col-lg-4">
                <label htmlFor="brand_name">Brand</label>
                <input readOnly id="brand_name" name="brand_name" className="form-control form-control-sm" value={product.BrandName || '' } maxLength="50" />
              </div>
              <div className="form-group col-md-6 col-lg-4">
                <label htmlFor="user_account_name">Create User</label>
                <input readOnly id="user_account_name" name="user_account_name" className="form-control form-control-sm" value={product.UserAccountName || '' } maxLength="50" />
              </div>
              <div className="form-group col-md-6 col-lg-4"><label>Image</label>
                <div><a href={`http://localhost:5000/uploads/products/${product.Image || '' }`} target="_blank" rel="noreferrer" title={`${product.Image || '' }`}><img className="img-item" src={`http://localhost:5000/uploads/products/${product.Image || '' }`} /></a></div>
              </div>
              <div className="col-12">
                <Link className="btn btn-sm btn-secondary" to={Util.getRef('/product')}>Cancel</Link>
                <button className="btn btn-sm btn-danger">Delete</button>
              </div>
            </div>
          </form>
        </div>
      </div>
    </div>
  )
}