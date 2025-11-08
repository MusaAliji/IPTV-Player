/**
 * HTTP Client Tests
 */

import {
  buildUrl,
  createDefaultHeaders,
  isHttpError,
  getErrorMessage,
  HttpError
} from '../src/utils/http-client';

describe('HTTP Client Utilities', () => {
  describe('buildUrl', () => {
    it('should return base URL when no params', () => {
      const url = buildUrl('https://api.example.com/users');
      expect(url).toBe('https://api.example.com/users');
    });

    it('should append query parameters', () => {
      const url = buildUrl('https://api.example.com/users', {
        page: 1,
        limit: 20
      });
      expect(url).toContain('page=1');
      expect(url).toContain('limit=20');
    });

    it('should handle array parameters', () => {
      const url = buildUrl('https://api.example.com/users', {
        ids: [1, 2, 3]
      });
      expect(url).toContain('ids=1');
      expect(url).toContain('ids=2');
      expect(url).toContain('ids=3');
    });

    it('should skip undefined and null values', () => {
      const url = buildUrl('https://api.example.com/users', {
        page: 1,
        name: undefined,
        email: null
      });
      expect(url).toContain('page=1');
      expect(url).not.toContain('name');
      expect(url).not.toContain('email');
    });
  });

  describe('createDefaultHeaders', () => {
    it('should create default headers without auth', () => {
      const headers = createDefaultHeaders();
      expect(headers['Content-Type']).toBe('application/json');
      expect(headers['Accept']).toBe('application/json');
      expect(headers['Authorization']).toBeUndefined();
    });

    it('should create headers with auth token', () => {
      const headers = createDefaultHeaders(true, 'test-token');
      expect(headers['Content-Type']).toBe('application/json');
      expect(headers['Accept']).toBe('application/json');
      expect(headers['Authorization']).toBe('Bearer test-token');
    });

    it('should not add auth header if token is missing', () => {
      const headers = createDefaultHeaders(true);
      expect(headers['Authorization']).toBeUndefined();
    });
  });

  describe('HttpError', () => {
    it('should create HTTP error with all properties', () => {
      const error = new HttpError('Not found', 404, 'Not Found', { id: 123 });
      expect(error.message).toBe('Not found');
      expect(error.status).toBe(404);
      expect(error.statusText).toBe('Not Found');
      expect(error.data).toEqual({ id: 123 });
      expect(error.name).toBe('HttpError');
    });
  });

  describe('isHttpError', () => {
    it('should return true for HttpError', () => {
      const error = new HttpError('Error', 500, 'Internal Server Error');
      expect(isHttpError(error)).toBe(true);
    });

    it('should return false for regular Error', () => {
      const error = new Error('Regular error');
      expect(isHttpError(error)).toBe(false);
    });

    it('should return false for non-error objects', () => {
      expect(isHttpError({})).toBe(false);
      expect(isHttpError('string')).toBe(false);
      expect(isHttpError(null)).toBe(false);
    });
  });

  describe('getErrorMessage', () => {
    it('should get message from HttpError with data', () => {
      const error = new HttpError('Error', 400, 'Bad Request', { message: 'Invalid input' });
      expect(getErrorMessage(error)).toBe('Invalid input');
    });

    it('should get message from HttpError without data', () => {
      const error = new HttpError('Error message', 500, 'Internal Server Error');
      expect(getErrorMessage(error)).toBe('Error message');
    });

    it('should get message from regular Error', () => {
      const error = new Error('Something went wrong');
      expect(getErrorMessage(error)).toBe('Something went wrong');
    });

    it('should handle unknown errors', () => {
      expect(getErrorMessage('string error')).toBe('An unknown error occurred');
      expect(getErrorMessage({})).toBe('An unknown error occurred');
    });
  });
});
