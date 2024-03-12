import React, { useEffect } from 'react'

export default function Default(props) {

  let { history } = props

  useEffect(() => {
    if (location.hash) {
      history.push(location.hash.substr(1))
    }
  }, [ history ])
  
  return (
    <div>
    </div>
  )
}