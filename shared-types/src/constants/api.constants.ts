/**
 * API Constants
 * API endpoints and HTTP-related constants
 */

/**
 * API base paths
 */
export const API_BASE_PATH = '/api/v1';

/**
 * API endpoint paths
 */
export const API_ENDPOINTS = {
  // Authentication
  AUTH: {
    LOGIN: '/auth/login',
    REGISTER: '/auth/register',
    LOGOUT: '/auth/logout',
    REFRESH_TOKEN: '/auth/refresh',
    PASSWORD_RESET_REQUEST: '/auth/password-reset/request',
    PASSWORD_RESET: '/auth/password-reset',
    VERIFY_EMAIL: '/auth/verify-email'
  },

  // Users
  USERS: {
    BASE: '/users',
    BY_ID: (id: number) => `/users/${id}`,
    PROFILE: '/users/profile',
    PREFERENCES: '/users/preferences',
    VIEWING_HISTORY: '/users/viewing-history',
    STATISTICS: '/users/statistics'
  },

  // Content
  CONTENT: {
    BASE: '/content',
    BY_ID: (id: number) => `/content/${id}`,
    SEARCH: '/content/search',
    BY_TYPE: (type: string) => `/content/type/${type}`,
    BY_GENRE: (genre: string) => `/content/genre/${genre}`,
    TRENDING: '/content/trending',
    RECOMMENDED: '/content/recommended'
  },

  // Channels
  CHANNELS: {
    BASE: '/channels',
    BY_ID: (id: number) => `/channels/${id}`,
    BY_CATEGORY: (category: string) => `/channels/category/${category}`,
    LIVE: '/channels/live',
    FAVORITES: '/channels/favorites'
  },

  // EPG (Electronic Program Guide)
  EPG: {
    BASE: '/epg',
    BY_ID: (id: number) => `/epg/${id}`,
    BY_CHANNEL: (channelId: number) => `/epg/channel/${channelId}`,
    CURRENT: (channelId: number) => `/epg/channel/${channelId}/current`,
    SCHEDULE: '/epg/schedule',
    GRID: '/epg/grid'
  },

  // Streaming
  STREAMING: {
    GET_STREAM: (contentId: number) => `/streaming/content/${contentId}`,
    GET_CHANNEL_STREAM: (channelId: number) => `/streaming/channel/${channelId}`,
    ANALYTICS: '/streaming/analytics',
    STATUS: '/streaming/status'
  }
} as const;

/**
 * HTTP status codes
 */
export const HTTP_STATUS = {
  OK: 200,
  CREATED: 201,
  NO_CONTENT: 204,
  BAD_REQUEST: 400,
  UNAUTHORIZED: 401,
  FORBIDDEN: 403,
  NOT_FOUND: 404,
  CONFLICT: 409,
  UNPROCESSABLE_ENTITY: 422,
  INTERNAL_SERVER_ERROR: 500,
  SERVICE_UNAVAILABLE: 503
} as const;

/**
 * HTTP methods
 */
export const HTTP_METHODS = {
  GET: 'GET',
  POST: 'POST',
  PUT: 'PUT',
  PATCH: 'PATCH',
  DELETE: 'DELETE'
} as const;

/**
 * Content type headers
 */
export const CONTENT_TYPES = {
  JSON: 'application/json',
  FORM_DATA: 'multipart/form-data',
  URL_ENCODED: 'application/x-www-form-urlencoded',
  TEXT: 'text/plain'
} as const;

/**
 * API rate limiting
 */
export const RATE_LIMITS = {
  DEFAULT_REQUESTS_PER_MINUTE: 60,
  AUTH_REQUESTS_PER_MINUTE: 5,
  STREAMING_REQUESTS_PER_MINUTE: 30
} as const;
