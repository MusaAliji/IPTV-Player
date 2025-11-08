/**
 * Formatter Tests
 */

import {
  formatDuration,
  formatDurationHuman,
  formatDate,
  formatTime,
  formatDateTime,
  formatEPGTime,
  formatRelativeTime,
  formatFileSize,
  formatBitrate,
  formatPercentage,
  truncateText,
  capitalizeFirst,
  toTitleCase,
  formatNumber,
  formatViewerCount
} from '../src/utils/formatters';

describe('Formatters', () => {
  describe('formatDuration', () => {
    it('should format seconds to MM:SS', () => {
      expect(formatDuration(125)).toBe('02:05');
    });

    it('should format seconds to HH:MM:SS', () => {
      expect(formatDuration(3665)).toBe('01:01:05');
    });

    it('should handle zero seconds', () => {
      expect(formatDuration(0)).toBe('00:00');
    });

    it('should handle negative seconds', () => {
      expect(formatDuration(-10)).toBe('00:00');
    });
  });

  describe('formatDurationHuman', () => {
    it('should format seconds only', () => {
      expect(formatDurationHuman(45)).toBe('45s');
    });

    it('should format minutes and seconds', () => {
      expect(formatDurationHuman(125)).toBe('2m 5s');
    });

    it('should format hours and minutes', () => {
      expect(formatDurationHuman(3665)).toBe('1h 1m');
    });

    it('should handle zero', () => {
      expect(formatDurationHuman(0)).toBe('0s');
    });
  });

  describe('formatDate', () => {
    it('should format ISO date string', () => {
      const result = formatDate('2025-01-15T12:00:00Z');
      expect(result).toContain('Jan');
      expect(result).toContain('15');
      expect(result).toContain('2025');
    });

    it('should handle invalid date', () => {
      expect(formatDate('invalid')).toBe('Invalid Date');
    });
  });

  describe('formatTime', () => {
    it('should format ISO date string to time', () => {
      const result = formatTime('2025-01-15T15:30:00Z');
      expect(result).toMatch(/\d{1,2}:\d{2}\s(AM|PM)/);
    });

    it('should handle invalid time', () => {
      expect(formatTime('invalid')).toBe('Invalid Time');
    });
  });

  describe('formatDateTime', () => {
    it('should format ISO date string to datetime', () => {
      const result = formatDateTime('2025-01-15T15:30:00Z');
      expect(result).toContain('Jan');
      expect(result).toContain('15');
      expect(result).toContain('2025');
    });

    it('should handle invalid datetime', () => {
      expect(formatDateTime('invalid')).toBe('Invalid DateTime');
    });
  });

  describe('formatEPGTime', () => {
    it('should format time for EPG', () => {
      const result = formatEPGTime('2025-01-15T15:30:00Z');
      expect(result).toMatch(/\d{2}:\d{2}/);
    });

    it('should handle invalid time', () => {
      expect(formatEPGTime('invalid')).toBe('--:--');
    });
  });

  describe('formatRelativeTime', () => {
    it('should format recent time as "just now"', () => {
      const now = new Date();
      expect(formatRelativeTime(now.toISOString())).toBe('just now');
    });

    it('should format minutes ago', () => {
      const date = new Date(Date.now() - 5 * 60 * 1000);
      expect(formatRelativeTime(date.toISOString())).toBe('5 minutes ago');
    });

    it('should format hours ago', () => {
      const date = new Date(Date.now() - 2 * 60 * 60 * 1000);
      expect(formatRelativeTime(date.toISOString())).toBe('2 hours ago');
    });
  });

  describe('formatFileSize', () => {
    it('should format bytes', () => {
      expect(formatFileSize(0)).toBe('0 Bytes');
      expect(formatFileSize(100)).toBe('100 Bytes');
    });

    it('should format kilobytes', () => {
      expect(formatFileSize(1024)).toBe('1 KB');
    });

    it('should format megabytes', () => {
      expect(formatFileSize(1048576)).toBe('1 MB');
    });

    it('should format gigabytes', () => {
      expect(formatFileSize(1073741824)).toBe('1 GB');
    });
  });

  describe('formatBitrate', () => {
    it('should format kbps', () => {
      expect(formatBitrate(500)).toBe('500 kbps');
    });

    it('should format Mbps', () => {
      expect(formatBitrate(5000)).toBe('5.0 Mbps');
    });
  });

  describe('formatPercentage', () => {
    it('should format percentage with default decimals', () => {
      expect(formatPercentage(75)).toBe('75%');
    });

    it('should format percentage with specified decimals', () => {
      expect(formatPercentage(75.5678, 2)).toBe('75.57%');
    });
  });

  describe('truncateText', () => {
    it('should not truncate short text', () => {
      expect(truncateText('Hello', 10)).toBe('Hello');
    });

    it('should truncate long text', () => {
      expect(truncateText('This is a long text', 10)).toBe('This is...');
    });
  });

  describe('capitalizeFirst', () => {
    it('should capitalize first letter', () => {
      expect(capitalizeFirst('hello')).toBe('Hello');
    });

    it('should handle empty string', () => {
      expect(capitalizeFirst('')).toBe('');
    });
  });

  describe('toTitleCase', () => {
    it('should convert to title case', () => {
      expect(toTitleCase('hello world')).toBe('Hello World');
    });

    it('should handle empty string', () => {
      expect(toTitleCase('')).toBe('');
    });
  });

  describe('formatNumber', () => {
    it('should format number with separators', () => {
      expect(formatNumber(1000)).toBe('1,000');
      expect(formatNumber(1000000)).toBe('1,000,000');
    });

    it('should handle small numbers', () => {
      expect(formatNumber(100)).toBe('100');
    });
  });

  describe('formatViewerCount', () => {
    it('should format small counts', () => {
      expect(formatViewerCount(500)).toBe('500');
    });

    it('should format thousands', () => {
      expect(formatViewerCount(1500)).toBe('1.5K');
    });

    it('should format millions', () => {
      expect(formatViewerCount(2500000)).toBe('2.5M');
    });
  });
});
