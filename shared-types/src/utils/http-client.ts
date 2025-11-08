/**
 * HTTP Client Helpers
 * Utilities for making HTTP requests and handling responses
 */

import { HTTP_STATUS, TIMEOUTS } from '../constants';

/**
 * HTTP request options
 */
export interface HttpRequestOptions {
  method?: 'GET' | 'POST' | 'PUT' | 'PATCH' | 'DELETE';
  headers?: Record<string, string>;
  body?: any;
  timeout?: number;
  credentials?: RequestCredentials;
}

/**
 * HTTP response wrapper
 */
export interface HttpResponse<T = any> {
  data: T;
  status: number;
  statusText: string;
  headers: Headers;
}

/**
 * HTTP error
 */
export class HttpError extends Error {
  constructor(
    message: string,
    public status: number,
    public statusText: string,
    public data?: any
  ) {
    super(message);
    this.name = 'HttpError';
  }
}

/**
 * Creates default headers for JSON requests
 */
export function createDefaultHeaders(includeAuth: boolean = false, token?: string): Record<string, string> {
  const headers: Record<string, string> = {
    'Content-Type': 'application/json',
    'Accept': 'application/json'
  };

  if (includeAuth && token) {
    headers['Authorization'] = `Bearer ${token}`;
  }

  return headers;
}

/**
 * Builds a URL with query parameters
 */
export function buildUrl(baseUrl: string, params?: Record<string, any>): string {
  if (!params || Object.keys(params).length === 0) {
    return baseUrl;
  }

  const url = new URL(baseUrl);
  Object.entries(params).forEach(([key, value]) => {
    if (value !== undefined && value !== null) {
      if (Array.isArray(value)) {
        value.forEach(item => url.searchParams.append(key, String(item)));
      } else {
        url.searchParams.append(key, String(value));
      }
    }
  });

  return url.toString();
}

/**
 * Makes an HTTP request with timeout support
 */
export async function makeRequest<T = any>(
  url: string,
  options: HttpRequestOptions = {}
): Promise<HttpResponse<T>> {
  const {
    method = 'GET',
    headers = {},
    body,
    timeout = TIMEOUTS.API_REQUEST,
    credentials = 'same-origin'
  } = options;

  const controller = new AbortController();
  const timeoutId = setTimeout(() => controller.abort(), timeout);

  try {
    const requestInit: RequestInit = {
      method,
      headers,
      credentials,
      signal: controller.signal
    };

    if (body && method !== 'GET') {
      requestInit.body = typeof body === 'string' ? body : JSON.stringify(body);
    }

    const response = await fetch(url, requestInit);
    clearTimeout(timeoutId);

    // Try to parse response body
    let data: T;
    const contentType = response.headers.get('content-type');
    if (contentType?.includes('application/json')) {
      data = await response.json() as T;
    } else {
      data = await response.text() as any;
    }

    // Check if response is ok
    if (!response.ok) {
      throw new HttpError(
        `HTTP Error: ${response.statusText}`,
        response.status,
        response.statusText,
        data
      );
    }

    return {
      data,
      status: response.status,
      statusText: response.statusText,
      headers: response.headers
    };
  } catch (error) {
    clearTimeout(timeoutId);

    if (error instanceof HttpError) {
      throw error;
    }

    if ((error as any).name === 'AbortError') {
      throw new HttpError('Request timeout', 408, 'Request Timeout');
    }

    throw new HttpError(
      error instanceof Error ? error.message : 'Unknown error',
      0,
      'Network Error'
    );
  }
}

/**
 * GET request helper
 */
export async function get<T = any>(
  url: string,
  params?: Record<string, any>,
  headers?: Record<string, string>
): Promise<HttpResponse<T>> {
  const finalUrl = buildUrl(url, params);
  return makeRequest<T>(finalUrl, { method: 'GET', headers });
}

/**
 * POST request helper
 */
export async function post<T = any>(
  url: string,
  body?: any,
  headers?: Record<string, string>
): Promise<HttpResponse<T>> {
  return makeRequest<T>(url, { method: 'POST', body, headers });
}

/**
 * PUT request helper
 */
export async function put<T = any>(
  url: string,
  body?: any,
  headers?: Record<string, string>
): Promise<HttpResponse<T>> {
  return makeRequest<T>(url, { method: 'PUT', body, headers });
}

/**
 * PATCH request helper
 */
export async function patch<T = any>(
  url: string,
  body?: any,
  headers?: Record<string, string>
): Promise<HttpResponse<T>> {
  return makeRequest<T>(url, { method: 'PATCH', body, headers });
}

/**
 * DELETE request helper
 */
export async function del<T = any>(
  url: string,
  headers?: Record<string, string>
): Promise<HttpResponse<T>> {
  return makeRequest<T>(url, { method: 'DELETE', headers });
}

/**
 * Checks if an error is an HTTP error
 */
export function isHttpError(error: any): error is HttpError {
  return error instanceof HttpError;
}

/**
 * Gets error message from various error types
 */
export function getErrorMessage(error: any): string {
  if (isHttpError(error)) {
    return error.data?.message || error.message;
  }
  if (error instanceof Error) {
    return error.message;
  }
  return 'An unknown error occurred';
}

/**
 * Retry logic for failed requests
 */
export async function retryRequest<T = any>(
  requestFn: () => Promise<HttpResponse<T>>,
  maxRetries: number = 3,
  delayMs: number = 1000
): Promise<HttpResponse<T>> {
  let lastError: Error;

  for (let attempt = 0; attempt < maxRetries; attempt++) {
    try {
      return await requestFn();
    } catch (error) {
      lastError = error as Error;

      // Don't retry on client errors (4xx) except 408 (timeout) and 429 (rate limit)
      if (isHttpError(error)) {
        const status = error.status;
        if (status >= 400 && status < 500 && status !== 408 && status !== 429) {
          throw error;
        }
      }

      // Wait before retrying (exponential backoff)
      if (attempt < maxRetries - 1) {
        await new Promise(resolve => setTimeout(resolve, delayMs * Math.pow(2, attempt)));
      }
    }
  }

  throw lastError!;
}
