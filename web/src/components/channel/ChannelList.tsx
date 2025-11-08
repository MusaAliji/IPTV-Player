import React from 'react';
import type { Channel } from '@muski/iptv-shared-types';
import './ChannelList.css';

interface ChannelListProps {
  channels: Channel[];
  selectedChannelId?: number;
  onChannelSelect: (channel: Channel) => void;
}

const ChannelList: React.FC<ChannelListProps> = ({
  channels,
  selectedChannelId,
  onChannelSelect,
}) => {
  return (
    <div className="channel-list">
      <h2 className="channel-list-title">Channels</h2>
      <div className="channel-list-content">
        {channels.map((channel) => (
          <div
            key={channel.id}
            className={`channel-item ${selectedChannelId === channel.id ? 'selected' : ''}`}
            onClick={() => onChannelSelect(channel)}
          >
            <div className="channel-number">{channel.channelNumber}</div>
            <div className="channel-details">
              <div className="channel-name">{channel.name}</div>
            </div>
            {channel.logoUrl && (
              <img src={channel.logoUrl} alt={channel.name} className="channel-logo" />
            )}
          </div>
        ))}
      </div>
    </div>
  );
};

export default ChannelList;
