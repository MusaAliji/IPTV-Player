import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Layout from '@components/common/Layout';
import HomePage from '@pages/home/HomePage';
import PlayerPage from '@pages/player/PlayerPage';
import EPGPage from '@pages/epg/EPGPage';
import LibraryPage from '@pages/library/LibraryPage';
import './App.css';

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<Layout />}>
          <Route index element={<HomePage />} />
          <Route path="player" element={<PlayerPage />} />
          <Route path="epg" element={<EPGPage />} />
          <Route path="library" element={<LibraryPage />} />
        </Route>
      </Routes>
    </Router>
  );
}

export default App;
