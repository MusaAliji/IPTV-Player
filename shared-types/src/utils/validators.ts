/**
 * Validation Utilities
 * Common validation functions for user input
 */

import { VALIDATION } from '../constants/app.constants';

/**
 * Validation result
 */
export interface ValidationResult {
  isValid: boolean;
  errors: string[];
}

/**
 * Validates an email address
 */
export function validateEmail(email: string): ValidationResult {
  const errors: string[] = [];

  if (!email) {
    errors.push('Email is required');
  } else {
    if (email.length > VALIDATION.EMAIL.MAX_LENGTH) {
      errors.push(`Email must be no more than ${VALIDATION.EMAIL.MAX_LENGTH} characters`);
    }
    if (!VALIDATION.EMAIL.PATTERN.test(email)) {
      errors.push('Email format is invalid');
    }
  }

  return {
    isValid: errors.length === 0,
    errors
  };
}

/**
 * Validates a username
 */
export function validateUsername(username: string): ValidationResult {
  const errors: string[] = [];

  if (!username) {
    errors.push('Username is required');
  } else {
    if (username.length < VALIDATION.USERNAME.MIN_LENGTH) {
      errors.push(`Username must be at least ${VALIDATION.USERNAME.MIN_LENGTH} characters`);
    }
    if (username.length > VALIDATION.USERNAME.MAX_LENGTH) {
      errors.push(`Username must be no more than ${VALIDATION.USERNAME.MAX_LENGTH} characters`);
    }
    if (!VALIDATION.USERNAME.PATTERN.test(username)) {
      errors.push('Username can only contain letters, numbers, underscores, and hyphens');
    }
  }

  return {
    isValid: errors.length === 0,
    errors
  };
}

/**
 * Validates a password
 */
export function validatePassword(password: string): ValidationResult {
  const errors: string[] = [];

  if (!password) {
    errors.push('Password is required');
  } else {
    if (password.length < VALIDATION.PASSWORD.MIN_LENGTH) {
      errors.push(`Password must be at least ${VALIDATION.PASSWORD.MIN_LENGTH} characters`);
    }
    if (password.length > VALIDATION.PASSWORD.MAX_LENGTH) {
      errors.push(`Password must be no more than ${VALIDATION.PASSWORD.MAX_LENGTH} characters`);
    }
    if (VALIDATION.PASSWORD.REQUIRE_UPPERCASE && !/[A-Z]/.test(password)) {
      errors.push('Password must contain at least one uppercase letter');
    }
    if (VALIDATION.PASSWORD.REQUIRE_LOWERCASE && !/[a-z]/.test(password)) {
      errors.push('Password must contain at least one lowercase letter');
    }
    if (VALIDATION.PASSWORD.REQUIRE_NUMBER && !/\d/.test(password)) {
      errors.push('Password must contain at least one number');
    }
    if (VALIDATION.PASSWORD.REQUIRE_SPECIAL_CHAR && !/[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/.test(password)) {
      errors.push('Password must contain at least one special character');
    }
  }

  return {
    isValid: errors.length === 0,
    errors
  };
}

/**
 * Validates a URL
 */
export function validateUrl(url: string): ValidationResult {
  const errors: string[] = [];

  if (!url) {
    errors.push('URL is required');
  } else {
    try {
      new URL(url);
    } catch {
      errors.push('URL format is invalid');
    }
  }

  return {
    isValid: errors.length === 0,
    errors
  };
}

/**
 * Validates a stream URL (can be HTTP, HTTPS, RTMP, etc.)
 */
export function validateStreamUrl(url: string): ValidationResult {
  const errors: string[] = [];

  if (!url) {
    errors.push('Stream URL is required');
  } else {
    const validProtocols = ['http:', 'https:', 'rtmp:', 'rtmps:', 'ws:', 'wss:'];
    try {
      const parsedUrl = new URL(url);
      if (!validProtocols.includes(parsedUrl.protocol)) {
        errors.push('Stream URL must use a valid protocol (HTTP, HTTPS, RTMP, WebSocket)');
      }
    } catch {
      errors.push('Stream URL format is invalid');
    }
  }

  return {
    isValid: errors.length === 0,
    errors
  };
}

/**
 * Validates a content title
 */
export function validateContentTitle(title: string): ValidationResult {
  const errors: string[] = [];

  if (!title) {
    errors.push('Title is required');
  } else {
    if (title.length < VALIDATION.CONTENT_TITLE.MIN_LENGTH) {
      errors.push(`Title must be at least ${VALIDATION.CONTENT_TITLE.MIN_LENGTH} character`);
    }
    if (title.length > VALIDATION.CONTENT_TITLE.MAX_LENGTH) {
      errors.push(`Title must be no more than ${VALIDATION.CONTENT_TITLE.MAX_LENGTH} characters`);
    }
  }

  return {
    isValid: errors.length === 0,
    errors
  };
}

/**
 * Validates a content description
 */
export function validateContentDescription(description: string): ValidationResult {
  const errors: string[] = [];

  if (description && description.length > VALIDATION.CONTENT_DESCRIPTION.MAX_LENGTH) {
    errors.push(`Description must be no more than ${VALIDATION.CONTENT_DESCRIPTION.MAX_LENGTH} characters`);
  }

  return {
    isValid: errors.length === 0,
    errors
  };
}

/**
 * Validates a channel name
 */
export function validateChannelName(name: string): ValidationResult {
  const errors: string[] = [];

  if (!name) {
    errors.push('Channel name is required');
  } else {
    if (name.length < VALIDATION.CHANNEL_NAME.MIN_LENGTH) {
      errors.push(`Channel name must be at least ${VALIDATION.CHANNEL_NAME.MIN_LENGTH} character`);
    }
    if (name.length > VALIDATION.CHANNEL_NAME.MAX_LENGTH) {
      errors.push(`Channel name must be no more than ${VALIDATION.CHANNEL_NAME.MAX_LENGTH} characters`);
    }
  }

  return {
    isValid: errors.length === 0,
    errors
  };
}

/**
 * Validates a rating value
 */
export function validateRating(rating: number): ValidationResult {
  const errors: string[] = [];

  if (rating < 0 || rating > 10) {
    errors.push('Rating must be between 0 and 10');
  }

  return {
    isValid: errors.length === 0,
    errors
  };
}

/**
 * Validates a date string (ISO 8601 format)
 */
export function validateDateString(dateString: string): ValidationResult {
  const errors: string[] = [];

  if (!dateString) {
    errors.push('Date is required');
  } else {
    const date = new Date(dateString);
    if (isNaN(date.getTime())) {
      errors.push('Date format is invalid');
    }
  }

  return {
    isValid: errors.length === 0,
    errors
  };
}

/**
 * Validates a time range
 */
export function validateTimeRange(startTime: string, endTime: string): ValidationResult {
  const errors: string[] = [];

  const startValidation = validateDateString(startTime);
  const endValidation = validateDateString(endTime);

  if (!startValidation.isValid) {
    errors.push(...startValidation.errors);
  }
  if (!endValidation.isValid) {
    errors.push(...endValidation.errors);
  }

  if (startValidation.isValid && endValidation.isValid) {
    const start = new Date(startTime);
    const end = new Date(endTime);
    if (start >= end) {
      errors.push('Start time must be before end time');
    }
  }

  return {
    isValid: errors.length === 0,
    errors
  };
}
