/**
 * User and Authentication Type Definitions
 * Corresponds to backend User, UserPreference, and ViewingHistory entities
 */

import { Content, Channel } from './content.types';

/**
 * User roles in the system
 */
export enum UserRole {
  User = 'User',
  Premium = 'Premium',
  Admin = 'Admin'
}

/**
 * User entity
 */
export interface User {
  id: number;
  username: string;
  email: string;
  fullName?: string;
  createdAt: string; // ISO 8601 date string
  lastLoginAt?: string; // ISO 8601 date string
  isActive: boolean;
  role: UserRole;
}

/**
 * User with password hash (internal use only, should not be exposed to frontend)
 */
export interface UserWithPassword extends User {
  passwordHash: string;
}

/**
 * DTO for user registration
 */
export interface RegisterUserDto {
  username: string;
  email: string;
  password: string;
  fullName?: string;
}

/**
 * DTO for user login
 */
export interface LoginDto {
  username: string;
  password: string;
}

/**
 * Authentication response with tokens
 */
export interface AuthResponse {
  user: User;
  accessToken: string;
  refreshToken?: string;
  expiresIn: number; // Token expiration time in seconds
}

/**
 * DTO for updating user profile
 */
export interface UpdateUserDto {
  username?: string;
  email?: string;
  fullName?: string;
  isActive?: boolean;
  role?: UserRole;
}

/**
 * DTO for changing password
 */
export interface ChangePasswordDto {
  currentPassword: string;
  newPassword: string;
}

/**
 * DTO for password reset request
 */
export interface PasswordResetRequestDto {
  email: string;
}

/**
 * DTO for password reset confirmation
 */
export interface PasswordResetDto {
  token: string;
  newPassword: string;
}

/**
 * User preferences
 */
export interface UserPreference {
  id: number;
  userId: number;
  favoriteGenres?: string[]; // Array of genre strings
  favoriteChannels?: number[]; // Array of channel IDs
  language?: string;
  enableNotifications: boolean;
  autoPlayNext: boolean;
  preferredQuality?: number; // Quality setting (e.g., 480, 720, 1080)
  subtitlesEnabled: boolean;
  subtitleLanguage?: string;
  createdAt: string; // ISO 8601 date string
  updatedAt: string; // ISO 8601 date string
}

/**
 * DTO for creating or updating user preferences
 */
export interface UpdateUserPreferenceDto {
  favoriteGenres?: string[];
  favoriteChannels?: number[];
  language?: string;
  enableNotifications?: boolean;
  autoPlayNext?: boolean;
  preferredQuality?: number;
  subtitlesEnabled?: boolean;
  subtitleLanguage?: string;
}

/**
 * Viewing history entry
 */
export interface ViewingHistory {
  id: number;
  userId: number;
  contentId?: number;
  channelId?: number;
  startTime: string; // ISO 8601 date string
  endTime?: string; // ISO 8601 date string
  duration?: number; // Duration in seconds
  progress?: number; // Progress in seconds (for resume functionality)
  completed: boolean;
  deviceInfo?: string;
  createdAt: string; // ISO 8601 date string
  content?: Content; // Optional populated content data
  channel?: Channel; // Optional populated channel data
}

/**
 * DTO for creating a viewing history entry
 */
export interface CreateViewingHistoryDto {
  contentId?: number;
  channelId?: number;
  startTime: string; // ISO 8601 date string
  endTime?: string; // ISO 8601 date string
  duration?: number;
  progress?: number;
  completed?: boolean;
  deviceInfo?: string;
}

/**
 * DTO for updating a viewing history entry
 */
export interface UpdateViewingHistoryDto {
  endTime?: string; // ISO 8601 date string
  duration?: number;
  progress?: number;
  completed?: boolean;
}

/**
 * Viewing history filters
 */
export interface ViewingHistoryFilters {
  contentId?: number;
  channelId?: number;
  startDate?: string; // ISO 8601 date string
  endDate?: string; // ISO 8601 date string
  completed?: boolean;
  limit?: number;
  offset?: number;
}

/**
 * Paginated viewing history response
 */
export interface PaginatedViewingHistoryResponse {
  items: ViewingHistory[];
  total: number;
  limit: number;
  offset: number;
  hasMore: boolean;
}

/**
 * User statistics
 */
export interface UserStatistics {
  totalWatchTime: number; // Total watch time in seconds
  contentWatched: number; // Number of unique content items watched
  favoriteGenres: string[];
  recentlyWatched: ViewingHistory[];
  continueWatching: ViewingHistory[]; // In-progress content
}
