import React, { useState, useEffect } from 'react'
import { Link } from 'react-router-dom'
import Util from '../../util'
import Service from './Service'

export default function BrandIndex(props) {

  const [brands, setBrands] = useState([])
  const [paging, setPaging] = useState({
    current: 1,
    size: 1,
    last: 1
  })

  useEffect(() => {
    Util.initView()
  }, [])

  useEffect(() => {
    get()
  }, [props.location])

  function get() {
    Service.get().then(response => {
      let query = Util.getQuery()
      setBrands(response.data.brands)
      setPaging({
        current: parseInt(query.page) || 1,
        size: parseInt(query.size) || 10,
        last: response.data.last
      })
    }).catch(e => {
      alert(e.response.data.message)
    })
  }

  return (
    <div className="container">
      <div className="row">
        <div className="col">
          <div className="col-12"><input id="searchbar_toggle" type="checkbox" />
            <div id="searchbar" className="form-row mb-4">
              <div className="form-group col-lg-2">
                <select id="search_col" onChange={()=> Util.searchChange()} className="form-control form-control-sm">
                  <option value="Id" data-type="number">Brand Id</option>
                  <option value="Name">Brand Name</option>
                </select>
              </div>
              <div className="form-group col-lg-2">
                <select id="search_oper" className="form-control form-control-sm">
                  <option value="c">Contains</option>
                  <option value="e">Equals</option>
                  <option value="g">&gt;</option>
                  <option value="ge">&gt;&#x3D;</option>
                  <option value="l">&lt;</option>
                  <option value="le">&lt;&#x3D;</option>
                </select>
              </div>
              <div className="form-group col-lg-2">
                <input id="search_word" autoComplete="off" onKeyUp={event=> Util.search(event)} className="form-control form-control-sm" />
              </div>
              <div className="col">
                <button className="btn btn-primary btn-sm" onClick={()=> Util.search()}>Search</button>
                <button className="btn btn-secondary btn-sm" onClick={()=> Util.clearSearch()}>Clear</button>
              </div>
            </div>
            <table className="table table-sm table-striped table-hover">
              <thead>
                <tr>
                  <th className={Util.getSortClass('Id','asc')}>
                    <Link to={Util.getLink(paging,'sort','Id','asc')}>Id</Link>
                  </th>
                  <th className={Util.getSortClass('Name')}>
                    <Link to={Util.getLink(paging,'sort','Name')}>Name</Link>
                  </th>
                  <th>Actions</th>
                </tr>
              </thead>
              <tbody>
                {brands.map((brand, index) =>
                <tr key={index}>
                  <td className="text-center">{brand.Id}</td>
                  <td>{brand.Name}</td>
                  <td className="text-center">
                    <Link className="btn btn-sm btn-secondary" to={`/brand/${brand.Id}`} title="View"><i className="fa fa-eye"></i></Link>
                    <Link className="btn btn-sm btn-primary" to={`/brand/edit/${brand.Id}`} title="Edit"><i className="fa fa-pencil"></i></Link>
                    <Link className="btn btn-sm btn-danger" to={`/brand/delete/${brand.Id}`} title="Delete"><i className="fa fa-times"></i></Link>
                  </td>
                </tr>
                )}
              </tbody>
            </table>
            <div className="row mb-1">
              <div className="col-md-3 col-6">
                <label>Show
                  <select id="page_size" onChange={event=> props.history.push(event.target.value)} value={Util.getLink(paging,'size',paging.size)}>
                    <option value={Util.getLink(paging,'size',10)}>10</option>
                    <option value={Util.getLink(paging,'size',20)}>20</option>
                    <option value={Util.getLink(paging,'size',30)}>30</option>
                  </select>
                  entries
                </label>
              </div>
              <div className="col-md-9 col-6">
                <div className="float-right d-none d-md-block">
                  <ul className="pagination flex-wrap">
                    <li className={`page-item${paging.current <=1 ? ' disabled' : '' }`}>
                      <Link className="page-link" to={Util.getLink(paging,'page',paging.current-1)}>Prev</Link>
                    </li>
                    {Util.getPages(paging.last).map(page => 
                    <li key={page} className={`page-item${paging.current==page ? ' active' : '' }`}>
                      <Link className="page-link" to={Util.getLink(paging,'page',page)}>{page}</Link>
                    </li>
                    )}
                    <li className={`page-item${paging.current>= paging.last ? ' disabled' : ''}`}>
                      <Link className="page-link" to={Util.getLink(paging,'page',paging.current+1)}>Next</Link>
                    </li>
                  </ul>
                </div>
                <div className="float-right d-block d-md-none">
                  <label> Page
                    <select id="page_size" onChange={event=> props.history.push(event.target.value)} value={Util.getLink(paging,'page', paging.current)}>
                      {Util.getPages(paging.last).map(page => 
                      <option key={page} value={Util.getLink(paging,'page',page)}>{page}</option>
                      )}
                    </select>
                  </label> of <span>{paging.last}</span>
                  <div className="btn-group">
                    <Link className={` btn btn-primary btn-sm${paging.current <=1 ? ' disabled' : '' }`} to={Util.getLink(paging,'page',paging.current-1)}><i className="fa fa-chevron-left"></i></Link>
                    <Link className={` btn btn-primary btn-sm${paging.current>= paging.last ? ' disabled' : ''}`} to={Util.getLink(paging,'page',paging.current+1)}><i className="fa fa-chevron-right"></i></Link>
                  </div>
                </div>
              </div>
            </div>
            <Link className="btn btn-sm btn-primary" to="/brand/create">Create</Link>
          </div>
          <style dangerouslySetInnerHTML={{ __html: '#searchbar_toggle_menu { display: inline-flex!important }' }} />
        </div>
      </div>
    </div>
  )
}