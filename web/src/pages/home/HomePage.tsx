import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import type { Content, Channel } from '@muski/iptv-shared-types';
import { contentService } from '@services/contentService';
import { useContentStore } from '@store/useContentStore';
import ContentCard from '@components/content/ContentCard';
import ChannelList from '@components/channel/ChannelList';
import './HomePage.css';

const HomePage: React.FC = () => {
  const navigate = useNavigate();
  const { channels, setContents, setChannels, setLoading, setError } = useContentStore();
  const [featuredContent, setFeaturedContent] = useState<Content[]>([]);

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      setError(null);

      // Load content and channels
      const [contentResponse, channelResponse] = await Promise.all([
        contentService.getAllContent({ limit: 20, offset: 0 }),
        contentService.getAllChannels({ limit: 10, offset: 0 }),
      ]);

      setContents(contentResponse.items);
      setChannels(channelResponse.items);
      setFeaturedContent(contentResponse.items.slice(0, 6));
    } catch (error: any) {
      console.error('Failed to load data:', error);
      setError(error.message || 'Failed to load data');
    } finally {
      setLoading(false);
    }
  };

  const handleContentClick = (content: Content) => {
    navigate(`/player?contentId=${content.id}`);
  };

  const handleChannelSelect = (channel: Channel) => {
    navigate(`/player?channelId=${channel.id}`);
  };

  return (
    <div className="home-page">
      <section className="hero-section">
        <h1>Welcome to IPTV Player</h1>
        <p>Stream your favorite TV shows, movies, and live channels</p>
      </section>

      <section className="content-section">
        <h2>Featured Content</h2>
        <div className="content-grid">
          {featuredContent.map((content) => (
            <ContentCard key={content.id} content={content} onClick={handleContentClick} />
          ))}
        </div>
      </section>

      <section className="channels-section">
        <h2>Popular Channels</h2>
        {channels.length > 0 && (
          <ChannelList channels={channels.slice(0, 5)} onChannelSelect={handleChannelSelect} />
        )}
      </section>
    </div>
  );
};

export default HomePage;
