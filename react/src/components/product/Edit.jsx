import React, { useState, useEffect } from 'react'
import { Link } from 'react-router-dom'
import Service from './Service'
import Util from '../../util'

export default function ProductEdit(props) {
  
  const [ product, setProduct ] = useState({})
  const [ brands, setBrands ] = useState([])
  const [ errors, setErrors ] = useState({})
  
  useEffect(() => {
    get().finally(() => {
      Util.initView(true)
    })
  }, [ props.match.params.id ])
  
  function get() {
    return Service.edit(props.match.params.id).then(response => {
      setProduct(response.data.product)
      setBrands(response.data.brands)
    })
  }

  function edit(e) {
    e.preventDefault()
    let data = { ...product }
    data.ImageFile = document.getElementsByName('ImageFile')[0].files[0]
    data = Util.getFormData(data)
    Service.edit(props.match.params.id, data).then(() => {
      props.history.push(Util.getRef('/product'))
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
    let data = { ...product }
    data[e.target.name] = e.target.value
    setProduct(data)
  }

  return (
    <div className="container">
      <div className="row">
        <div className="col">
          <form method="post" onSubmit={edit} encType="multipart/form-data">
            <div className="row">
              <div className="form-group col-md-6 col-lg-4">
                <label htmlFor="product_id">Id</label>
                <input readOnly id="product_id" name="Id" className="form-control form-control-sm" onChange={onChange} value={product.Id || '' } type="number" required />
                {errors.Id && <span className="text-danger">{errors.Id}</span>}
              </div>
              <div className="form-group col-md-6 col-lg-4">
                <label htmlFor="product_name">Name</label>
                <input id="product_name" name="Name" className="form-control form-control-sm" onChange={onChange} value={product.Name || '' } required maxLength="50" />
                {errors.Name && <span className="text-danger">{errors.Name}</span>}
              </div>
              <div className="form-group col-md-6 col-lg-4">
                <label htmlFor="product_price">Price</label>
                <input id="product_price" name="Price" className="form-control form-control-sm" onChange={onChange} value={product.Price || '' } type="number" step="0.1" required />
                {errors.Price && <span className="text-danger">{errors.Price}</span>}
              </div>
              <div className="form-group col-md-6 col-lg-4">
                <label htmlFor="product_brand_id">Brand</label>
                <select id="product_brand_id" name="BrandId" className="form-control form-control-sm" onChange={onChange} value={product.BrandId || '' } required>
                  <option></option>
                  {brands.map((brand, index) =>
                  <option key={index} value={brand.Id}>{brand.Name}</option>
                  )}
                </select>
                {errors.BrandId && <span className="text-danger">{errors.BrandId}</span>}
              </div>
              <div className="form-group col-md-6 col-lg-4">
                <label htmlFor="product_image">Image</label>
                <input type="file" accept="image/*" id="product_image" name="ImageFile" className="form-control form-control-sm" maxLength="50" />
                <a href={`http://localhost:5000/uploads/products/${product.Image || '' }`} target="_blank" rel="noreferrer" title={`${product.Image || '' }`}><img className="img-item" src={`http://localhost:5000/uploads/products/${product.Image || '' }`} /></a>
                {errors.Image && <span className="text-danger">{errors.Image}</span>}
              </div>
              <div className="col-12">
                <Link className="btn btn-sm btn-secondary" to={Util.getRef('/product')}>Cancel</Link>
                <button className="btn btn-sm btn-primary">Submit</button>
              </div>
            </div>
          </form>
        </div>
      </div>
    </div>
  )
}