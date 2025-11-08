import React from 'react';
import type { Content } from '@muski/iptv-shared-types';
import { formatDuration } from '@muski/iptv-shared-types';
import './ContentCard.css';

interface ContentCardProps {
  content: Content;
  onClick?: (content: Content) => void;
}

const ContentCard: React.FC<ContentCardProps> = ({ content, onClick }) => {
  const handleClick = () => {
    onClick?.(content);
  };

  return (
    <div className="content-card" onClick={handleClick}>
      {content.thumbnailUrl && (
        <div className="content-thumbnail">
          <img src={content.thumbnailUrl} alt={content.title} />
          {content.duration && (
            <div className="content-duration">{formatDuration(content.duration)}</div>
          )}
        </div>
      )}

      <div className="content-info">
        <h3 className="content-title">{content.title}</h3>

        {content.description && (
          <p className="content-description">{content.description}</p>
        )}

        <div className="content-metadata">
          {content.rating && (
            <span className="content-rating">⭐ {content.rating.toFixed(1)}</span>
          )}
          {content.releaseDate && (
            <span className="content-year">{content.releaseDate}</span>
          )}
          {content.genre && (
            <span className="content-genre">{content.genre}</span>
          )}
        </div>
      </div>
    </div>
  );
};

export default ContentCard;
