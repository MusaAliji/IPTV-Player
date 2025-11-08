import { create } from 'zustand';
import type { Content, Channel } from '@muski/iptv-shared-types';

interface ContentState {
  contents: Content[];
  channels: Channel[];
  loading: boolean;
  error: string | null;

  // Actions
  setContents: (contents: Content[]) => void;
  setChannels: (channels: Channel[]) => void;
  setLoading: (loading: boolean) => void;
  setError: (error: string | null) => void;
}

export const useContentStore = create<ContentState>((set) => ({
  contents: [],
  channels: [],
  loading: false,
  error: null,

  setContents: (contents) => set({ contents }),
  setChannels: (channels) => set({ channels }),
  setLoading: (loading) => set({ loading }),
  setError: (error) => set({ error }),
}));
