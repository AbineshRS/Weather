import { useState } from 'react'

import './App.css'
import { BrowserRouter, Route, Routes } from 'react-router-dom'
import Weather from './components/Weather'

function App() {

  return (
    <div>
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<><Weather /></>} />
        </Routes>
      </BrowserRouter>
    </div>
  )
}

export default App
