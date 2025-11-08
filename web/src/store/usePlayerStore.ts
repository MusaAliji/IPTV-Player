import { create } from 'zustand';
import type { Content, Channel } from '@iptv-player/shared-types';

interface PlayerState {
  currentContent: Content | null;
  currentChannel: Channel | null;
  isPlaying: boolean;
  volume: number;
  isMuted: boolean;
  currentTime: number;
  duration: number;
  quality: string;

  // Actions
  setCurrentContent: (content: Content | null) => void;
  setCurrentChannel: (channel: Channel | null) => void;
  setIsPlaying: (isPlaying: boolean) => void;
  setVolume: (volume: number) => void;
  setIsMuted: (isMuted: boolean) => void;
  setCurrentTime: (currentTime: number) => void;
  setDuration: (duration: number) => void;
  setQuality: (quality: string) => void;
}

export const usePlayerStore = create<PlayerState>((set) => ({
  currentContent: null,
  currentChannel: null,
  isPlaying: false,
  volume: 1,
  isMuted: false,
  currentTime: 0,
  duration: 0,
  quality: 'auto',

  setCurrentContent: (content) => set({ currentContent: content }),
  setCurrentChannel: (channel) => set({ currentChannel: channel }),
  setIsPlaying: (isPlaying) => set({ isPlaying }),
  setVolume: (volume) => set({ volume }),
  setIsMuted: (isMuted) => set({ isMuted }),
  setCurrentTime: (currentTime) => set({ currentTime }),
  setDuration: (duration) => set({ duration }),
  setQuality: (quality) => set({ quality }),
}));
