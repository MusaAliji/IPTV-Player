import React, { useEffect, useRef, useState } from 'react';
import Hls from 'hls.js';
import { usePlayerStore } from '@store/usePlayerStore';
import './VideoPlayer.css';

interface VideoPlayerProps {
  streamUrl: string;
  autoPlay?: boolean;
  onError?: (error: string) => void;
}

const VideoPlayer: React.FC<VideoPlayerProps> = ({ streamUrl, autoPlay = false, onError }) => {
  const videoRef = useRef<HTMLVideoElement>(null);
  const hlsRef = useRef<Hls | null>(null);
  const [error, setError] = useState<string | null>(null);

  const {
    isPlaying,
    volume,
    isMuted,
    setIsPlaying,
    setCurrentTime,
    setDuration,
  } = usePlayerStore();

  useEffect(() => {
    const video = videoRef.current;
    if (!video) return;

    // Set volume
    video.volume = volume;
    video.muted = isMuted;
  }, [volume, isMuted]);

  useEffect(() => {
    const video = videoRef.current;
    if (!video || !streamUrl) return;

    // Clean up previous HLS instance
    if (hlsRef.current) {
      hlsRef.current.destroy();
      hlsRef.current = null;
    }

    if (Hls.isSupported()) {
      const hls = new Hls({
        enableWorker: true,
        lowLatencyMode: true,
        backBufferLength: 90,
      });

      hlsRef.current = hls;

      hls.loadSource(streamUrl);
      hls.attachMedia(video);

      hls.on(Hls.Events.MANIFEST_PARSED, () => {
        if (autoPlay) {
          video.play().catch((err) => {
            console.error('Auto-play failed:', err);
            setIsPlaying(false);
          });
        }
      });

      hls.on(Hls.Events.ERROR, (_event, data) => {
        console.error('HLS Error:', data);
        if (data.fatal) {
          switch (data.type) {
            case Hls.ErrorTypes.NETWORK_ERROR:
              setError('Network error occurred');
              onError?.('Network error occurred');
              hls.startLoad();
              break;
            case Hls.ErrorTypes.MEDIA_ERROR:
              setError('Media error occurred');
              onError?.('Media error occurred');
              hls.recoverMediaError();
              break;
            default:
              setError('Fatal error occurred');
              onError?.('Fatal error occurred');
              hls.destroy();
              break;
          }
        }
      });
    } else if (video.canPlayType('application/vnd.apple.mpegurl')) {
      // Native HLS support (Safari)
      video.src = streamUrl;
      if (autoPlay) {
        video.play().catch((err) => {
          console.error('Auto-play failed:', err);
          setIsPlaying(false);
        });
      }
    } else {
      setError('HLS is not supported in this browser');
      onError?.('HLS is not supported in this browser');
    }

    return () => {
      if (hlsRef.current) {
        hlsRef.current.destroy();
      }
    };
  }, [streamUrl, autoPlay, onError, setIsPlaying]);

  useEffect(() => {
    const video = videoRef.current;
    if (!video) return;

    if (isPlaying) {
      video.play().catch((err) => {
        console.error('Play failed:', err);
        setIsPlaying(false);
      });
    } else {
      video.pause();
    }
  }, [isPlaying, setIsPlaying]);

  const handleTimeUpdate = () => {
    if (videoRef.current) {
      setCurrentTime(videoRef.current.currentTime);
    }
  };

  const handleLoadedMetadata = () => {
    if (videoRef.current) {
      setDuration(videoRef.current.duration);
    }
  };

  const handlePlay = () => {
    setIsPlaying(true);
  };

  const handlePause = () => {
    setIsPlaying(false);
  };

  return (
    <div className="video-player-container">
      {error && <div className="video-error">{error}</div>}
      <video
        ref={videoRef}
        className="video-player"
        controls
        onTimeUpdate={handleTimeUpdate}
        onLoadedMetadata={handleLoadedMetadata}
        onPlay={handlePlay}
        onPause={handlePause}
      />
    </div>
  );
};

export default VideoPlayer;
