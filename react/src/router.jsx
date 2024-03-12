import React, { Suspense, lazy } from 'react'
import { Switch, Route } from 'react-router-dom'
import Login from './components/authen/Login'
import Logout from './components/authen/Logout'
import ResetPassword from './components/authen/ResetPassword'
import ChangePassword from './components/authen/ChangePassword'
import Profile from './components/Profile'
import Default from './components/Default'
import Home from './components/Home'
import NotFound from './components/NotFound'

export default function AppRoute(props) {
  return (
    <Suspense fallback={''}>
      <Switch>
        <Route path="/login" component={(p) => <Login {...p} user={props.user} setUser={props.setUser} />} />
        <Route path="/logout" component={(p) => <Logout {...p} setUser={props.setUser} />} />
        <Route path="/resetPassword" component={ResetPassword} />
        <Route path="/changePassword/:token" component={ChangePassword} />
        <Route path="/" component={Default} exact />
        <Route path="/home" component={Home} />
        <Route path="/profile" component={Profile} />
        <Route path="/userAccount" component={lazy(() => import('./components/userAccount/Index'))} exact />
        <Route path="/userAccount/create" component={lazy(() => import('./components/userAccount/Create'))} exact />
        <Route path="/userAccount/:id/" component={lazy(() => import('./components/userAccount/Detail'))} exact />
        <Route path="/userAccount/edit/:id/" component={lazy(() => import('./components/userAccount/Edit'))} exact />
        <Route path="/userAccount/delete/:id/" component={lazy(() => import('./components/userAccount/Delete'))} exact />
        <Route path="/product" component={lazy(() => import('./components/product/Index'))} exact />
        <Route path="/product/create" component={lazy(() => import('./components/product/Create'))} exact />
        <Route path="/product/:id/" component={lazy(() => import('./components/product/Detail'))} exact />
        <Route path="/product/edit/:id/" component={lazy(() => import('./components/product/Edit'))} exact />
        <Route path="/product/delete/:id/" component={lazy(() => import('./components/product/Delete'))} exact />
        <Route path="/brand" component={lazy(() => import('./components/brand/Index'))} exact />
        <Route path="/brand/create" component={lazy(() => import('./components/brand/Create'))} exact />
        <Route path="/brand/:id/" component={lazy(() => import('./components/brand/Detail'))} exact />
        <Route path="/brand/edit/:id/" component={lazy(() => import('./components/brand/Edit'))} exact />
        <Route path="/brand/delete/:id/" component={lazy(() => import('./components/brand/Delete'))} exact />
        <Route path="/orderHeader" component={lazy(() => import('./components/orderHeader/Index'))} exact />
        <Route path="/orderHeader/create" component={lazy(() => import('./components/orderHeader/Create'))} exact />
        <Route path="/orderHeader/:id/" component={lazy(() => import('./components/orderHeader/Detail'))} exact />
        <Route path="/orderHeader/edit/:id/" component={lazy(() => import('./components/orderHeader/Edit'))} exact />
        <Route path="/orderHeader/delete/:id/" component={lazy(() => import('./components/orderHeader/Delete'))} exact />
        <Route path="/orderDetail/create" component={lazy(() => import('./components/orderDetail/Create'))} exact />
        <Route path="/orderDetail/edit/:orderId/:no/" component={lazy(() => import('./components/orderDetail/Edit'))} exact />
        <Route path="/orderDetail/delete/:orderId/:no/" component={lazy(() => import('./components/orderDetail/Delete'))} exact />
        <Route path="*" component={NotFound} />
      </Switch>
    </Suspense>
  )
}