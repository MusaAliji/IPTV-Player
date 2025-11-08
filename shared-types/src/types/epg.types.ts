/**
 * Electronic Program Guide (EPG) Type Definitions
 * Corresponds to backend EPGProgram entity and related types
 */

import { Channel } from './content.types';

/**
 * EPG Program entity representing a scheduled TV program
 */
export interface EPGProgram {
  id: number;
  channelId: number;
  title: string;
  description?: string;
  startTime: string; // ISO 8601 date string
  endTime: string; // ISO 8601 date string
  category?: string;
  rating?: string; // Content rating (e.g., "PG", "TV-14", etc.)
  createdAt: string; // ISO 8601 date string
  channel?: Channel; // Optional populated channel data
}

/**
 * DTO for creating a new EPG program
 */
export interface CreateEPGProgramDto {
  channelId: number;
  title: string;
  description?: string;
  startTime: string; // ISO 8601 date string
  endTime: string; // ISO 8601 date string
  category?: string;
  rating?: string;
}

/**
 * DTO for updating an existing EPG program
 */
export interface UpdateEPGProgramDto {
  channelId?: number;
  title?: string;
  description?: string;
  startTime?: string; // ISO 8601 date string
  endTime?: string; // ISO 8601 date string
  category?: string;
  rating?: string;
}

/**
 * EPG program filters for querying
 */
export interface EPGProgramFilters {
  channelId?: number;
  startDate?: string; // ISO 8601 date string
  endDate?: string; // ISO 8601 date string
  category?: string;
  search?: string; // Search by title or description
  limit?: number;
  offset?: number;
}

/**
 * Paginated EPG program response
 */
export interface PaginatedEPGProgramResponse {
  items: EPGProgram[];
  total: number;
  limit: number;
  offset: number;
  hasMore: boolean;
}

/**
 * EPG schedule for a specific channel and date range
 */
export interface EPGSchedule {
  channelId: number;
  channelName: string;
  programs: EPGProgram[];
  startDate: string; // ISO 8601 date string
  endDate: string; // ISO 8601 date string
}

/**
 * Multi-channel EPG grid data
 */
export interface EPGGrid {
  channels: Channel[];
  schedules: Map<number, EPGProgram[]>; // channelId -> programs
  timeSlots: TimeSlot[];
  startTime: string; // ISO 8601 date string
  endTime: string; // ISO 8601 date string
}

/**
 * Time slot for EPG grid view
 */
export interface TimeSlot {
  startTime: string; // ISO 8601 date string
  endTime: string; // ISO 8601 date string
  duration: number; // Duration in minutes
}

/**
 * Current and upcoming program information for a channel
 */
export interface ChannelSchedule {
  channelId: number;
  currentProgram?: EPGProgram;
  nextProgram?: EPGProgram;
  upcomingPrograms: EPGProgram[];
}
