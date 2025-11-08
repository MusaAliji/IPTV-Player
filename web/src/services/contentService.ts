import { apiService } from './api';
import type {
  Content,
  Channel,
  PaginatedContentResponse,
  PaginatedChannelResponse,
  ContentFilters,
  ChannelFilters,
} from '@muski/iptv-shared-types';
import { API_ENDPOINTS } from '@muski/iptv-shared-types';

export const contentService = {
  // Content operations
  async getAllContent(filters?: ContentFilters): Promise<PaginatedContentResponse> {
    return apiService.get<PaginatedContentResponse>(API_ENDPOINTS.CONTENT.BASE, filters);
  },

  async getContentById(id: number): Promise<Content> {
    return apiService.get<Content>(API_ENDPOINTS.CONTENT.BY_ID(id));
  },

  async searchContent(query: string): Promise<Content[]> {
    return apiService.get<Content[]>(API_ENDPOINTS.CONTENT.SEARCH, { q: query });
  },

  // Channel operations
  async getAllChannels(filters?: ChannelFilters): Promise<PaginatedChannelResponse> {
    return apiService.get<PaginatedChannelResponse>(API_ENDPOINTS.CHANNELS.BASE, filters);
  },

  async getChannelById(id: number): Promise<Channel> {
    return apiService.get<Channel>(API_ENDPOINTS.CHANNELS.BY_ID(id));
  },

  async getLiveChannels(): Promise<Channel[]> {
    return apiService.get<Channel[]>(API_ENDPOINTS.CHANNELS.LIVE);
  },

  // Streaming
  async getStreamUrl(contentId: number, quality?: string): Promise<{ url: string }> {
    return apiService.get<{ url: string }>(
      API_ENDPOINTS.STREAMING.GET_STREAM(contentId),
      quality ? { quality } : undefined
    );
  },
};
