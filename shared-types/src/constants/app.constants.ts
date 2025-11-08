/**
 * Application Constants
 * General application configuration and constants
 */

/**
 * Timeout configurations (in milliseconds)
 */
export const TIMEOUTS = {
  API_REQUEST: 30000,          // 30 seconds
  FILE_UPLOAD: 120000,         // 2 minutes
  STREAMING_CONNECT: 15000,    // 15 seconds
  STREAM_BUFFER: 10000,        // 10 seconds
  WEBSOCKET_CONNECT: 10000,    // 10 seconds
  RETRY_DELAY: 2000            // 2 seconds
} as const;

/**
 * Pagination defaults
 */
export const PAGINATION = {
  DEFAULT_LIMIT: 20,
  MAX_LIMIT: 100,
  DEFAULT_OFFSET: 0
} as const;

/**
 * Validation limits
 */
export const VALIDATION = {
  USERNAME: {
    MIN_LENGTH: 3,
    MAX_LENGTH: 30,
    PATTERN: /^[a-zA-Z0-9_-]+$/
  },
  PASSWORD: {
    MIN_LENGTH: 8,
    MAX_LENGTH: 128,
    REQUIRE_UPPERCASE: true,
    REQUIRE_LOWERCASE: true,
    REQUIRE_NUMBER: true,
    REQUIRE_SPECIAL_CHAR: false
  },
  EMAIL: {
    MAX_LENGTH: 255,
    PATTERN: /^[^\s@]+@[^\s@]+\.[^\s@]+$/
  },
  CONTENT_TITLE: {
    MIN_LENGTH: 1,
    MAX_LENGTH: 200
  },
  CONTENT_DESCRIPTION: {
    MAX_LENGTH: 2000
  },
  CHANNEL_NAME: {
    MIN_LENGTH: 1,
    MAX_LENGTH: 100
  }
} as const;

/**
 * Storage keys for local/session storage
 */
export const STORAGE_KEYS = {
  ACCESS_TOKEN: 'iptv_access_token',
  REFRESH_TOKEN: 'iptv_refresh_token',
  USER_PREFERENCES: 'iptv_user_preferences',
  PLAYER_CONFIG: 'iptv_player_config',
  THEME: 'iptv_theme',
  LANGUAGE: 'iptv_language',
  LAST_VIEWED: 'iptv_last_viewed',
  PLAYBACK_POSITION: 'iptv_playback_position'
} as const;

/**
 * Date/Time formats
 */
export const DATE_FORMATS = {
  ISO_8601: "yyyy-MM-dd'T'HH:mm:ss.SSSxxx",
  DATE_ONLY: 'yyyy-MM-dd',
  TIME_ONLY: 'HH:mm:ss',
  DISPLAY_DATE: 'MMM dd, yyyy',
  DISPLAY_TIME: 'hh:mm a',
  DISPLAY_DATETIME: 'MMM dd, yyyy hh:mm a',
  EPG_TIME: 'HH:mm'
} as const;

/**
 * Streaming configuration
 */
export const STREAMING_CONFIG = {
  DEFAULT_QUALITY: '720p',
  AUTO_QUALITY: true,
  BUFFER_SIZE: 10, // seconds
  MAX_RETRIES: 3,
  SEEK_INTERVAL: 10, // seconds
  VOLUME: {
    DEFAULT: 80,
    MAX: 100,
    MIN: 0
  },
  PLAYBACK_RATE: {
    DEFAULT: 1.0,
    MIN: 0.5,
    MAX: 2.0,
    STEP: 0.25
  }
} as const;

/**
 * Content rating values
 */
export const CONTENT_RATINGS = [
  'G',      // General Audiences
  'PG',     // Parental Guidance Suggested
  'PG-13',  // Parents Strongly Cautioned
  'R',      // Restricted
  'NC-17',  // Adults Only
  'TV-Y',   // All Children
  'TV-Y7',  // Directed to Older Children
  'TV-G',   // General Audience
  'TV-PG',  // Parental Guidance Suggested
  'TV-14',  // Parents Strongly Cautioned
  'TV-MA'   // Mature Audience Only
] as const;

/**
 * Supported languages
 */
export const SUPPORTED_LANGUAGES = [
  { code: 'en', name: 'English' },
  { code: 'es', name: 'Spanish' },
  { code: 'fr', name: 'French' },
  { code: 'de', name: 'German' },
  { code: 'it', name: 'Italian' },
  { code: 'pt', name: 'Portuguese' },
  { code: 'ru', name: 'Russian' },
  { code: 'zh', name: 'Chinese' },
  { code: 'ja', name: 'Japanese' },
  { code: 'ko', name: 'Korean' },
  { code: 'ar', name: 'Arabic' }
] as const;

/**
 * Common video quality settings
 */
export const VIDEO_QUALITIES = [
  { label: 'Auto', value: 'auto', resolution: null },
  { label: '360p', value: '360p', resolution: { width: 640, height: 360 } },
  { label: '480p', value: '480p', resolution: { width: 854, height: 480 } },
  { label: '720p', value: '720p', resolution: { width: 1280, height: 720 } },
  { label: '1080p', value: '1080p', resolution: { width: 1920, height: 1080 } },
  { label: '4K', value: '4k', resolution: { width: 3840, height: 2160 } }
] as const;

/**
 * Error messages
 */
export const ERROR_MESSAGES = {
  NETWORK_ERROR: 'Network error. Please check your connection.',
  UNAUTHORIZED: 'You are not authorized to perform this action.',
  NOT_FOUND: 'The requested resource was not found.',
  VALIDATION_ERROR: 'Please check your input and try again.',
  SERVER_ERROR: 'An unexpected error occurred. Please try again later.',
  STREAM_ERROR: 'Failed to load the stream. Please try again.',
  TIMEOUT: 'Request timed out. Please try again.'
} as const;

/**
 * Success messages
 */
export const SUCCESS_MESSAGES = {
  LOGIN: 'Login successful!',
  LOGOUT: 'Logged out successfully.',
  REGISTER: 'Registration successful! Please verify your email.',
  PASSWORD_CHANGED: 'Password changed successfully.',
  PROFILE_UPDATED: 'Profile updated successfully.',
  PREFERENCES_SAVED: 'Preferences saved successfully.',
  CONTENT_ADDED: 'Content added successfully.',
  CHANNEL_ADDED: 'Channel added successfully.'
} as const;
