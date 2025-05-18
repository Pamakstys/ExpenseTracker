import { createRoot } from 'react-dom/client'
import './index.css'
import { BrowserRouter, Routes, Route } from 'react-router-dom'
import Layout from './components/Layout.tsx'
import Login from './pages/Login.tsx'
import Home from './pages/Home.tsx'
import Index from './pages/Index.tsx'
import Groups from './pages/Groups.tsx'
import ViewGroup from './pages/ViewGroup.tsx'

createRoot(document.getElementById('root')!).render(
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Layout />}>
          <Route index element={<Index/>}/>
          <Route path="/home" element={<Home/>}/>
          <Route path="/groups" element={<Groups/>}/>
          <Route path="/groups/:groupId" element={<ViewGroup/>}/>
        </Route>
        
        <Route path="/login" element={<Login/>}/>
      </Routes>
    </BrowserRouter>
)
