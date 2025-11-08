import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import type { Content } from '@iptv-player/shared-types';
import { ContentType } from '@iptv-player/shared-types';
import { contentService } from '@services/contentService';
import { useContentStore } from '@store/useContentStore';
import ContentCard from '@components/content/ContentCard';
import './LibraryPage.css';

const LibraryPage: React.FC = () => {
  const navigate = useNavigate();
  const { contents, setContents, setLoading, setError } = useContentStore();
  const [filter, setFilter] = useState<ContentType | 'all'>('all');
  const [searchQuery, setSearchQuery] = useState('');

  useEffect(() => {
    loadContent();
  }, [filter]);

  const loadContent = async () => {
    try {
      setLoading(true);
      setError(null);

      const response = await contentService.getAllContent({
        offset: 0,
        limit: 100,
        type: filter === 'all' ? undefined : filter,
      });

      setContents(response.items);
    } catch (error: any) {
      console.error('Failed to load content:', error);
      setError(error.message || 'Failed to load content');
    } finally {
      setLoading(false);
    }
  };

  const handleSearch = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!searchQuery.trim()) {
      loadContent();
      return;
    }

    try {
      setLoading(true);
      const results = await contentService.searchContent(searchQuery);
      setContents(results);
    } catch (error: any) {
      console.error('Search failed:', error);
      setError('Search failed');
    } finally {
      setLoading(false);
    }
  };

  const handleContentClick = (content: Content) => {
    navigate(`/player?contentId=${content.id}`);
  };

  const filteredContents = contents;

  return (
    <div className="library-page">
      <div className="library-header">
        <h1>Video Library</h1>
        <div className="library-controls">
          <form onSubmit={handleSearch} className="search-form">
            <input
              type="text"
              placeholder="Search content..."
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
            />
            <button type="submit">Search</button>
          </form>

          <div className="filter-buttons">
            <button
              className={filter === 'all' ? 'active' : ''}
              onClick={() => setFilter('all')}
            >
              All
            </button>
            <button
              className={filter === ContentType.Movie ? 'active' : ''}
              onClick={() => setFilter(ContentType.Movie)}
            >
              Movies
            </button>
            <button
              className={filter === ContentType.Series ? 'active' : ''}
              onClick={() => setFilter(ContentType.Series)}
            >
              Series
            </button>
            <button
              className={filter === ContentType.VOD ? 'active' : ''}
              onClick={() => setFilter(ContentType.VOD)}
            >
              VOD
            </button>
          </div>
        </div>
      </div>

      <div className="library-content">
        {filteredContents.length > 0 ? (
          <div className="content-grid">
            {filteredContents.map((content) => (
              <ContentCard key={content.id} content={content} onClick={handleContentClick} />
            ))}
          </div>
        ) : (
          <p className="no-content">No content found</p>
        )}
      </div>
    </div>
  );
};

export default LibraryPage;
