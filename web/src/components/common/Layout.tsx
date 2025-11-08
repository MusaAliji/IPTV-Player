import React from 'react';
import { Link, Outlet } from 'react-router-dom';
import './Layout.css';

const Layout: React.FC = () => {
  return (
    <div className="layout">
      <nav className="navbar">
        <div className="nav-brand">
          <h1>IPTV Player</h1>
        </div>
        <ul className="nav-links">
          <li>
            <Link to="/">Home</Link>
          </li>
          <li>
            <Link to="/player">Live TV</Link>
          </li>
          <li>
            <Link to="/epg">TV Guide</Link>
          </li>
          <li>
            <Link to="/library">Library</Link>
          </li>
        </ul>
      </nav>
      <main className="main-content">
        <Outlet />
      </main>
    </div>
  );
};

export default Layout;
