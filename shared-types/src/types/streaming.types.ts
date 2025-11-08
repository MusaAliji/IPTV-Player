/**
 * Streaming and Playback Type Definitions
 * Types related to video streaming, playback control, and quality settings
 */

/**
 * Streaming quality levels
 */
export enum StreamQuality {
  Auto = 'auto',
  Low = '360p',      // 360p
  Medium = '480p',   // 480p
  High = '720p',     // 720p HD
  Full = '1080p',    // 1080p Full HD
  Ultra = '4k'       // 4K Ultra HD
}

/**
 * Playback state
 */
export enum PlaybackState {
  Idle = 'idle',
  Loading = 'loading',
  Playing = 'playing',
  Paused = 'paused',
  Buffering = 'buffering',
  Ended = 'ended',
  Error = 'error'
}

/**
 * Stream protocol types
 */
export enum StreamProtocol {
  HLS = 'hls',       // HTTP Live Streaming
  DASH = 'dash',     // Dynamic Adaptive Streaming over HTTP
  RTMP = 'rtmp',     // Real-Time Messaging Protocol
  WebRTC = 'webrtc', // Web Real-Time Communication
  HTTP = 'http'      // Progressive HTTP download
}

/**
 * Subtitle/Caption track
 */
export interface SubtitleTrack {
  id: string;
  label: string;
  language: string;
  languageCode: string; // ISO 639-1 code
  url?: string;
  isDefault: boolean;
}

/**
 * Audio track
 */
export interface AudioTrack {
  id: string;
  label: string;
  language: string;
  languageCode: string; // ISO 639-1 code
  isDefault: boolean;
}

/**
 * Stream information
 */
export interface StreamInfo {
  url: string;
  protocol: StreamProtocol;
  quality: StreamQuality;
  bitrate?: number; // In kbps
  resolution?: {
    width: number;
    height: number;
  };
  codec?: string;
  fps?: number; // Frames per second
}

/**
 * Available stream sources with multiple quality options
 */
export interface StreamSource {
  contentId?: number;
  channelId?: number;
  streams: StreamInfo[];
  subtitles: SubtitleTrack[];
  audioTracks: AudioTrack[];
  thumbnailUrl?: string;
  duration?: number; // Duration in seconds (for VOD)
}

/**
 * Playback position for resume functionality
 */
export interface PlaybackPosition {
  contentId?: number;
  channelId?: number;
  position: number; // Position in seconds
  duration: number; // Total duration in seconds
  percentage: number; // Position as percentage (0-100)
  timestamp: string; // ISO 8601 date string - when this position was recorded
}

/**
 * Player configuration
 */
export interface PlayerConfig {
  autoPlay: boolean;
  autoQuality: boolean; // Auto quality selection based on bandwidth
  preferredQuality: StreamQuality;
  volume: number; // 0-100
  muted: boolean;
  subtitlesEnabled: boolean;
  preferredSubtitleLanguage?: string;
  preferredAudioLanguage?: string;
  playbackRate: number; // Playback speed (e.g., 1.0 = normal, 1.5 = 1.5x speed)
  seekInterval: number; // Seek interval in seconds (e.g., 10 for 10-second skip)
}

/**
 * Playback error types
 */
export enum PlaybackError {
  NetworkError = 'network_error',
  DecodeError = 'decode_error',
  SourceNotSupported = 'source_not_supported',
  DRMError = 'drm_error',
  UnknownError = 'unknown_error'
}

/**
 * Playback error information
 */
export interface PlaybackErrorInfo {
  type: PlaybackError;
  message: string;
  code?: string;
  details?: any;
  recoverable: boolean;
}

/**
 * Player state
 */
export interface PlayerState {
  state: PlaybackState;
  currentTime: number; // Current playback position in seconds
  duration: number; // Total duration in seconds
  bufferedPercentage: number; // 0-100
  volume: number; // 0-100
  muted: boolean;
  quality: StreamQuality;
  playbackRate: number;
  isLive: boolean; // Whether this is a live stream
  isFullscreen: boolean;
  subtitlesEnabled: boolean;
  currentSubtitle?: SubtitleTrack;
  currentAudioTrack?: AudioTrack;
  error?: PlaybackErrorInfo;
}

/**
 * Bandwidth/Network information
 */
export interface NetworkInfo {
  bandwidth: number; // In kbps
  latency: number; // In milliseconds
  recommendedQuality: StreamQuality;
}

/**
 * Streaming analytics event
 */
export interface StreamingAnalyticsEvent {
  userId?: number;
  contentId?: number;
  channelId?: number;
  eventType: StreamingEventType;
  timestamp: string; // ISO 8601 date string
  quality: StreamQuality;
  buffering: boolean;
  bitrate?: number;
  error?: PlaybackErrorInfo;
  deviceInfo?: string;
}

/**
 * Types of streaming events for analytics
 */
export enum StreamingEventType {
  StreamStart = 'stream_start',
  StreamEnd = 'stream_end',
  QualityChange = 'quality_change',
  BufferStart = 'buffer_start',
  BufferEnd = 'buffer_end',
  Seek = 'seek',
  Pause = 'pause',
  Resume = 'resume',
  Error = 'error',
  BitrateChange = 'bitrate_change'
}

/**
 * Live stream status
 */
export interface LiveStreamStatus {
  channelId: number;
  isLive: boolean;
  viewerCount?: number;
  startedAt?: string; // ISO 8601 date string
  currentProgram?: {
    title: string;
    startTime: string;
    endTime: string;
  };
}
