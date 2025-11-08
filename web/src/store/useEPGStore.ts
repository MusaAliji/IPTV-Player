import { create } from 'zustand';
import type { EPGProgram, EPGSchedule } from '@muski/iptv-shared-types';

interface EPGState {
  programs: EPGProgram[];
  schedules: EPGSchedule[];
  currentDate: Date;
  loading: boolean;
  error: string | null;

  // Actions
  setPrograms: (programs: EPGProgram[]) => void;
  setSchedules: (schedules: EPGSchedule[]) => void;
  setCurrentDate: (date: Date) => void;
  setLoading: (loading: boolean) => void;
  setError: (error: string | null) => void;
}

export const useEPGStore = create<EPGState>((set) => ({
  programs: [],
  schedules: [],
  currentDate: new Date(),
  loading: false,
  error: null,

  setPrograms: (programs) => set({ programs }),
  setSchedules: (schedules) => set({ schedules }),
  setCurrentDate: (date) => set({ currentDate: date }),
  setLoading: (loading) => set({ loading }),
  setError: (error) => set({ error }),
}));
