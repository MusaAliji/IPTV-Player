import React, { useEffect, useState } from 'react';
import { useSearchParams } from 'react-router-dom';
import type { Channel } from '@iptv-player/shared-types';
import { contentService } from '@services/contentService';
import { usePlayerStore } from '@store/usePlayerStore';
import { useContentStore } from '@store/useContentStore';
import VideoPlayer from '@components/video/VideoPlayer';
import ChannelList from '@components/channel/ChannelList';
import './PlayerPage.css';

const PlayerPage: React.FC = () => {
  const [searchParams] = useSearchParams();
  const channelId = searchParams.get('channelId');
  const contentId = searchParams.get('contentId');

  const { channels, setChannels } = useContentStore();
  const { currentChannel, currentContent, setCurrentChannel, setCurrentContent } = usePlayerStore();
  const [streamUrl, setStreamUrl] = useState<string>('');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    loadChannels();
  }, []);

  useEffect(() => {
    if (channelId) {
      loadChannel(channelId);
    } else if (contentId) {
      loadContent(contentId);
    }
  }, [channelId, contentId]);

  const loadChannels = async () => {
    try {
      const response = await contentService.getAllChannels({ limit: 50, offset: 0 });
      setChannels(response.items);
    } catch (error: any) {
      console.error('Failed to load channels:', error);
    }
  };

  const loadChannel = async (id: string) => {
    try {
      setLoading(true);
      setError(null);

      const channelIdNum = parseInt(id, 10);
      const channel = await contentService.getChannelById(channelIdNum);
      setCurrentChannel(channel);

      if (channel.streamUrl) {
        setStreamUrl(channel.streamUrl);
      }
    } catch (error: any) {
      console.error('Failed to load channel:', error);
      setError('Failed to load channel');
    } finally {
      setLoading(false);
    }
  };

  const loadContent = async (id: string) => {
    try {
      setLoading(true);
      setError(null);

      const contentIdNum = parseInt(id, 10);
      const content = await contentService.getContentById(contentIdNum);
      setCurrentContent(content);

      const streamData = await contentService.getStreamUrl(contentIdNum);
      setStreamUrl(streamData.url);
    } catch (error: any) {
      console.error('Failed to load content:', error);
      setError('Failed to load content');
    } finally {
      setLoading(false);
    }
  };

  const handleChannelSelect = (channel: Channel) => {
    setCurrentChannel(channel);
    setCurrentContent(null);
    if (channel.streamUrl) {
      setStreamUrl(channel.streamUrl);
    }
  };

  return (
    <div className="player-page">
      <div className="player-main">
        <div className="player-container">
          {loading && <div className="loading">Loading...</div>}
          {error && <div className="error">{error}</div>}
          {streamUrl && !loading && !error && (
            <VideoPlayer streamUrl={streamUrl} autoPlay={true} onError={setError} />
          )}
          {!streamUrl && !loading && !error && (
            <div className="no-stream">
              <p>Select a channel or content to start watching</p>
            </div>
          )}
        </div>

        <div className="player-info">
          {currentChannel && (
            <div className="now-playing">
              <h2>{currentChannel.name}</h2>
            </div>
          )}
          {currentContent && (
            <div className="now-playing">
              <h2>{currentContent.title}</h2>
              {currentContent.description && <p>{currentContent.description}</p>}
            </div>
          )}
        </div>
      </div>

      <aside className="channel-sidebar">
        <ChannelList
          channels={channels}
          selectedChannelId={currentChannel?.id}
          onChannelSelect={handleChannelSelect}
        />
      </aside>
    </div>
  );
};

export default PlayerPage;
