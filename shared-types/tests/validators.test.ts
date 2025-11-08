/**
 * Validator Tests
 */

import {
  validateEmail,
  validateUsername,
  validatePassword,
  validateUrl,
  validateStreamUrl,
  validateContentTitle,
  validateContentDescription,
  validateChannelName,
  validateRating,
  validateDateString,
  validateTimeRange
} from '../src/utils/validators';

describe('Validators', () => {
  describe('validateEmail', () => {
    it('should validate correct email', () => {
      const result = validateEmail('user@example.com');
      expect(result.isValid).toBe(true);
      expect(result.errors).toHaveLength(0);
    });

    it('should reject empty email', () => {
      const result = validateEmail('');
      expect(result.isValid).toBe(false);
      expect(result.errors).toContain('Email is required');
    });

    it('should reject invalid email format', () => {
      const result = validateEmail('invalid-email');
      expect(result.isValid).toBe(false);
      expect(result.errors.length).toBeGreaterThan(0);
    });

    it('should reject email that is too long', () => {
      const longEmail = 'a'.repeat(256) + '@example.com';
      const result = validateEmail(longEmail);
      expect(result.isValid).toBe(false);
    });
  });

  describe('validateUsername', () => {
    it('should validate correct username', () => {
      const result = validateUsername('user123');
      expect(result.isValid).toBe(true);
      expect(result.errors).toHaveLength(0);
    });

    it('should reject empty username', () => {
      const result = validateUsername('');
      expect(result.isValid).toBe(false);
      expect(result.errors).toContain('Username is required');
    });

    it('should reject username that is too short', () => {
      const result = validateUsername('ab');
      expect(result.isValid).toBe(false);
    });

    it('should reject username with invalid characters', () => {
      const result = validateUsername('user@123');
      expect(result.isValid).toBe(false);
    });
  });

  describe('validatePassword', () => {
    it('should validate correct password', () => {
      const result = validatePassword('Password123');
      expect(result.isValid).toBe(true);
      expect(result.errors).toHaveLength(0);
    });

    it('should reject empty password', () => {
      const result = validatePassword('');
      expect(result.isValid).toBe(false);
      expect(result.errors).toContain('Password is required');
    });

    it('should reject password that is too short', () => {
      const result = validatePassword('Pass1');
      expect(result.isValid).toBe(false);
    });

    it('should reject password without uppercase', () => {
      const result = validatePassword('password123');
      expect(result.isValid).toBe(false);
    });

    it('should reject password without lowercase', () => {
      const result = validatePassword('PASSWORD123');
      expect(result.isValid).toBe(false);
    });

    it('should reject password without number', () => {
      const result = validatePassword('Password');
      expect(result.isValid).toBe(false);
    });
  });

  describe('validateUrl', () => {
    it('should validate correct URL', () => {
      const result = validateUrl('https://example.com');
      expect(result.isValid).toBe(true);
      expect(result.errors).toHaveLength(0);
    });

    it('should reject empty URL', () => {
      const result = validateUrl('');
      expect(result.isValid).toBe(false);
    });

    it('should reject invalid URL', () => {
      const result = validateUrl('not-a-url');
      expect(result.isValid).toBe(false);
    });
  });

  describe('validateStreamUrl', () => {
    it('should validate HTTP stream URL', () => {
      const result = validateStreamUrl('http://example.com/stream');
      expect(result.isValid).toBe(true);
    });

    it('should validate HTTPS stream URL', () => {
      const result = validateStreamUrl('https://example.com/stream');
      expect(result.isValid).toBe(true);
    });

    it('should validate RTMP stream URL', () => {
      const result = validateStreamUrl('rtmp://example.com/live/stream');
      expect(result.isValid).toBe(true);
    });

    it('should validate WebSocket stream URL', () => {
      const result = validateStreamUrl('ws://example.com/stream');
      expect(result.isValid).toBe(true);
    });

    it('should reject invalid protocol', () => {
      const result = validateStreamUrl('ftp://example.com/stream');
      expect(result.isValid).toBe(false);
    });
  });

  describe('validateContentTitle', () => {
    it('should validate correct title', () => {
      const result = validateContentTitle('Sample Movie');
      expect(result.isValid).toBe(true);
    });

    it('should reject empty title', () => {
      const result = validateContentTitle('');
      expect(result.isValid).toBe(false);
    });

    it('should reject title that is too long', () => {
      const longTitle = 'a'.repeat(201);
      const result = validateContentTitle(longTitle);
      expect(result.isValid).toBe(false);
    });
  });

  describe('validateContentDescription', () => {
    it('should validate correct description', () => {
      const result = validateContentDescription('This is a sample description.');
      expect(result.isValid).toBe(true);
    });

    it('should allow empty description', () => {
      const result = validateContentDescription('');
      expect(result.isValid).toBe(true);
    });

    it('should reject description that is too long', () => {
      const longDescription = 'a'.repeat(2001);
      const result = validateContentDescription(longDescription);
      expect(result.isValid).toBe(false);
    });
  });

  describe('validateChannelName', () => {
    it('should validate correct channel name', () => {
      const result = validateChannelName('CNN');
      expect(result.isValid).toBe(true);
    });

    it('should reject empty channel name', () => {
      const result = validateChannelName('');
      expect(result.isValid).toBe(false);
    });
  });

  describe('validateRating', () => {
    it('should validate rating in valid range', () => {
      const result = validateRating(7.5);
      expect(result.isValid).toBe(true);
    });

    it('should reject rating below 0', () => {
      const result = validateRating(-1);
      expect(result.isValid).toBe(false);
    });

    it('should reject rating above 10', () => {
      const result = validateRating(11);
      expect(result.isValid).toBe(false);
    });
  });

  describe('validateDateString', () => {
    it('should validate correct ISO date string', () => {
      const result = validateDateString('2025-01-15T12:00:00Z');
      expect(result.isValid).toBe(true);
    });

    it('should reject empty date string', () => {
      const result = validateDateString('');
      expect(result.isValid).toBe(false);
    });

    it('should reject invalid date string', () => {
      const result = validateDateString('not-a-date');
      expect(result.isValid).toBe(false);
    });
  });

  describe('validateTimeRange', () => {
    it('should validate valid time range', () => {
      const result = validateTimeRange('2025-01-15T12:00:00Z', '2025-01-15T14:00:00Z');
      expect(result.isValid).toBe(true);
    });

    it('should reject when start time is after end time', () => {
      const result = validateTimeRange('2025-01-15T14:00:00Z', '2025-01-15T12:00:00Z');
      expect(result.isValid).toBe(false);
    });

    it('should reject when start time equals end time', () => {
      const result = validateTimeRange('2025-01-15T12:00:00Z', '2025-01-15T12:00:00Z');
      expect(result.isValid).toBe(false);
    });
  });
});
