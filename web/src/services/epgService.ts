import { apiService } from './api';
import type {
  EPGProgram,
  EPGSchedule,
} from '@iptv-player/shared-types';
import { API_ENDPOINTS } from '@iptv-player/shared-types';

export const epgService = {
  async getProgramsByChannel(
    channelId: number,
    startTime: Date,
    endTime: Date
  ): Promise<EPGProgram[]> {
    return apiService.get<EPGProgram[]>(
      API_ENDPOINTS.EPG.BY_CHANNEL(channelId),
      {
        startTime: startTime.toISOString(),
        endTime: endTime.toISOString(),
      }
    );
  },

  async getCurrentProgram(channelId: number): Promise<EPGProgram | null> {
    return apiService.get<EPGProgram | null>(API_ENDPOINTS.EPG.CURRENT(channelId));
  },

  async getSchedule(channelId: number, date: Date): Promise<EPGSchedule> {
    return apiService.get<EPGSchedule>(
      API_ENDPOINTS.EPG.SCHEDULE,
      { channelId, date: date.toISOString() }
    );
  },

  async getAllPrograms(startTime: Date, endTime: Date): Promise<EPGProgram[]> {
    return apiService.get<EPGProgram[]>(API_ENDPOINTS.EPG.BASE, {
      startTime: startTime.toISOString(),
      endTime: endTime.toISOString(),
    });
  },
};
