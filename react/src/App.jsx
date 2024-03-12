import React, { useState, useEffect } from 'react'
import { Router, Link } from 'react-router-dom'
import history from './history'
import http from './http'
import Route  from './router'

export default function App() {
  const [ isReady, setIsReady ] = useState(false)
  const [ user, setUser ] = useState(null)
  const [ path, setPath ] = useState(location.pathname)
  
  useEffect(() => {
    let unlisten = history.listen((location) => {
      setPath(location.pathname)
      history.prev = history.current
      history.current = location.pathname + location.search
    })
    http.get('/user').then(response => {
      setUser(response.data)
      setIsReady(true)
    }).catch(() => {
      setIsReady(true)
    })
    return () => {
      unlisten()
    }
  }, [])
  
  if (!isReady) {
    return null
  }
  return (
    <Router history={history}>
      { user ?
      <div className="wrapper">
        <input id="sidebar_toggle" type="checkbox" />
        <nav id="sidebar">
          <Link to="/" className="bg-light border-bottom">
          <h4>StackPuz</h4>
          </Link>
          <ul className="list-unstyled">
            <li>
              <Link to="/home" className={path.endsWith('/home') ? 'active bg-primary' : '' }>Home</Link>
            </li>
            {user && user.menu.map((menu, index) =>
            <li key={index}>
              <Link to={`/${menu.path}`} className={path.substr(1).split('/')[0]==menu.path ? 'active bg-primary' : '' }>{menu.title}</Link>
            </li>
            )}
          </ul>
        </nav>
        <div id="body">
          <nav className="navbar bg-light border-bottom">
            <label htmlFor="sidebar_toggle" className=" btn btn-primary btn-sm"><i className="fa fa-bars"></i></label>
            <ul className="navbar-nav ml-auto">
              <li id="searchbar_toggle_menu" className="d-none">
                <a className="nav-link text-secondary" href="#"><label htmlFor="searchbar_toggle" className="d-lg-none"><i className="fa fa-search"></i></label></a>
              </li>
              <li className="dropdown">
                <a className="nav-link text-secondary dropdown-toggle" data-toggle="dropdown" href="#"><i className="fa fa-user"></i> <span className="d-none d-lg-inline"> {(user && user.name) || ''}</span></a>
                <div className="dropdown-menu dropdown-menu-right">
                  <Link to="/profile" className="dropdown-item"><i className="fa fa-user"></i> Profile</Link>
                  <Link to="/logout" className="dropdown-item"><i className="fa fa-sign-out"></i> Logout</Link>
                </div>
              </li>
            </ul>
          </nav>
          <div className="content">
            <Route user={user} setUser={setUser} />
          </div>
        </div>
      </div>
      :
      <Route user={user} setUser={setUser} />
      }
    </Router>
  )
}