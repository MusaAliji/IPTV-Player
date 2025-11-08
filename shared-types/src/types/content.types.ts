/**
 * Content and Media Type Definitions
 * Corresponds to backend Content entity and related types
 */

/**
 * Content types supported by the IPTV platform
 */
export enum ContentType {
  LiveTV = 'LiveTV',
  VOD = 'VOD',
  Series = 'Series',
  Movie = 'Movie'
}

/**
 * Main content entity representing any media content
 */
export interface Content {
  id: number;
  title: string;
  description?: string;
  streamUrl: string;
  thumbnailUrl?: string;
  type: ContentType;
  duration?: number; // Duration in seconds for VOD content
  releaseDate?: string; // ISO 8601 date string
  genre?: string;
  rating?: number; // Rating value (e.g., 0-10)
  createdAt: string; // ISO 8601 date string
  updatedAt: string; // ISO 8601 date string
}

/**
 * DTO for creating new content
 */
export interface CreateContentDto {
  title: string;
  description?: string;
  streamUrl: string;
  thumbnailUrl?: string;
  type: ContentType;
  duration?: number;
  releaseDate?: string;
  genre?: string;
  rating?: number;
}

/**
 * DTO for updating existing content
 */
export interface UpdateContentDto {
  title?: string;
  description?: string;
  streamUrl?: string;
  thumbnailUrl?: string;
  type?: ContentType;
  duration?: number;
  releaseDate?: string;
  genre?: string;
  rating?: number;
}

/**
 * Channel entity for live TV
 */
export interface Channel {
  id: number;
  name: string;
  streamUrl: string;
  logoUrl?: string;
  channelNumber: number;
  category?: string;
  language?: string;
  isActive: boolean;
  createdAt: string; // ISO 8601 date string
  updatedAt: string; // ISO 8601 date string
}

/**
 * DTO for creating a new channel
 */
export interface CreateChannelDto {
  name: string;
  streamUrl: string;
  logoUrl?: string;
  channelNumber: number;
  category?: string;
  language?: string;
  isActive?: boolean;
}

/**
 * DTO for updating an existing channel
 */
export interface UpdateChannelDto {
  name?: string;
  streamUrl?: string;
  logoUrl?: string;
  channelNumber?: number;
  category?: string;
  language?: string;
  isActive?: boolean;
}

/**
 * Content search filters
 */
export interface ContentFilters {
  type?: ContentType;
  genre?: string;
  minRating?: number;
  maxRating?: number;
  search?: string; // Search by title or description
  limit?: number;
  offset?: number;
}

/**
 * Paginated content response
 */
export interface PaginatedContentResponse {
  items: Content[];
  total: number;
  limit: number;
  offset: number;
  hasMore: boolean;
}

/**
 * Channel search filters
 */
export interface ChannelFilters {
  category?: string;
  language?: string;
  isActive?: boolean;
  search?: string; // Search by name
  limit?: number;
  offset?: number;
}

/**
 * Paginated channel response
 */
export interface PaginatedChannelResponse {
  items: Channel[];
  total: number;
  limit: number;
  offset: number;
  hasMore: boolean;
}
