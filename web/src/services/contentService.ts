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

  // Channel operations (LiveTV content)
  // Note: Channels are Content items with type='LiveTV' in the backend
  async getAllChannels(filters?: ChannelFilters): Promise<PaginatedChannelResponse> {
    // Map to content endpoint with type filter
    const response = await apiService.get<Content[]>(
      API_ENDPOINTS.CONTENT.BY_TYPE('LiveTV'),
      filters
    );
    // Transform Content[] to match expected channel response structure
    const limit = filters?.limit || response.length;
    const offset = filters?.offset || 0;
    return {
      items: response as unknown as Channel[],
      total: response.length,
      limit,
      offset,
      hasMore: offset + limit < response.length,
    };
  },

  async getChannelById(id: number): Promise<Channel> {
    // Channels are just content items, so use content endpoint
    const content = await apiService.get<Content>(API_ENDPOINTS.CONTENT.BY_ID(id));
    return content as unknown as Channel;
  },

  async getLiveChannels(): Promise<Channel[]> {
    // Get all LiveTV type content
    const response = await apiService.get<Content[]>(API_ENDPOINTS.CONTENT.BY_TYPE('LiveTV'));
    return response as unknown as Channel[];
  },

  // Streaming
  async getStreamUrl(contentId: number, quality?: string): Promise<{ url: string }> {
    return apiService.get<{ url: string }>(
      API_ENDPOINTS.STREAMING.GET_STREAM(contentId),
      quality ? { quality } : undefined
    );
  },
};
