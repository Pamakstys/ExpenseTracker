import { Outlet } from 'react-router-dom';
import Navbar from './Navbar.tsx';

export default function layout() {
  return (
    <div className="app-layout">
      <Navbar />
      <main>
        <Outlet />
      </main>
    </div>
  );
}
